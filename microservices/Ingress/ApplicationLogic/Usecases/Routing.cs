using ApplicationLogic.Interfaces;

namespace ApplicationLogic.Usecases;

public class Routing
{
    private readonly IRepositoryRead _repositoryRead;

    public Routing(IRepositoryRead repositoryRead)
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