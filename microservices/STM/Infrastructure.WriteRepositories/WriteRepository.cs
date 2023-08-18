using Application.CommandServices.Seedwork;
using Domain.Common.Seedwork.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.WriteRepositories;

public abstract class WriteRepository<TAggregate> : DbContext, IWriteRepository<TAggregate> where TAggregate : Aggregate<TAggregate>, IEquatable<Entity<TAggregate>>
{
    private readonly ILogger _logger;

    protected DbSet<TAggregate> Aggregates { get; init; }

    protected WriteRepository(DbContextOptions<WriteRepository<TAggregate>> options, ILogger logger) : base(options)
    {
        _logger = logger;
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
            Aggregates.Update(aggregate);
        }
    }

    public async Task AddAllAsync(IEnumerable<TAggregate> aggregates)
    {
        var unsavedAggregates = await Aggregates.Except(aggregates).ToListAsync();

        var savedAggregates = aggregates.Except(unsavedAggregates).ToList();

        foreach (var aggregate in unsavedAggregates)
        {
            Aggregates.Add(aggregate);
        }

        foreach (var aggregate in savedAggregates)
        {
            Aggregates.Update(aggregate);
        }
    }
}