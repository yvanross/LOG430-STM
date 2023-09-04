using Application.Commands.Seedwork;

namespace Application.Commands;

public record struct TrackBus(string ScheduledDepartureId, string ScheduledDestinationId, string BusId) : ICommand;