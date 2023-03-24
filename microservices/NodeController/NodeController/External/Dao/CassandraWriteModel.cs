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
    private readonly IPodReadModel = new PodReadModel();


    private async Task CreateUserDefinedTypes(IServiceInstance serviceInstance)
    {
        var dto = ServiceInstanceDto.TryConvertToDto(serviceInstance);

        if (dto is null) return;

        // Connect to the Cassandra cluster
        var cluster = Cluster.Builder()
            .AddContactPoint("localhost")
            .Build();

        // Get a session to interact with the keyspace
        var session = cluster.Connect("my_keyspace");

        // Define the UDT schema in C#
        var myObjectUdt = new UserDefinedType(
            "my_object",
            new[]
            {
                new UdtColumnInfo("property1", typeof(int)),
                new UdtColumnInfo("property2", typeof(string)),
                new UdtColumnInfo("property3", typeof(double))
            }
        );

        // Create the UDT in Cassandra
        var createUdtCql = new CreateTypeCql(myObjectUdt);
        session.Execute(createUdtCql);

        // Register the UDT mapping with the driver
        var config = new MappingConfiguration()
            .Define(UdtMap.For<MyObject>("my_object"));
        var mapper = new Mapper(session, config);
    }

    public async Task Log(IExperimentReport experimentReport)
    {
        // Connect to the Cassandra cluster
        var cluster = Cluster.Builder()
            .AddContactPoint("localhost")
            .Build();

        // Get a session to interact with the keyspace
        var session = cluster.Connect("my_keyspace");

        // Define your data
        var partitionKey = 1;
        var eventTime = DateTimeOffset.UtcNow;
        var objects = new List<UDTMap>
        {
            new UDTMap { { "property1", 42 }, { "property2", "Hello" }, { "property3", 3.14 } },
            new UDTMap { { "property1", 13 }, { "property2", "World" }, { "property3", 2.71 } }
        };
        var someInt = 123;

        // Prepare an INSERT statement
        var insertStatement = session.Prepare("INSERT INTO my_timeseries (partition_key, event_time, objects, some_int) VALUES (?, ?, ?, ?)");

        // Execute the INSERT statement with the data
        session.Execute(insertStatement.Bind(partitionKey, eventTime, objects, someInt));

        Console.WriteLine("Data appended to the time series");
    }
}