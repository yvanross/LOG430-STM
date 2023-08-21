using Application.CommandServices.ServiceInterfaces.Repositories;
using Domain.Events.Interfaces;

namespace Infrastructure.WriteRepositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly AppWriteDbContext _context;

    public UnitOfWork(AppWriteDbContext context, IDomainEventDispatcher eventDispatcher)
    {
        _context = context;
        _eventDispatcher = eventDispatcher;
    }

    public async Task SaveChangesAsync()
    {
        if (_context.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                await _context.SaveChangesAsync();

                // Optimal design would use an outbox. See https://microservices.io/patterns/data/transactional-outbox.html
                await DispatchDomainEventsAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        else
        {
            await _context.SaveChangesAsync();
            
            await DispatchDomainEventsAsync();
        }
    }

    private async Task DispatchDomainEventsAsync()
    {
        var entitiesWithEvents = _context.ChangeTracker.Entries<IHasDomainEvents>()
            .Where(entry => entry.Entity.DomainEvents.Any())
            .Select(entry => entry.Entity)
            .ToArray();

        foreach (var entity in entitiesWithEvents)
        {
            var events = entity.DomainEvents.ToList();

            var eventsByTypes = events
                .GroupBy(item => item.GetType())
                .Select(group => new { Type = group.Key, Items = group.ToList() });

            var mergedEvents = eventsByTypes
                .SelectMany(eventsByType => eventsByType.Items
                    .First().GetEventReduceBehavior().Invoke(eventsByType.Items))
                .ToList();

            foreach (var domainEvent in mergedEvents)
            {
                await _eventDispatcher.DispatchAsync(domainEvent);
            }

            entity.ClearDomainEvents();
        }
    }
}