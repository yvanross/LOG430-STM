using Application.Common.AntiCorruption;
using MediatR;

namespace Aspect.Configuration.AntiCorruption.Mediatr;

public class CommandWrapper<TCommand> : IRequest where TCommand : ICommand
{
    public TCommand Command { get; }

    public CommandWrapper(TCommand command)
    {
        Command = command;
    }
}