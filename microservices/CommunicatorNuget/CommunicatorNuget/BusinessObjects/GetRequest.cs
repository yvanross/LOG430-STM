using CommunicatorNuget.DomainInterfaces;

namespace CommunicatorNuget.BusinessObjects;

public class GetRequest : IRestRequest
{
    public required string Address { get; set; }

    public string Endpoint { get; set; } = string.Empty;

    public required INameValue[] Headers { get; set; }

    public INameValue[] Params { get; set; } = Array.Empty<INameValue>();

}