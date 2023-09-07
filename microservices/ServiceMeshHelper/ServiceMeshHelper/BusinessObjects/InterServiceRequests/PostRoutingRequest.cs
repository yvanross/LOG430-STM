namespace ServiceMeshHelper.BusinessObjects.InterServiceRequests;

/// <inheritdoc />
/// /// <typeparam name="T">Is a matching type to the expected type by the Rest endpoint</typeparam>

public class PostRoutingRequest<T> : ServiceRoutingRequest 
{
    /// <summary>
    /// Post body to send to the service instance
    /// </summary>
    public required T Payload { get; set; }
}