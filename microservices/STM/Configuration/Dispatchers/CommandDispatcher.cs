using Application.Commands.Seedwork;

namespace Configuration.Dispatchers;

public class CommandDispatcher : ICommandDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CommandDispatcher> _logger;

    public CommandDispatcher(IServiceProvider serviceProvider, ILogger<CommandDispatcher> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellation) where TCommand : ICommand
    {
        var handler = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<ICommandHandler<TCommand>>();

        if (command.GetCommandName() is {} commandName && string.IsNullOrWhiteSpace(commandName) is false)
        {
            _logger.LogInformation($"Dispatching command '{command.GetType().Name}'");
        }

        await handler.Handle(command, cancellation);
    }
}