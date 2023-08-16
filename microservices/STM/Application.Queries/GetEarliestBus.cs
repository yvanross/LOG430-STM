using Application.Queries.AntiCorruption;
using Application.ViewModels;
using Domain.ValueObjects;

namespace Application.Queries;

public record struct GetEarliestBus(Position From, Position To) : IQuery<RideViewModel>;