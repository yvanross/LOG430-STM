namespace ApplicationLogic.Interfaces;

public interface INodeControllerClient
{
    Task BeginExperiment(string hostAddressAndPort, string experimentDto);
}