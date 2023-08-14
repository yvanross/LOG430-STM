using Application.Common.AntiCorruption;
using Application.ViewModels;
using Domain.ValueObjects;

namespace Application.Queries;

public record struct GetEarliestBus(Position from, Position to) : IQuery<RideViewModel>;