using Application.Commands.Seedwork;

namespace Aspect.Configuration.Dispatchers;

public class CommandDispatcher : ICommandDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public CommandDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellation)
    {
        var handler = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<ICommandHandler<TCommand>>();

        await handler.Handle(command, cancellation);
    }
}