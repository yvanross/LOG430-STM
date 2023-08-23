using System.Threading.Channels;

namespace Application.EventHandlers.Messaging.PipeAndFilter;

public record Funnel(
    Func<ChannelReader<dynamic>, ChannelWriter<dynamic>, CancellationToken, Task> Act,
    Type InputType,
    Type OutputType);