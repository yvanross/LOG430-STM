namespace Entities.DomainInterfaces;

public interface INodeState
{
    public void EvaluateState(INode node);
}