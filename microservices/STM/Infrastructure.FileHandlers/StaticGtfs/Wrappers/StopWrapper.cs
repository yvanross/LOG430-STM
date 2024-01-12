using Application.Mapping.Interfaces.Wrappers;

namespace Infrastructure.FileHandlers.StaticGtfs.Wrappers;

public sealed record StopWrapper(string Id, double Latitude, double Longitude) : IStopWrapper;