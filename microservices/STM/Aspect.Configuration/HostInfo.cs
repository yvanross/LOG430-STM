using Application.Common.Interfaces;

namespace Aspect.Configuration;

public class HostInfo : IHostInfo
{
    private static readonly string ServiceAddress = Environment.GetEnvironmentVariable("SERVICES_ADDRESS")!;

    private static readonly string StmApiKey = Environment.GetEnvironmentVariable("API_KEY") ?? 
                                               throw new ArgumentNullException("API_KEY", 
                                                   "The api key was not defined in the env variables, this is critical");

    public string GetAddress()
    {
        return ServiceAddress;
    }

    public string GetStmApiKey()
    {
        return StmApiKey;
    }
}