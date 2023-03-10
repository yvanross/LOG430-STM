namespace Ambassador.Dto;

public class SubscriptionDto
{
    public string ServiceType { get; }
    public string ServiceAddress { get; }
    public string ContainerId { get; }
    public Guid ServiceId { get; }
    public bool AutoScaleInstances { get; }
    public int MinimumNumberOfInstances { get; }

    public SubscriptionDto(string serviceType, string serviceAddress, string containerId, Guid serviceId,
        bool autoScaleInstances, int minimumNumberOfInstances)
    {
        MinimumNumberOfInstances = minimumNumberOfInstances;
        ServiceType = serviceType;
        ServiceAddress = serviceAddress;
        ContainerId = containerId;
        ServiceId = serviceId;
        AutoScaleInstances = autoScaleInstances;
    }
}