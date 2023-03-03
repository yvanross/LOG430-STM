namespace STM.Use_Cases;

public abstract class ChaosDaemon
{
    public static bool ChaosEnabled { get; set; } = false;

    protected void IsChaosEnabled()
    {
        if (ChaosEnabled) throw new Exception("Im not responding, you can't see this... shhhh");
    }
}