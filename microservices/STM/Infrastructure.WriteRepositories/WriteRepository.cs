using Application.CommandServices.Seedwork;
using Domain.Common.Seedwork.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.WriteRepositories;

public abstract class WriteRepository<TAggregate> : DbContext, IWriteRepository<TAggregate> where TAggregate : Aggregate<TAggregate>
{
    private readonly ILogger _logger;

    protected DbSet<TAggregate> Aggregates { get; init; }

    protected WriteRepository(DbContextOptions options, ILogger logger) : base(options)
    {
        _logger = logger;
    }

    public async Task<IEnumerable<TAggregate>> GetAllAsync()
    {
        return await Aggregates.ToListAsync();
    }

    public async Task<TAggregate> GetAsync(string id)
    {
        return await Aggregates.FindAsync(id) ?? throw new KeyNotFoundException();
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
}