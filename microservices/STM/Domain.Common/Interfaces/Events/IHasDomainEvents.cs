namespace Domain.Common.Interfaces.Events;

public interface IHasDomainEvents
{
    IEnumerable<IDomainEvent> DomainEvents { get; }

    void ClearDomainEvents();
}