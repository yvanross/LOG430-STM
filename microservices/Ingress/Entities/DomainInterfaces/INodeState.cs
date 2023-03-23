namespace Entities.DomainInterfaces.ResourceManagement;

public interface INodeState
{
    public void EvaluateState(INode node);
}