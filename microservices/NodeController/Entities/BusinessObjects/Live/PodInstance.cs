using System.Collections.Immutable;
using Entities.BusinessObjects.Planned;
using Entities.BusinessObjects.States;
using Entities.Dao;
using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.ResourceManagement;
using Entities.Extensions;

namespace Entities.BusinessObjects.Live;

public class PodInstance : IPodInstance
{
    private readonly IPodReadService _podReadService;

    private ImmutableList<IServiceInstance> _serviceInstances = ImmutableList<IServiceInstance>.Empty;
    private IServiceState _serviceStatus = new LaunchedState();

    public PodInstance(IPodReadService podReadService)
    {
        _podReadService = podReadService;
    }

    public required string Id { get; set; } 

    public required string Type { get; init; }

    public required ImmutableList<IServiceInstance> ServiceInstances
    {
        get => _serviceInstances;
        set => ImmutableInterlocked.Update(ref _serviceInstances, _ => value);
    }

    public required IServiceState ServiceStatus
    {
        get
        {
            if(_serviceStatus is not ReadyState) EvaluateState();

            return _serviceStatus;
        }
        set => _serviceStatus = value;
    }

    public void AddServiceInstance(IServiceInstance serviceInstance)
    {
        ImmutableInterlocked.Update(ref _serviceInstances, instances => instances.Add(serviceInstance));
    }

    public void ReplaceServiceInstance(IServiceInstance serviceInstance)
    {
        ImmutableInterlocked.Update(ref _serviceInstances, instances =>
        {
            return ServiceInstances.RemoveAll(s => s.Id.EqualsIgnoreCase(serviceInstance.Id)).Add(serviceInstance);
        });
    }

    public bool Equals(IPodInstance? other)
    {
        if (other is null) return false;

        return Id.Equals(other.Id) && Type == other.Type && ServiceInstances.Equals(other.ServiceInstances);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((PodInstance)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Type, ServiceInstances);
    }

    private void EvaluateState()
    {
        if (string.IsNullOrWhiteSpace(Type) is false &&
            _podReadService.GetPodType(Type) is PodType podType &&
            podType.GetLinkedServiceTypes().Count.Equals(_serviceInstances.Count) &&
            ServiceInstances.All(si => si.ServiceStatus is ReadyState))
        {
            ServiceStatus = new ReadyState();
        }
    }
}