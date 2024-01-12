using Application.Commands.Seedwork;

namespace Application.Commands.Cleaning;

public sealed record ClearDb() : ICommand
{
    public string GetCommandName()
    {
        return "Clear Db";
    }
}