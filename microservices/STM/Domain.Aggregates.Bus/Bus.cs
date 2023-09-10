using Domain.Common.Exceptions;
using Domain.Common.Interfaces;
using Domain.Common.Seedwork.Abstract;

namespace Domain.Aggregates.Bus;

public class Bus : Aggregate<Bus>
{
    // EF Core
    private Bus(string id) : base(id) {}

    internal Bus(string id, string name, string tripId, int currentStopIndex, IDatetimeProvider datetimeProvider) : base(id)
    {
        SetBusName(name, datetimeProvider);
        SetTripId(tripId);
        UpdateCurrentStopIndex(currentStopIndex, datetimeProvider);
    }

    public string Name { get; private set; } = default!;

    public string TripId { get; private set; } = default!;

    public int CurrentStopIndex { get; private set; }

    public DateTime LastUpdateTime { get; private set; }

    public void UpdateCurrentStopIndex(int currentStopIndex, IDatetimeProvider datetimeProvider)
    {
        if (CurrentStopIndex < 0)
        {
            throw new AggregateInvalideStateException("Current stop index was less than 0, aggregate is in an invalid state");
        }

        if (currentStopIndex > CurrentStopIndex)
        {
            CurrentStopIndex = currentStopIndex;
        }

        SetBusUpdatedTime(datetimeProvider);
    }

    private void SetTripId(string tripId)
    {
        if (string.IsNullOrWhiteSpace(tripId))
        {
            throw new ArgumentException("TripId cannot be null or whitespace", nameof(tripId));
        }

        TripId = tripId;
    }

    private void SetBusName(string name, IDatetimeProvider datetimeProvider)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be null or whitespace", nameof(name));
        }

        Name = name;
    }

    private void SetBusUpdatedTime(IDatetimeProvider datetimeProvider)
    {
        LastUpdateTime = datetimeProvider.GetCurrentTime();
    }

}