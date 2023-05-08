using ApplicationLogic.Interfaces;
using Entities.BusinessObjects;
using Entities.BusinessObjects.States;
using Entities.DomainInterfaces;
using Infrastructure.Repository;
using Microsoft.Extensions.Logging;
using NuGet.Protocol.Core.Types;
using Polly;
using Monitor = ApplicationLogic.Usecases.Monitor;

namespace Configuration;

public class SchedulingTask
{
    private readonly ILogger<SchedulingTask> _logger;
    private readonly Monitor _monitor;
    private readonly IScheduler _scheduler;
    private readonly IRepositoryWrite _repository;
    private readonly IAuthorizationService _authorizationService;

    public SchedulingTask(ILogger<SchedulingTask> logger, Monitor monitor, IScheduler scheduler, IRepositoryWrite repository, IAuthorizationService authorizationService)
    {
        _logger = logger;
        _monitor = monitor;
        _scheduler = scheduler;
        _repository = repository;
        _authorizationService = authorizationService;
    }

    public async Task ScheduleRecurringTasks()
    {
        _logger.LogInformation("Scheduling Heartbeat processing");

        try
        {
            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(1));

            _ = retryPolicy.ExecuteAsync(async () =>
            {
                var accountNames = await _authorizationService.GetVisibleAccounts(string.Empty);

                foreach (var accountName in accountNames)
                {
                    _repository.AddOrUpdateNode(new Node()
                    {
                        Name = accountName,
                        ServiceStatus = new UnresponsiveState(),
                        Version = "No Data"
                    });
                }

                _logger.LogInformation($"{accountNames.Length} Users were found in db, ack as Unresponsive");
            });
        }
        finally
        {
            _scheduler.TryAddTask(_monitor.BeginProcessingHeartbeats);

            _logger.LogInformation("Scheduling Heartbeat processing");
        }
    }
}