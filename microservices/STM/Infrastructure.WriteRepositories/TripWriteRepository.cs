using Application.CommandServices.Repositories;
using Application.Common.Extensions;
using Application.Dtos;
using Application.Mapping.Interfaces;
using Domain.Aggregates.Trip;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.WriteRepositories;

public class TripWriteRepository : ITripWriteRepository
{
    private readonly AppWriteDbContext _writeDbContext;
    private readonly IMappingTo<IEnumerable<ScheduledStopDto>, Trip> _scheduledStopDtoToTripMapper;
    private readonly IMappingTo<Trip, IEnumerable<ScheduledStopDto>> _tripToScheduledStopDtoMapper;
    private readonly DbContextOptions<AppWriteDbContext> _dbContextOptions;
    private readonly ILogger<TripWriteRepository> _logger;

    private readonly DbSet<ScheduledStopDto> _scheduledStopDtos;

    public TripWriteRepository(
        AppWriteDbContext writeDbContext,
        IMappingTo<IEnumerable<ScheduledStopDto>, Trip> scheduledStopDtoToTripMapper,
        IMappingTo<Trip, IEnumerable<ScheduledStopDto>> tripToScheduledStopDtoMapper,
        DbContextOptions<AppWriteDbContext> dbContextOptions,
        ILogger<TripWriteRepository> logger)
    {
        _writeDbContext = writeDbContext;
        _scheduledStopDtoToTripMapper = scheduledStopDtoToTripMapper;
        _tripToScheduledStopDtoMapper = tripToScheduledStopDtoMapper;
        _dbContextOptions = dbContextOptions;
        _logger = logger;
        _scheduledStopDtos = _writeDbContext.Set<ScheduledStopDto>();
    }

    public async Task AddOrUpdateAsync(Trip aggregate)
    {
        await _writeDbContext.AddAsync(_tripToScheduledStopDtoMapper.MapFrom(aggregate));
    }

    public async Task<Trip> GetAsync(string id)
    {
        var scheduledStopDtos = await _scheduledStopDtos
            .AsNoTracking()
            .Where(scheduledStopDto => scheduledStopDto.TripId.Equals(id))
            .ToListAsync()
           ??
           throw new KeyNotFoundException($"Aggregate of type {typeof(Trip)} could not be found using id: {id}");

        var trip = _scheduledStopDtoToTripMapper.MapFrom(scheduledStopDtos);

        return trip;
    }

    public async Task<IEnumerable<Trip>> GetAllAsync(params string[] ids)
    {
        List<IGrouping<string, ScheduledStopDto>> scheduledStopDtos;

        if (ids.IsEmpty())
        {
            _logger.LogInformation("Good luck on getting 5M+ entities from the db, filter it first by id");

            scheduledStopDtos = await _scheduledStopDtos
                .AsNoTracking()
                .GroupBy(scheduledStopDto => scheduledStopDto.TripId)
                .ToListAsync();
        }
        else
        {
             scheduledStopDtos = await _scheduledStopDtos
                .AsNoTracking()
                .Where(scheduledStopDto => ids.Contains(scheduledStopDto.TripId))
                .GroupBy(scheduledStopDto => scheduledStopDto.TripId)
                .ToListAsync();
        }

        var trips = scheduledStopDtos.Select(_scheduledStopDtoToTripMapper.MapFrom).ToList();

        return trips;
    }

    public async Task AddAllAsync(IEnumerable<Trip> aggregates)
    {
        var scheduledStopDtos = aggregates.SelectMany(_tripToScheduledStopDtoMapper.MapFrom).ToList();

        if (_writeDbContext.IsInMemory())
            await _scheduledStopDtos.AddRangeAsync(scheduledStopDtos);
        else
        {
            // Create a new DbContext instance for this operation
            await using var context = new AppWriteDbContext(_dbContextOptions);

            await context.BulkInsertAsync(scheduledStopDtos);
        }
    }

    public void Remove(Trip ride)
    {
        throw new NotImplementedException();
    }

    public async Task ClearAsync()
    {
        if (_writeDbContext.IsInMemory() is false)
            await _writeDbContext.Database.ExecuteSqlRawAsync(
                $"""
                DELETE FROM public."{_scheduledStopDtos.EntityType.GetTableName()}"
                """);
    }
}