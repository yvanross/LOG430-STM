using Application.Common.AntiCorruption;

namespace Application.Commands;

public record struct TrackBus(string ScheduledDepartureId, string ScheduledDestinationId, string BusId, string TripId) : ICommand;
