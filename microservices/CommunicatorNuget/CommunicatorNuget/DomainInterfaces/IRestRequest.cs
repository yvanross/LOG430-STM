using CommunicatorNuget.BusinessObjects;

namespace CommunicatorNuget.DomainInterfaces;

public interface IRestRequest
{
    string Address { get; set; }

    string Endpoint { get; set; }

    INameValue[] Headers { get; set; }

    INameValue[] Params { get; set; }

    public Execute()
    {

    }
}