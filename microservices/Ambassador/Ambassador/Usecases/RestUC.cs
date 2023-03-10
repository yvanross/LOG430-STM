using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;
using System.Text;
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

        internal async Task<RestResponse> Get(GetRoutingRequest routingRequest)
        {
            try
            {
                var restTooling = await GetDestinationRoutingData(routingRequest, Method.Get);

                var res = await restTooling.client.ExecuteAsync(restTooling.request);

                return res;
            }
            catch (Exception e)
            {
                var exception = GetExceptionMessage(e, $"Get Routing Data exception, endpoint: {routingRequest.Endpoint}, Target Service: {routingRequest.TargetService}");

                ContainerService.Logger?.LogError(exception.Message);

                throw exception;
            }
        }

        internal async Task<RestResponse> Post<T>(PostRoutingRequest<T> routingRequest) where T : class
        {
            try
            {
                var restTooling = await GetDestinationRoutingData(routingRequest, Method.Post);

                restTooling.request.AddJsonBody(routingRequest.Payload);

                var res = await restTooling.client.ExecuteAsync(restTooling.request);

                return res;
            }
            catch (Exception e)
            {
                var exception = GetExceptionMessage(e, $"Post Routing Data exception, endpoint: {routingRequest.Endpoint}, Target Service: {routingRequest.TargetService}");

                ContainerService.Logger?.LogError(exception.Message);

                throw exception;
            }
        }

        private async Task<(RestClient client, RestRequest request)> GetDestinationRoutingData(ServiceRoutingRequest routingRequest, Method method)
        {
            try
            {
                var routingData = await _ingressRoutingUc.GetServiceRoutingData(routingRequest.TargetService);

                var client = new RestClient(routingData.Address);

                var request = new RestRequest(routingRequest.Endpoint, method);

                routingData.IngressAddedHeaders.ForEach(header => request.AddHeader(header.Name, header.Value));

                routingData.IngressAddedQueryParams.ForEach(queryParams => request.AddQueryParameter(queryParams.Name, queryParams.Value));

                routingRequest.Params.ForEach(param => request.AddQueryParameter(param.Name, param.Value));

                return (client, request);
            }
            catch (Exception e)
            {
                var exception = GetExceptionMessage(e, "Ingress Routing Data exception");

                ContainerService.Logger?.LogError(exception.Message);

                throw exception;
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
