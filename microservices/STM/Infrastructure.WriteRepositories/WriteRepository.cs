using Application.CommandServices.Seedwork;
using Domain.Common.Seedwork.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.WriteRepositories;

public abstract class WriteRepository<TAggregate> : IWriteRepository<TAggregate> where TAggregate : Aggregate<TAggregate>, IEquatable<Entity<TAggregate>>
{
    private readonly ILogger _logger;

    protected DbSet<TAggregate> Aggregates { get; set; }

    protected WriteRepository(AppWriteDbContext writeDbContext, ILogger logger)
    {
        _logger = logger;

        Aggregates = writeDbContext.Set<TAggregate>();
    }

    public virtual async Task<IEnumerable<TAggregate>> GetAllAsync()
    {
        return await Aggregates.ToListAsync();
    }

    public virtual async Task<TAggregate> GetAsync(string id)
    {
        return await Aggregates.FindAsync(id) ?? throw new KeyNotFoundException($"Aggregate of type {typeof(TAggregate)} could not be found using id: {id}");
    }

    public async Task<bool> Exists(string id)
    {
        return await Aggregates.AnyAsync(x => x.Id == id);
    }

    public virtual async Task AddAsync(TAggregate aggregate)
    {
        var persistedAggregate = await Aggregates.FindAsync(aggregate.Id);

        if (persistedAggregate == null)
        {
            Aggregates.Add(aggregate);
        }
        else
        {
            var entry = Aggregates.Entry(persistedAggregate);

            entry.State = EntityState.Detached;

            Aggregates.Entry(aggregate).State = EntityState.Modified;

            Aggregates.Update(aggregate);
        }
    }

    public virtual async Task AddAllAsync(IEnumerable<TAggregate> aggregates)
    {
        await Aggregates.AddRangeAsync(aggregates);
    }
}