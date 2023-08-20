﻿using Application.QueryServices.Seedwork;
using Domain.Common.Seedwork.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.ReadRepositories;

public abstract class ReadRepository<TAggregate> : IReadRepository<TAggregate> where TAggregate : Aggregate<TAggregate>
{
    protected DbSet<TAggregate> Aggregates { get; init; }

    protected ReadRepository(AppReadDbContext context)
    {
        Aggregates = context.Set<TAggregate>();
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