using System.Collections.Generic;

namespace ServiceMeshHelper.Bo;

public class RoutingData
{
    public required string Address { get; set; }

    public List<NameValue> IngressAddedHeaders { get; set; } = new ();

    public List<NameValue> IngressAddedQueryParams { get; set; } = new ();
}