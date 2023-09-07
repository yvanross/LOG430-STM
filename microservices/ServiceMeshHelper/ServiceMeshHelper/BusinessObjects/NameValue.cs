namespace ServiceMeshHelper.BusinessObjects;

/// <summary>
///  Represents a parameter name and value pair
/// </summary>
public class NameValue
{
    /// <summary>
    /// Name of the parameter
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Value of the parameter
    /// </summary>
    public required string Value { get; set; }
}