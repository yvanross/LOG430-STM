namespace Entities.BusinessObjects;

public class Microservice
{
    public required string Id { get; set; }

    public required string Name { get; set; }
    
    public required string ImageName { get; set; }
    
    public required string Status { get; set; }

    public required string Port { get; set; }
}