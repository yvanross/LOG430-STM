using System.Threading.Channels;

namespace Application.EventHandlers.Messaging.PipeAndFilter;

public sealed class LastPipe : Pipe
{
    private readonly Type _outGoingType;
    private readonly Channel<object> _outgoingTypeCheck;
    private readonly Funnel _previousFunnel;

    public LastPipe(
        ChannelReader<object> incomingChannel,
        Funnel previousFunnel,
        Type outGoingType,
        CancellationToken token) : base(incomingChannel, token)
    {
        _previousFunnel = previousFunnel;
        _outGoingType = outGoingType;

        _outgoingTypeCheck = Channel.CreateUnbounded<object>();
    }

    public override ChannelReader<object> GetPipeReader()
    {
        return _outgoingTypeCheck.Reader;
    }

    public override Task WireAndBeginProcessing()
    {
        var interceptionTask = IncomingInterception();

        var processingTask = BeginProcessing();

        return Task.WhenAny(interceptionTask, processingTask);
    }

    private protected override Task IncomingInterception()
    {
        var incomingInterceptionTask = base.IncomingInterception();

        var outgoingInterceptionTask = OutgoingInterception();

        return Task.WhenAny(incomingInterceptionTask, outgoingInterceptionTask);
    }

    private async Task OutgoingInterception()
    {
        await foreach (var @object in OutgoingChannel.Reader.ReadAllAsync(Token))
        {
            OutgoingMessageTypeCheck(@object);

            await _outgoingTypeCheck.Writer.WriteAsync(@object, Token);
        }
    }

    private Task BeginProcessing()
    {
        if (OutgoingChannel is null)
            throw new InvalidOperationException("Call Wire first, _outgoingChannel is null");

        //Long running task
        var task = _previousFunnel.Act(PipeChannel.Reader, OutgoingChannel.Writer, Token);

        return task;
    }

    private protected override void IncomingMessageTypeCheck(object message)
    {
        var inputType = _previousFunnel?.InputType;

        var messageType = message.GetType();

        if (messageType != inputType)
            throw new InvalidOperationException($"Expected type {inputType} but got {messageType} in pipeline");
    }

    private void OutgoingMessageTypeCheck(object message)
    {
        var outPutType = _outGoingType;

        var messageType = message.GetType();

        if (messageType != outPutType)
            throw new InvalidOperationException($"Expected type {outPutType} and {messageType} to match in pipeline");
    }
}