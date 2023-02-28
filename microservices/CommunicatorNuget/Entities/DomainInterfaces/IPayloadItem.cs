namespace CommunicatorNuget.Interfaces;

public interface IPayloadItem
{
    string paramName { get; set; }

    string value { get; set; }
}