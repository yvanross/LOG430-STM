namespace Ambassador.BusinessObjects;

public class RoutingData
{
    public required string Address { get; set; }

    public List<NameValue> IngressAddedHeaders { get; set; } = new ();
}