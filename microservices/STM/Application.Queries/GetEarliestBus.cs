using Application.Queries.Seedwork;
using Application.ViewModels;
using Domain.ValueObjects;

namespace Application.Queries;

public record struct GetEarliestBus(Position From, Position To) : IQuery<RideViewModel>;