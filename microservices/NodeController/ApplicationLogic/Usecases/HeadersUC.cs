using Ambassador;
using Ambassador.BusinessObjects;
using Microsoft.Net.Http.Headers;

namespace ApplicationLogic.Usecases;

public class HeadersUC
{
    private const string TomtomApiKey = "sQQL61fpIzA0TroTMOwjrUcgwutuJ7Aw";

    public void AddJsonHeader(RoutingData data)
    {
        data.IngressAddedHeaders.Add(new NameValue()
        {
            Name = HeaderNames.Accept,
            Value = "application/json"
        });
    }

    public void AddAuthorizationHeaders(RoutingData data, string serviceType)
    {
        if (serviceType.Equals(ServiceTypes.Tomtom.ToString()))
        {
            data.IngressAddedQueryParams.Add(new NameValue()
            {
                Name = "key",
                Value = TomtomApiKey
            });
        }
    }
}