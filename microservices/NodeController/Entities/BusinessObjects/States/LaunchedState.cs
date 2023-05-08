using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.ResourceManagement;

namespace Entities.BusinessObjects.States;

public class LaunchedState : IServiceState
{
    public string GetStateName()
    {
        return "Started";
    }
}