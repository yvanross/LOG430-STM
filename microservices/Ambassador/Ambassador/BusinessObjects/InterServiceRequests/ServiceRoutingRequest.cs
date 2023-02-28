namespace Ambassador.BusinessObjects.InterServiceRequests;

public class ServiceRoutingRequest
{
    public required string TargetService { get; set; }
    
    public required string Endpoint { get; set; }

    public List<NameValue> Params { get; set; } = new();

    protected ServiceRoutingRequest() { }
}