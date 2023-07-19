using System.Collections.Immutable;
using Entities.Dao;
using Entities.DomainInterfaces.Planned;
using Entities.Extensions;

namespace Entities.BusinessObjects.Planned;

public class PodType : IPodType
{
    private readonly IPodReadService _podReadService;

    private SemaphoreSlim _semaphore = new (1);

    public PodType(IPodReadService podReadService)
    {
        _podReadService = podReadService;
    }

    private ImmutableList<string> _linkedServiceTypes = ImmutableList<string>.Empty;

    private ImmutableList<string> _serviceTypes = ImmutableList<string>.Empty;

    private ImmutableList<string> _replicasHostnames = ImmutableList<string>.Empty;

    private string _podLeader;

    public required string Type { get; set; }

    public required bool ShareVolumes { get; set; }

    public int NumberOfInstances => _replicasHostnames.Count;

    public ImmutableList<string> ReplicasHostnames => _replicasHostnames;

    public ImmutableList<IServiceType> ServiceTypes
    {
        get
        {
            if (_serviceTypes.Count.Equals(_linkedServiceTypes.Count) is false && _semaphore.CurrentCount > 0 && _semaphore.Wait(100))
            {
                var unregisteredServicesNames = _linkedServiceTypes
                    .WhereNotTrueInAnyOf(_serviceTypes, (typeName, serviceType) => typeName.EqualsIgnoreCase(serviceType)) .ToList();

                var unregisteredServiceTypes = unregisteredServicesNames.Select(_podReadService.GetServiceType).NotNull();

                ImmutableInterlocked.Update(ref _serviceTypes, (serviceTypes) => serviceTypes.AddRange(unregisteredServiceTypes.Select(st => st.Type)));

                _semaphore.Release();
            }

            return _serviceTypes.ConvertAll(_podReadService.GetServiceType).NotNull().ToImmutableList();
        }
    }

    public IServiceType? PodLeader => _podReadService.GetServiceType(_podLeader);

    public void IncreaseNumberOfPod()
    {
        ImmutableInterlocked.Update(ref _replicasHostnames,  (replicasHostnames) => replicasHostnames.Add($"{Type}-{_replicasHostnames.Count + 1}"));
    }

    public void DecreaseNumberOfPod()
    {
        ImmutableInterlocked.Update(ref _replicasHostnames, (replicasHostnames) => replicasHostnames.RemoveAt(replicasHostnames.Count));
    }

    public void SetNumberOfPod(int numberOfInstances)
    {
        if (_replicasHostnames.Count < numberOfInstances)
        {
            var newHostnames = new List<string>();

            for (var i = _replicasHostnames.Count; i < numberOfInstances; i++) newHostnames.Add($"{PodLeader?.Type ?? ServiceTypes.First().Type}-{i + 1}");

            ImmutableInterlocked.Update(ref _replicasHostnames, (replicasHostnames) => replicasHostnames.AddRange(newHostnames));
        }
        else if(_replicasHostnames.Count > numberOfInstances)
        {
            ImmutableInterlocked.Update(ref _replicasHostnames, (replicasHostnames) => replicasHostnames.RemoveRange(numberOfInstances-1, replicasHostnames.Count - numberOfInstances));
        }
    }

    public void AddHostname(string hostname) => ImmutableInterlocked.Update(ref _replicasHostnames, (replicasHostnames) => replicasHostnames.Add(hostname));

    public void AddRangeHostnames(IEnumerable<string> hostnames) => ImmutableInterlocked.Update(ref _replicasHostnames, (replicasHostnames) => replicasHostnames.AddRange(hostnames));

    public void AddServiceTypes(string serviceType) => ImmutableInterlocked.Update(ref _linkedServiceTypes, (linkedServiceTypes) => linkedServiceTypes.Add(serviceType));

    public void AddRangeServiceTypes(IEnumerable<string> serviceTypes) => ImmutableInterlocked.Update(ref _linkedServiceTypes, (linkedServiceTypes) => linkedServiceTypes.AddRange(serviceTypes));

    public void SetPodLeader(string podLeader)
    {
        _podLeader = podLeader;
    }

    internal ImmutableList<string> GetLinkedServiceTypes() => _linkedServiceTypes;
}