namespace Application.Commands.AntiCorruption;

public interface ICommandDispatcher
{
    Task Dispatch<TCommand>(TCommand command, CancellationToken cancellation);
}