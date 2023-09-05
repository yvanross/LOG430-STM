using Application.Commands.Seedwork;

namespace Application.Commands.UpdateStaticGtfs;

public record UpdateStaticGtfsCommand : ICommand
{
    public string GetCommandName()
    => "Update Static GTFS";
}