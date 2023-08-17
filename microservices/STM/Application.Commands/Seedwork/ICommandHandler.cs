namespace Application.Commands.Seedwork;

public interface ICommandHandler<in TCommand>
{
    Task Handle(TCommand command, CancellationToken cancellation);
}