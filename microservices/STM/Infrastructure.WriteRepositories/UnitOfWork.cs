using Application.CommandServices.ServiceInterfaces.Repositories;
using Domain.Events.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.WriteRepositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly DbContext _context;
    private readonly IDomainEventDispatcher _eventDispatcher;

    public UnitOfWork(DbContext context, IDomainEventDispatcher eventDispatcher)
    {
        _context = context;
        _eventDispatcher = eventDispatcher;
    }

    public async Task SaveChangesAsync()
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            await _context.SaveChangesAsync();

            //Optimal design would use an outbox. See https://microservices.io/patterns/data/transactional-outbox.html
            await DispatchDomainEventsAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
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
            var events = entity.DomainEvents.ToArray();

            foreach (var domainEvent in events)
            {
                await _eventDispatcher.DispatchAsync(domainEvent);
            }

            entity.ClearDomainEvents();
        }
    }
}