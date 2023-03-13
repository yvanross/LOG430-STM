using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Channels;
using Ambassador.BusinessObjects;
using Ambassador.BusinessObjects.InterServiceRequests;
using Ambassador.Health;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;

namespace Ambassador.Usecases
{
    internal class RestUC
    {
        private IngressRoutingUC _ingressRoutingUc = new ();

        internal async Task<ChannelReader<RestResponse>> Get(GetRoutingRequest routingRequest)
        {
            var destinationRoutingDatas = await GetDestinationRoutingData(routingRequest, Method.Get);

            var channel = Channel.CreateUnbounded<RestResponse>();

            var tasks = destinationRoutingDatas.ConvertAll(routingData => 
                routingData.client.ExecuteAsync(routingData.request)
                .ContinueWith(task =>
                {
                    if (task.Result.IsSuccessStatusCode)
                        return channel.Writer.WriteAsync(task.Result);

                    ContainerService.Logger?.LogInformation(task.Exception?.Message);
                    return ValueTask.CompletedTask;
                }));

            _ = Task.WhenAll(tasks).ContinueWith(_=>channel.Writer.Complete()).ConfigureAwait(false);

            return channel.Reader;
        }

        internal async Task<ChannelReader<RestResponse>> Post<T>(PostRoutingRequest<T> routingRequest) where T : class
        {
            var destinationRoutingDatas = await GetDestinationRoutingData(routingRequest, Method.Post);

            destinationRoutingDatas.ForEach(tuple => tuple.request.AddJsonBody(routingRequest.Payload));
            
            var channel = Channel.CreateUnbounded<RestResponse>();

            var tasks = destinationRoutingDatas.ConvertAll(routingData =>
                routingData.client.ExecuteAsync(routingData.request)
                    .ContinueWith(task =>
                    {
                        if (task.Result.IsSuccessStatusCode)
                            return channel.Writer.WriteAsync(task.Result);

                        ContainerService.Logger?.LogInformation(task.Exception?.Message);
                        return ValueTask.CompletedTask;
                    }));

            _ = Task.WhenAll(tasks).ContinueWith(_ => channel.Writer.Complete()).ConfigureAwait(false);

            return channel.Reader;
        }

        private async Task<List<(RestClient client, RestRequest request)>> GetDestinationRoutingData(ServiceRoutingRequest routingRequest, Method method)
        {
            try
            {
                var routingInfos = await _ingressRoutingUc.GetServiceRoutingData(routingRequest.TargetService, routingRequest.Mode);

                var routingData = DecorateRequest(routingInfos).ToList();

                return routingData;
            }
            catch (Exception e)
            {
                var exception = GetExceptionMessage(e, "IngressController Routing Data exception");

                ContainerService.Logger?.LogError(exception.Message);

                throw exception;
            }

            IEnumerable<(RestClient client, RestRequest request)> DecorateRequest(IEnumerable<RoutingData> routingInfos)
            {
                foreach (var routingInfo in routingInfos)
                {
                    var client = new RestClient(routingInfo.Address);

                    var request = new RestRequest(routingRequest.Endpoint, method);

                    routingInfo.IngressAddedHeaders.ForEach(header => request.AddHeader(header.Name, header.Value));

                    routingInfo.IngressAddedQueryParams.ForEach(queryParams =>
                        request.AddQueryParameter(queryParams.Name, queryParams.Value));

                    routingRequest.Params.ForEach(param => request.AddQueryParameter(param.Name, param.Value));

                    yield return (client, request);
                }
            }
        }

        private Exception GetExceptionMessage(Exception e, string type)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append($"{e.Message} \n {type} \n\n\t StackTrace: {e.StackTrace}");

            Exception? innerException = e.InnerException;

            int level = 1;

            while (innerException is not null)
            {
                sb.Append($"\n\n\t Inner Exception {level}: {e.Message} \n\t StackTrace: {e.StackTrace}");

                level++;

                innerException = innerException.InnerException;
            }

            return new Exception(sb.ToString());
        }
    }
}
