using ServiceMeshHelper.Services;
using ServiceMeshHelper.Usecases;

namespace ServiceMeshHelper.Controllers;


/// <summary>
/// Controller exposing actions to manage tcp connections within the service mesh.
/// </summary>
public class TcpController
{
    private static readonly Tcp Tcp = new();

    /// <summary>
    /// Allows managed connection over tcp to a service in the mesh using an integrated reverse proxy which is failover capable
    /// Preflight retried 10 times before giving up with back-off.
    /// </summary>
    /// <param name="targetService">Name of the service type to connect to, ex: EventStream</param>
    /// <returns></returns>
    public static async Task<string> GetTcpSocketForRabbitMq(string targetService)
    {
        return await GetTcpSocket("rabbitmq", targetService);
    }

    /// <summary>
    /// Allows managed connection over tcp to a service in the mesh using an integrated reverse proxy which is failover capable
    /// Preflight retried 10 times before giving up with back-off.
    /// </summary>
    /// <param name="protocol">Name of the communication protocol, ex: rabbitmq or redis or jdbc:postgresql</param>
    /// <param name="targetService">Name of the service type to connect to, ex: EventStream</param>
    /// <returns></returns>
    public static async Task<string> GetTcpSocket(string protocol, string targetService)
    {
        var port = await Tcp.Preflight(targetService);

        return $"{protocol}://{ServiceMeshConfiguration.ServicesAddress}:{port}";
    }
}