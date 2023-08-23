using System.Threading.Channels;

namespace Application.EventHandlers.Messaging.PipeAndFilter;

public sealed class FirstPipe : Pipe
{
    private readonly Type _incomingType;
    private readonly Funnel _nextFunnel;

    public FirstPipe(
        ChannelReader<object> incomingChannel,
        Type incomingType,
        Funnel nextFunnel,
        CancellationToken token) : base(incomingChannel, token)
    {
        _incomingType = incomingType;
        _nextFunnel = nextFunnel;
    }

    public override ChannelReader<object> GetPipeReader()
    {
        return PipeChannel.Reader;
    }

    public override Task WireAndBeginProcessing()
    {
        return IncomingInterception();
    }

    private protected override void IncomingMessageTypeCheck(object message)
    {
        var inputType = _incomingType;

        var messageType = message.GetType();

        if (messageType != inputType)
            throw new InvalidOperationException($"Expected type {inputType} but got {messageType} in pipeline");
    }
}