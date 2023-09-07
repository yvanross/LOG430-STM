namespace Application.EventHandlers.Interfaces;

public interface IApplicationEventHandler<in TApplicationEvent> where TApplicationEvent : Event
{
    Task HandleAsync(TApplicationEvent applicationEvent);
}