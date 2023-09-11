namespace Domain.Aggregates.Ride;

public class RideUpdateInfo
{
    public RideUpdateInfo(int firstRecordedStopIndex, int currentStopIndex, int firstStopIndex, int targetStopIndex, string busName)
    {
        FirstRecordedStopIndex = firstRecordedStopIndex;
        CurrentStopIndex = currentStopIndex;
        FirstStopIndex = firstStopIndex;
        TargetStopIndex = targetStopIndex;
        BusName = busName;
    }

    public int FirstRecordedStopIndex { get; }
    public int CurrentStopIndex { get; }
    public int FirstStopIndex { get; }
    public int TargetStopIndex { get; }
    public string BusName { get; }

    public override string ToString()
    {
        return $"FirstRecordedStopIndex: {FirstRecordedStopIndex}, CurrentStopIndex: {CurrentStopIndex}, FirstStopIndex: {FirstStopIndex}, TargetStopIndex: {TargetStopIndex}, BusName: {BusName}";
    }
}