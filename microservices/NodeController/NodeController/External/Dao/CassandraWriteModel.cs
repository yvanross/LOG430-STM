using ApplicationLogic.Interfaces.Dao;
using Cassandra;
using Entities.DomainInterfaces.ResourceManagement;
using ApplicationLogic.Usecases;
using NodeController.External.Docker;
using NodeController.External.Ingress;
using NodeController.Dto;

namespace NodeController.External.Dao;

public class CassandraWriteModel : ISystemStateWriteModel
{
    private static string _contactPoint = null!;

    private static readonly string KeySpace = HostInfo.TeamName;

    public async Task Log(IExperimentReport experimentReport)
    {
        if (string.IsNullOrEmpty(_contactPoint))
        {
            await new IngressUC(new HostInfo(), new IngressClient()).GetLogStoreAddressAndPort().ContinueWith(r => _contactPoint = r.Result);

            await CreateUserDefinedTypes();
        }

        var dto = ExperimentDto.TryConvertToDto(experimentReport);

        using var cluster = Cluster.Builder()
            .AddContactPoint(_contactPoint)
            .WithDefaultKeyspace(KeySpace)
            .Build();

        using var session = await cluster.ConnectAsync();

        var insertQuery = "INSERT INTO experiment VALUES (?)";
        var preparedStatement = await session.PrepareAsync(insertQuery);

        var boundStatement = preparedStatement.Bind(dto);

        await session.ExecuteAsync(boundStatement);
    }

    private static async Task CreateUserDefinedTypes()
    {
        var cluster = Cluster.Builder()
            .AddContactPoint(_contactPoint)
            .WithDefaultKeyspace(KeySpace)
            .Build();

        using var session = await cluster.ConnectAsync();

        await session.UserDefinedTypes.DefineAsync(UdtMap.For<ExperimentDto>("experiment"));
    }
}