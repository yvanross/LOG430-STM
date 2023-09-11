namespace Infrastructure.Events;

public class EventDto
{
    public EventDto(string EventType, string Event)
    {
        this.EventType = EventType;
        this.Event = Event;
    }

    public string EventType { get; set; }

    public string Event { get; set; }
}