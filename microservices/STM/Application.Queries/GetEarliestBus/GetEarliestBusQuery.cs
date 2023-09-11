using Application.Queries.Seedwork;
using Domain.ValueObjects;

namespace Application.Queries.GetEarliestBus;

public record struct GetEarliestBusQuery(Position From, Position To) : IQuery
{
    public string GetQueryName()
    => "Get Earliest Bus";
}