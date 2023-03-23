using ApplicationLogic.Interfaces;
using Entities.DomainInterfaces;

namespace ApplicationLogic.Usecases;

public class RoutingUC
{
    private readonly IRepositoryRead _repositoryRead;

    public RoutingUC(IRepositoryRead repositoryRead)
    {
        _repositoryRead = repositoryRead;
    }

    public string RouteByDestinationType(string nodeId)
    {
        var node = _repositoryRead.ReadNodeById(nodeId);

        if (node is null)
            throw new Exception("service was not found");

        return $"http://{node.Address}:{node.Port}";
    }
}