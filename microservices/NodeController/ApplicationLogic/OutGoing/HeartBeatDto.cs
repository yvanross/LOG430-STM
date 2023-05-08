
// MassTransit URN type resolutions, namespaces must be equal between project for a shared type 
// ReSharper disable once CheckNamespace
namespace MqContracts;

public class HeartBeatDto
{
    public string Source { get; set; }

    public string Version { get; set; }
    
    public bool Secure { get; set; }
    
    public bool Dirty { get; set; }
}