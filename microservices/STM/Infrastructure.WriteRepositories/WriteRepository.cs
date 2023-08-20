using Application.CommandServices.Seedwork;
using Domain.Common.Seedwork.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.WriteRepositories;

public abstract class WriteRepository<TAggregate> : IWriteRepository<TAggregate> where TAggregate : Aggregate<TAggregate>, IEquatable<Entity<TAggregate>>
{
    private readonly ILogger _logger;

    private protected DbSet<TAggregate> Aggregates { get; init; }

    protected WriteRepository(AppWriteDbContext writeDbContext, ILogger logger)
    {
        _logger = logger;
        Aggregates = writeDbContext.Set<TAggregate>();
    }

    public async Task<IEnumerable<TAggregate>> GetAllAsync()
    {
        return await Aggregates.ToListAsync();
    }

    public async Task<TAggregate> GetAsync(string id)
    {
        return await Aggregates.FindAsync(id) ?? throw new KeyNotFoundException($"Aggregate of type {typeof(TAggregate)} could not be found using id: {id}");
    }

    public async Task AddAsync(TAggregate aggregate)
    {
        var persistedAggregate = await Aggregates.FindAsync(aggregate.Id);

        if (persistedAggregate == null)
        {
            Aggregates.Add(aggregate);
        }
        else
        {
            Aggregates.Remove(persistedAggregate);
            Aggregates.Add(aggregate);
        }
    }

    public async Task AddAllAsync(IEnumerable<TAggregate> aggregates)
    {
        await Aggregates.AddRangeAsync(aggregates);
    }
}