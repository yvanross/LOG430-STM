namespace Application.Common.AntiCorruption;

public interface ICommandHandler<in TCommand>
{
    Task Handle(TCommand command);
}