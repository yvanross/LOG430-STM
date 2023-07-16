using System.Text;
using System.Threading.Channels;
using RestSharp;
using ServiceMeshHelper.Bo;
using ServiceMeshHelper.Bo.InterServiceRequests;
using ServiceMeshHelper.Services;

namespace ServiceMeshHelper.Usecases
{
    internal class Rest
    {
        private NodeControllerRoutingClient nodeController = new ();

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

                    return ValueTask.CompletedTask;
                }));

            _ = Task.WhenAll(tasks).ContinueWith(_=>channel.Writer.Complete());

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

                        return ValueTask.CompletedTask;
                    }));

            _ = Task.WhenAll(tasks).ContinueWith(_ => channel.Writer.Complete());

            return channel.Reader;
        }

        internal Task<IEnumerable<RoutingData>> GetServiceRoutingData(string targetService, LoadBalancingMode mode)
        {
            return nodeController.RouteByServiceType(targetService, mode);
        }

        private async Task<List<(RestClient client, RestRequest request)>> GetDestinationRoutingData(ServiceRoutingRequest routingRequest, Method method)
        {
            try
            {
                var routingInfos = await GetServiceRoutingData(routingRequest.TargetService, routingRequest.Mode);

                var routingData = DecorateRequest(routingInfos).ToList();

                return routingData;
            }
            catch (Exception e)
            {
                var exception = GetExceptionMessage(e, "NodeController Routing Data exception");

                throw exception;
            }

            IEnumerable<(RestClient client, RestRequest request)> DecorateRequest(IEnumerable<RoutingData> routingInfos)
            {
                foreach (var routingInfo in routingInfos)
                {
                    var client = new RestClient(routingInfo.Address);

                    var request = new RestRequest(routingRequest.Endpoint, method);

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
