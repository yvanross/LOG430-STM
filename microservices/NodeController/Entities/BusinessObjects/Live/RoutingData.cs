namespace Entities.BusinessObjects.Live;

public class RoutingData
{
    public required string Address { get; set; }

    public List<NameValue> AddedHeaders { get; set; } = new ();

    public List<NameValue> AddedQueryParams { get; set; } = new ();
}