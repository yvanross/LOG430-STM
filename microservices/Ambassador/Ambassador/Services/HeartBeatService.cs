using Ambassador.BusinessObjects.InterServiceRequests;
using Ambassador.Extensions;
using Ambassador.Usecases;

namespace Ambassador.Health;

internal class HeartBeatService
{
    private static readonly PeriodicTimer PeriodicTimer = new(TimeSpan.FromSeconds(1));

    private static Task? _clock ;

    private Func<Task>? _sendHeartbeat;

    internal void TryBeginSendingHeartbeats(Func<Task> sendHeartbeat)
    {
        if (_clock is null)
        {
            _sendHeartbeat = sendHeartbeat;

            _clock = BeginSendingHeartbeats();
        }
    }

    internal async Task BeginSendingHeartbeats()
    {
        while (await PeriodicTimer.WaitForNextTickAsync())
        {
            await Try.WithConsequenceAsync(async () =>
            {
                await _sendHeartbeat!();

                return Task.FromResult(0);
            }, retryCount: int.MaxValue);
        }
    }
}