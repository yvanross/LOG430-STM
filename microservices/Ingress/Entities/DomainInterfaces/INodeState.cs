namespace Entities.DomainInterfaces;

public interface INodeState
{
    public string GetStateName();

    public void EvaluateState(INode node);
}