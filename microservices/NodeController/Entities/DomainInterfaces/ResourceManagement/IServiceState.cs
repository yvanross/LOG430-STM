namespace Entities.DomainInterfaces.ResourceManagement;

public interface IServiceState
{
    public DateTime GetStateChangeTime();

    public string GetStateName();
}