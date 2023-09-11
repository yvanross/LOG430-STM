namespace Application.EventHandlers;

public abstract class Event
{
    public Guid Id { get; init; }

    public DateTime Created { get; init; }

    //EF
    private Event() { }

    protected Event(Guid id, DateTime created)
    {
        Id = id;
        Created = created;
    }
}