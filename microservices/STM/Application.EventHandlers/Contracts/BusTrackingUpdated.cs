// MassTransit URN type resolutions, namespaces must be equal between projects for a shared type 
// ReSharper disable once CheckNamespace
namespace Contracts
{
    public class BusTrackingUpdated
    {
        public string Message { get; set; }

        public bool TrackingCompleted { get; set; }

        public double Duration { get; set; }

        public BusTrackingUpdated(string message, bool trackingCompleted, double duration)
        {
            Message = message;
            TrackingCompleted = trackingCompleted;
            Duration = duration;
        }
    }
   
}