﻿using Application.CommandServices.Repositories;
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
                var aggregatesWithEvents = GetDomainEvents();

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                // Optimal design would use an outbox. See https://microservices.io/patterns/data/transactional-outbox.html
                await DispatchDomainEventsAsync(aggregatesWithEvents);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        else
        {
            var aggregatesWithEvents = GetDomainEvents();

            await _context.SaveChangesAsync();
            
            await DispatchDomainEventsAsync(aggregatesWithEvents);
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

        foreach (var @event in events)
        {
            await _eventDispatcher.DispatchAsync(@event);
        }

        hasDomainEventsEnumerable.ForEach(entity => entity.ClearDomainEvents());
    }
}