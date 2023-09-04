namespace Domain.Events.Interfaces;

public interface IHasDomainEvents
{
    IEnumerable<IDomainEvent> DomainEvents { get; }

    void ClearDomainEvents();
}