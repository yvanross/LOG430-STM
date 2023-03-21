namespace Entities.DomainInterfaces.ResourceManagement;

public interface IChaosConfig
{
    /// <summary>
    /// CPU quota in units of 10^-9 CPUs.
    /// </summary>
    public long NanoCpus { get; set; }

    /// <summary>
    /// Memory limit in bytes.
    /// </summary>
    public long Memory { get; set; }

    /// <summary>
    /// Maximum Number of artifact at any given time
    /// </summary>
    public int MaxNumberOfPods { get; set; }
    
    /// <summary>
    /// Rate of artifact removal per minute
    /// </summary>
    public int KillRate { get; set; }
}