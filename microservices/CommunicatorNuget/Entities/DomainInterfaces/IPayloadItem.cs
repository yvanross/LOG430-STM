namespace Entities.DomainInterfaces;

public interface IPayloadItem
{
    string paramName { get; set; }

    string value { get; set; }
}