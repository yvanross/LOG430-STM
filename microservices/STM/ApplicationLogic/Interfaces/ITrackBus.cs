using Entities.Transit.Interfaces;

namespace ApplicationLogic.Use_Cases;

public interface ITrackBus
{
    void SetBus(IBus bus);
    IBusTracking? GetUpdate();
}