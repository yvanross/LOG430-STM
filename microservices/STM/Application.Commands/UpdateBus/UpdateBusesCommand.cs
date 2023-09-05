using Application.Commands.Seedwork;

namespace Application.Commands.UpdateBus;

public record UpdateBusesCommand : ICommand
{
    public string GetCommandName()
    => "Update Buses";
}