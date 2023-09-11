using Application.Commands.Seedwork;

namespace Application.Commands.UpdateTrips;

public record UpdateTripsCommand : ICommand
{
    public string GetCommandName()
    => "Update Trips";
}