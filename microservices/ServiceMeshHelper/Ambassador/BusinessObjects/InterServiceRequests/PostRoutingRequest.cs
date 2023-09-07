namespace ServiceMeshHelper.BusinessObjects.InterServiceRequests;

public class PostRoutingRequest<T> : ServiceRoutingRequest 
{
    public required T Payload { get; set; }
}