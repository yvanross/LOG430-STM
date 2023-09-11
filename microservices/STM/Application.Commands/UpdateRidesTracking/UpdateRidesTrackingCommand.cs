using Application.Commands.Seedwork;

namespace Application.Commands.UpdateRidesTracking;

public record UpdateRidesTrackingCommand : ICommand
{
    public string GetCommandName()
        => string.Empty; // Signifies to not logs it (called too often)
}