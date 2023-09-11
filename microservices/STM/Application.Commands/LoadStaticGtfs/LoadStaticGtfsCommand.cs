using Application.Commands.Seedwork;

namespace Application.Commands.LoadStaticGtfs;

public record LoadStaticGtfsCommand : ICommand
{
    public string GetCommandName()
    => "Load Static GTFS";
}