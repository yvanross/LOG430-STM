using Application.Commands.Seedwork;

namespace Application.Commands.TrackBus;

public record struct TrackBusCommand(string ScheduledDepartureId, string ScheduledDestinationId, string BusId) : ICommand
{
    public string GetCommandName()
    => "Track Bus";
}