using ApplicationLogic.BusinessObjects;

namespace ApplicationLogic.Interfaces;

public interface ITrackBus
{
    void SetBus(IBus bus);

    BusTracking GetUpdate();
}