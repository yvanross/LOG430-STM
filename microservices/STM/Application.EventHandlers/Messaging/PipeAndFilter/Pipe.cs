using System.Threading.Channels;

namespace Application.EventHandlers.Messaging.PipeAndFilter;

public abstract class Pipe
{
    private readonly ChannelReader<object> _incomingChannel;

    private protected readonly Channel<object> OutgoingChannel;

    private protected readonly Channel<object> PipeChannel;

    private protected readonly CancellationToken Token;

    protected Pipe(
        ChannelReader<object> incomingChannel,
        CancellationToken token)
    {
        _incomingChannel = incomingChannel;
        Token = token;

        PipeChannel = Channel.CreateUnbounded<dynamic>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = true
        });

        OutgoingChannel = Channel.CreateUnbounded<object>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = true
        });
    }

    public virtual ChannelReader<object> GetPipeReader()
    {
        return OutgoingChannel.Reader;
    }

    public abstract Task WireAndBeginProcessing();

    private protected virtual async Task IncomingInterception()
    {
        await foreach (var @object in _incomingChannel.ReadAllAsync(Token))
        {
            IncomingMessageTypeCheck(@object);

            await PipeChannel.Writer.WriteAsync(@object, Token);
        }
    }

    private protected abstract void IncomingMessageTypeCheck(object message);
}