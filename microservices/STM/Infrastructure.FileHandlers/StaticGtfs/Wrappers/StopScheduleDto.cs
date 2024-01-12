using Application.Mapping.Interfaces.Wrappers;

namespace Infrastructure.FileHandlers.StaticGtfs.Wrappers;

public sealed record StopScheduleWrapper(string StopId, TimeSpan DepartureTime, int StopSequence) : IStopScheduleWrapper;