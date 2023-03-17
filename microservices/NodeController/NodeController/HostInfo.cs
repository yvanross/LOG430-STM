namespace NodeController;

public static class HostInfo
{
    public static readonly string ServiceAddress = Environment.GetEnvironmentVariable("LocalDockerAddress")!;
}