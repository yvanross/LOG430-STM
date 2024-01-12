using Application.Common.Interfaces;
using Infrastructure.FileHandlers.StaticGtfs.Enum;

namespace Configuration;

public class HostInfo : IHostInfo
{
    private const string ApiKeyEnvVariable = "API_KEY";

    private const string MemoryConsumptionEnvVariable = "MEMORY_CONSUMPTION";

    private static readonly string ServiceAddress = Environment.GetEnvironmentVariable("SERVICES_ADDRESS")!;

    private static readonly string StmApiKey = Environment.GetEnvironmentVariable(ApiKeyEnvVariable) ?? 
                                               throw new ArgumentNullException(ApiKeyEnvVariable, 
                                                   "The api key was not defined in the env variables, this is critical");

    private static readonly string MemoryConsumption = Environment.GetEnvironmentVariable(MemoryConsumptionEnvVariable)!;

    public string GetAddress()
    {
        return ServiceAddress;
    }

    public string GetStmApiKey()
    {
        return StmApiKey;
    }

    public MemoryConsumptionEnum GetMemoryConsumption()
    {
        return MemoryConsumption switch
        {
            "LOW" => MemoryConsumptionEnum.LOW,
            "MEDIUM" => MemoryConsumptionEnum.MEDIUM,
            "HIGH" => MemoryConsumptionEnum.HIGH,
            _ => throw new ArgumentOutOfRangeException(nameof(MemoryConsumption), MemoryConsumption, null)
        };
    }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(StmApiKey))
            throw new ArgumentNullException(ApiKeyEnvVariable,
                "The api key was not defined in the env variables, this is critical");

        if (string.IsNullOrWhiteSpace(MemoryConsumption))
            throw new ArgumentNullException(MemoryConsumptionEnvVariable,
                               "The memory consumption was not defined in the env variables, this is critical");
    }
}