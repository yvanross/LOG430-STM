namespace Application.EventHandlers;

public interface IApplicationEventHandler<in TApplicationEvent> where TApplicationEvent : Event
{
    Task HandleAsync(TApplicationEvent applicationEvent);
}