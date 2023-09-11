using Application.CommandServices.Repositories;
using Domain.Common.Interfaces.Events;
using Microsoft.Extensions.Logging;

namespace Infrastructure.WriteRepositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppWriteDbContext _context;
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly ILogger<UnitOfWork> _logger;

    public UnitOfWork(AppWriteDbContext context, IDomainEventDispatcher eventDispatcher, ILogger<UnitOfWork> logger)
    {
        _context = context;
        _eventDispatcher = eventDispatcher;
        _logger = logger;
    }

    public async Task SaveChangesAsync()
    {
        if (_context.IsInMemory())
        {
            var aggregatesWithEvents = GetDomainEvents();

            await _context.SaveChangesAsync();

            await DispatchDomainEventsAsync(aggregatesWithEvents);
        }
        else
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var aggregatesWithEvents = GetDomainEvents();

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                // Optimal design would use an outbox. See https://microservices.io/patterns/data/transactional-outbox.html
                await DispatchDomainEventsAsync(aggregatesWithEvents);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error saving changes to database. Rolling back transaction.");

                await transaction.RollbackAsync();

                throw;
            }
        }
    }

    private List<IHasDomainEvents> GetDomainEvents()
    {
        return _context.ChangeTracker.Entries<IHasDomainEvents>()
            .Where(entry => entry.Entity.DomainEvents.Any())
            .Select(entry => entry.Entity)
            .ToList();
    }

    private async Task DispatchDomainEventsAsync(List<IHasDomainEvents> hasDomainEventsEnumerable)
    {
        var events = hasDomainEventsEnumerable
            .SelectMany(entitiesWithEvent => entitiesWithEvent.DomainEvents.ToList())
            .ToList();

        foreach (var @event in events) await _eventDispatcher.DispatchAsync(@event);

        hasDomainEventsEnumerable.ForEach(entity => entity.ClearDomainEvents());
    }
}