namespace Application.Commands.AntiCorruption;

public interface ICommandHandler<in TCommand>
{
    Task Handle(TCommand command, CancellationToken cancellation);
}