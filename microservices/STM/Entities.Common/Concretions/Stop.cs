using Entities.Common.Interfaces;

namespace Entities.Common.Concretions;

public class Stop : IStop
{
    public string Id { get; init; }

    public IPosition Position { get; init; }

    public string Message { get; set; } = string.Empty;
}