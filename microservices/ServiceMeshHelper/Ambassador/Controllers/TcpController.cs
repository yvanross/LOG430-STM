using Polly;
using ServiceMeshHelper.Services;
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
    public static async Task<string> GetTcpSocketForRabbitMq(string targetService)
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
        var backOffRetry = Policy.Handle<Exception>()
            .WaitAndRetryForeverAsync(attempt => TimeSpan.FromSeconds(Math.Max(attempt/2, 5)), 
                (exception, span) => Console.WriteLine(exception));

        var port = await backOffRetry.ExecuteAsync(() => tcp.Preflight(targetService));

        return $"{protocol}://{ServiceMeshConfiguration.ServicesAddress}:{port}";
    }
}