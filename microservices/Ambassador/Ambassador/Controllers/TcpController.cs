using ServiceMeshHelper.Usecases;

namespace ServiceMeshHelper.Controllers;


public class TcpController
{
    private static readonly Tcp tcp = new();

    /// <summary>
    /// Allows managed connection over tcp to a service in the mesh using an integrated reverse proxy which is failover capable
    /// </summary>
    /// <param name="targetService">Name of the service type to connect to, ex: EventStream</param>
    /// <returns></returns>
    public static async Task<string> GetTcpSocketForSericeType(string targetService)
    {
        return await GetTcpSocket("rabbitmq", targetService);
    }

    /// <summary>
    /// Allows managed connection over tcp to a service in the mesh using an integrated reverse proxy which is failover capable
    /// </summary>
    /// <param name="protocol">Name of the communication protocol, ex: rabbitmq or redis or jdbc:postgresql</param>
    /// <param name="targetService">Name of the service type to connect to, ex: EventStream</param>
    /// <returns></returns>
    public static async Task<string> GetTcpSocket(string protocol, string targetService)
    {
        var port = await tcp.Preflight(targetService);

        return $"{protocol}://{Environment.GetEnvironmentVariable("SERVICES_ADDRESS")}:{port}";
    }
}