﻿using Application.QueryServices.ServiceInterfaces.Repositories;
using Domain.Aggregates;

namespace Infrastructure.ReadRepositories;

public class StopReadRepository : ReadRepository<Stop>, IStopReadRepository
{
    public StopReadRepository(AppReadDbContext context) : base(context)
    {
    }
}