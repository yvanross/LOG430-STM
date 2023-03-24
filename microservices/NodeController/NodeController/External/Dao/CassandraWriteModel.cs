using ApplicationLogic.Interfaces.Dao;
using Cassandra.Mapping;
using Cassandra;
using Entities.BusinessObjects.Live;
using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.ResourceManagement;
using System.Drawing;

namespace NodeController.External.Repository;

public class CassandraWriteModel : ISystemStateWriteModel
{
    private static readonly string ContactPoint = "127.0.0.1";

    private static readonly string Keyspace = "your_keyspace_name";

    static CassandraWriteModel()
    {
        _ = CreateUserDefinedTypes();
    }

    private static async Task CreateUserDefinedTypes()
    {
        var cluster = Cluster.Builder()
            .AddContactPoint(ContactPoint)
            .Build();

        using var session = await cluster.ConnectAsync();

        await session.UserDefinedTypes.DefineAsync(UdtMap.For<ExperimentDto>("experiment"));
    }

    public async Task Log(IExperimentReport experimentReport)
    {
        var dto = ExperimentDto.TryConvertToDto(experimentReport);

        using var cluster = Cluster.Builder()
            .AddContactPoint(ContactPoint)
            .Build();

        using var session = await cluster.ConnectAsync();

        var insertQuery = "INSERT INTO experiment VALUES (?)";
        var preparedStatement = await session.PrepareAsync(insertQuery);

        var boundStatement = preparedStatement.Bind(dto);

        await session.ExecuteAsync(boundStatement);
    }
}