using Application.Commands.Seedwork;

namespace Aspect.Configuration.Dispatchers;

public class CommandDispatcher : ICommandDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public CommandDispatcher(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    public async Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellation)
    {
       // using var scope = _serviceProvider.CreateScope();

       // var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<TCommand>>();

        var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand>>();

        await handler.Handle(command, cancellation);
    }
}