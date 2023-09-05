using Application.Queries.Seedwork;
using Application.ViewModels;
using Domain.ValueObjects;

namespace Application.Queries.GetEarliestBus;

public record struct GetEarliestBusQuery(Position From, Position To) : IQuery<RideViewModel>
{
    public string GetQueryName()
    => "Get Earliest Bus";
}