using Application.Common.AntiCorruption;
using MediatR;

namespace Aspect.Configuration.AntiCorruption.Mediatr;

public class CommandHandler<TCommand> : IRequestHandler<CommandWrapper<TCommand>> where TCommand : ICommand
{
    private readonly ICommandHandler<TCommand> _handler;

    public CommandHandler(ICommandHandler<TCommand> handler)
    {
        _handler = handler;
    }

    public Task Handle(CommandWrapper<TCommand> request, CancellationToken cancellationToken)
    {
        return _handler.Handle(request.Command);
    }
}