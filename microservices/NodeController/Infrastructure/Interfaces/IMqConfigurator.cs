using MassTransit;

namespace Infrastructure.Interfaces;

public interface IMqConfigurator
{
    IBusControl GetPublishEndpoint();
    
    Task Configure();
}