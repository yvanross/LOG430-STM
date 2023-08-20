namespace Application.CommandServices.ServiceInterfaces;

internal interface IScopedProcessor
{
    public Task ProcessUpdates();
}
