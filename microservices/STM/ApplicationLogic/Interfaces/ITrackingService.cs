using Entities.Domain;

namespace ApplicationLogic.Interfaces;

public interface ITrackingService
{
    (IBusTracking, ITrackingService?) GetUpdate();
}