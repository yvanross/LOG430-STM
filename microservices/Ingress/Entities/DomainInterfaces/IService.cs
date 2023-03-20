namespace Entities.DomainInterfaces;

public interface IService
{
    string Id { get; init; }

    string Type { get; init; }

    string PodId { get; set; }
}