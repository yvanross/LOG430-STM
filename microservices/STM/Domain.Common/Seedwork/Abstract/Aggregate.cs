using Domain.Common.Interfaces.Events;

namespace Domain.Common.Seedwork.Abstract;

public abstract class Aggregate<T> : Entity<T>, IHasDomainEvents where T : class
{
    private readonly List<IDomainEvent> _domainEvents = new();

    protected Aggregate(string id) : base(id) { }

    public IEnumerable<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    protected void RaiseDomainEvent(IDomainEvent eventItem)
    {
        _domainEvents.Add(eventItem);
    }
}