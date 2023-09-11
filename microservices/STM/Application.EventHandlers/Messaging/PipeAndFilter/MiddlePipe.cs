using System.Threading.Channels;

namespace Application.EventHandlers.Messaging.PipeAndFilter;

public sealed class MiddlePipe : Pipe
{
    private readonly Funnel _previousFunnel;

    public MiddlePipe(
        ChannelReader<object> incomingChannel,
        Funnel previousFunnel,
        CancellationToken token) : base(incomingChannel, token)
    {
        _previousFunnel = previousFunnel;
    }

    public override Task WireAndBeginProcessing()
    {
        var interceptionTask = IncomingInterception();

        var processingTask = BeginProcessing();

        return Task.WhenAny(interceptionTask, processingTask);
    }

    private protected override void IncomingMessageTypeCheck(object message)
    {
        var inputType = _previousFunnel?.InputType;

        var messageType = message.GetType();

        if (messageType != inputType)
            throw new InvalidOperationException($"Expected type {inputType} but got {messageType} in pipeline");
    }

    private Task BeginProcessing()
    {
        if (OutgoingChannel is null)
            throw new InvalidOperationException("Call Wire first, _outgoingChannel is null");

        //Long running task
        var task = _previousFunnel.Act(PipeChannel.Reader, OutgoingChannel.Writer, Token)
            .ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    // Handle the exception here
                    var exception = t.Exception?.InnerException; // Access the original exception
                }
            });

        return task;
    }
}