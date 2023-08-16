using Application.QueryServices.Seedwork;
using Domain.Common.Seedwork.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.ReadRepositories;

public abstract class ReadRepository<TAggregate> : DbContext, IReadRepository<TAggregate> where TAggregate : Aggregate<TAggregate>
{
    private readonly ILogger _logger;

    protected DbSet<TAggregate> Aggregates { get; init; }

    protected ReadRepository(DbContextOptions options, ILogger logger) : base(options)
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
}