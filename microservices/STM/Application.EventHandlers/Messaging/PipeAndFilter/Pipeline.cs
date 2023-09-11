using System.Threading.Channels;
using Application.Common.Extensions;
using Microsoft.Extensions.Logging;

namespace Application.EventHandlers.Messaging.PipeAndFilter;

public class Pipeline<TEvent, TResult>
{
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly List<Funnel> _funnels;

    private readonly ChannelReader<dynamic> _incomingChannel;
    private readonly ILogger _logger;

    public Pipeline(IEnumerable<Funnel> funnels, ChannelReader<dynamic> incomingChannel,
        CancellationTokenSource cancellationTokenSource, ILogger logger)
    {
        _funnels = funnels.ToList();
        _incomingChannel = incomingChannel;
        _cancellationTokenSource = cancellationTokenSource;
        _logger = logger;

        if (_funnels.Any() && _funnels.First().InputType != typeof(TEvent) &&
            _funnels.Last().OutputType != typeof(TResult))
            throw new InvalidOperationException(
                $"InputType in first Funnel must be {typeof(TEvent)} and OutputType must be {typeof(TResult)}");
    }

    public ChannelReader<object> Process()
    {
        if (_funnels.IsEmpty())
        {
            var channel = Channel.CreateUnbounded<object>();

            //will complete with cancellationToken
            _ = HandleEmptySinks(channel.Writer);

            return channel.Reader;
        }

        var lastSinkReader = LayoutPipeline();

        return lastSinkReader;
    }

    private async Task HandleEmptySinks(ChannelWriter<object> channelWriter)
    {
        await foreach (var @object in _incomingChannel.ReadAllAsync(_cancellationTokenSource.Token))
            await channelWriter.WriteAsync(@object, _cancellationTokenSource.Token);
    }

    private ChannelReader<object> LayoutPipeline()
    {
        var pipes = new List<Pipe>();

        ChannelReader<object> previousPipeReader = null!;

        Pipe pipe = new FirstPipe(_incomingChannel, typeof(TEvent), _funnels.First(), _cancellationTokenSource.Token);

        previousPipeReader = pipe.GetPipeReader();

        pipes.Add(pipe);

        for (var i = 0; i < _funnels.Count; i++)
        {
            var previousFunnel = _funnels[i];

            pipe = i.Equals(_funnels.Count - 1)
                ? new LastPipe(previousPipeReader, previousFunnel, typeof(TResult), _cancellationTokenSource.Token)
                : new MiddlePipe(previousPipeReader, previousFunnel, _cancellationTokenSource.Token);

            previousPipeReader = pipe.GetPipeReader();

            pipes.Add(pipe);
        }

        Task.WhenAny(pipes.Select(p => p.WireAndBeginProcessing()).ToArray())
            .ContinueWith(task =>
            {
                try
                {
                    CheckForException(task);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error in Pipeline");
                }
                finally
                {
                    _cancellationTokenSource.Cancel();
                }
            });

        return previousPipeReader;
    }

    private static void CheckForException(Task task)
    {
        if (task.Status == TaskStatus.Faulted) throw task.Exception.InnerException;

        if (task is Task<Task> nestedTaskContainer)
        {
            var nestedTask = nestedTaskContainer.Unwrap();

            CheckForException(nestedTask);
        }
    }
}