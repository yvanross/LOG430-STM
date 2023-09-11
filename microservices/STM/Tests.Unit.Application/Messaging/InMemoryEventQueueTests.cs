using Application.EventHandlers;
using Application.EventHandlers.Messaging;
using Application.EventHandlers.Messaging.PipeAndFilter;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Unit.Application.Messaging;

[TestClass]
public class InMemoryEventQueueTests
{
    private InMemoryEventQueue _queue;
    public TestContext TestContext { get; set; }

    [TestInitialize]
    public void Setup()
    {
        //service provider is used to dispatch application level events, not used here
        _queue = new InMemoryEventQueue(null, null);
    }

    [TestMethod]
    public async Task SubscribeTest()
    {
        const string baseName = "test ";
        const string newName = nameof(SubscribeTest);

        _queue.Subscribe<TestEvent, TestEvent>(async (result, token) =>
        {
            result.Name.Should().Be(baseName);

            result.GetType().Should().Be<TestEvent>();

            await _queue.Publish(new TestResult_SubscribeTest(Guid.NewGuid(), DateTime.UtcNow)
            {
                Name = newName
            });
        }, new TestLogger(TestContext));

        await _queue.Publish(new TestEvent(Guid.NewGuid(), DateTime.UtcNow)
        {
            Name = baseName
        });

        var result = await _queue.ConsumeNext<TestResult_SubscribeTest>();

        result.Name.Should().Be(newName);

        result.GetType().Should().Be<TestResult_SubscribeTest>();
    }

    [TestMethod]
    public async Task SubscribeTest_WithFunnel()
    {
        const string baseName = "test ";
        const string newName = nameof(SubscribeTest_WithFunnel);

        _queue.Subscribe<TestEvent, TestResult>(async (result, token) =>
            {
                result.Name.Should().Be(newName);

                result.GetType().Should().Be<TestResult>();

                await _queue.Publish(new TestResult_SubscribeTest_WithFunnel(Guid.NewGuid(), DateTime.UtcNow)
                {
                    Name = result.Name
                });
            }, new TestLogger(TestContext),
            new Funnel(async (reader, writer, cancellationToken) =>
            {
                await foreach (var @event in reader.ReadAllAsync(cancellationToken))
                {
                    var eventTransform = new TestResult(Guid.NewGuid(), DateTime.UtcNow)
                    {
                        Name = newName
                    };

                    await writer.WriteAsync(eventTransform, cancellationToken);
                }
            }, typeof(TestEvent), typeof(TestResult)));

        await _queue.Publish(new TestEvent(Guid.NewGuid(), DateTime.UtcNow)
        {
            Name = baseName
        });

        var result = await _queue.ConsumeNext<TestResult_SubscribeTest_WithFunnel>();

        result.Name.Should().Be(newName);

        result.GetType().Should().Be<TestResult_SubscribeTest_WithFunnel>();
    }

    [TestMethod]
    public async Task SubscribeTest_With_Long_Funnel()
    {
        const string baseName = "test ";
        const string newName = nameof(SubscribeTest_WithFunnel);

        _queue.Subscribe<TestEvent, TestResult_SubscribeTest_With_Long_Funnel>(async (result, token) =>
            {
                result.GetType().Should().Be<TestResult_SubscribeTest_With_Long_Funnel>();

                await _queue.Publish(result);
            }, new TestLogger(TestContext),
            new Funnel(async (reader, writer, cancellationToken) =>
            {
                await foreach (var @event in reader.ReadAllAsync(cancellationToken))
                {
                    var eventTransform = new TestResult(Guid.NewGuid(), DateTime.UtcNow)
                    {
                        Name = newName
                    };

                    await writer.WriteAsync(eventTransform, cancellationToken);
                }
            }, typeof(TestEvent), typeof(TestResult)),
            new Funnel(async (reader, writer, cancellationToken) =>
            {
                await foreach (var @event in reader.ReadAllAsync(cancellationToken))
                {
                    var eventTransform = new TestResult_SubscribeTest_With_Long_Funnel(Guid.NewGuid(), DateTime.UtcNow)
                    {
                        Name = baseName
                    };
                    await writer.WriteAsync(eventTransform, cancellationToken);
                }
            }, typeof(TestResult), typeof(TestResult_SubscribeTest_With_Long_Funnel)));

        await _queue.Publish(new TestEvent(Guid.NewGuid(), DateTime.UtcNow)
        {
            Name = baseName
        });

        var result = await _queue.ConsumeNext<TestResult_SubscribeTest_With_Long_Funnel>();

        result.Name.Should().Be(baseName);

        result.GetType().Should().Be<TestResult_SubscribeTest_With_Long_Funnel>();
    }

    private class TestEvent : Event
    {
        public string Name { get; set; }

        public TestEvent(Guid id, DateTime created) : base(id, created)
        {
        }
    }

    private class TestResult : Event
    {
        public string Name { get; set; }

        public TestResult(Guid id, DateTime created) : base(id, created)
        {
        }
    }

    private class TestResult_SubscribeTest : Event
    {
        public string Name { get; set; }

        public TestResult_SubscribeTest(Guid id, DateTime created) : base(id, created)
        {
        }
    }

    private class TestResult_SubscribeTest_WithFunnel : Event
    {
        public string Name { get; set; }

        public TestResult_SubscribeTest_WithFunnel(Guid id, DateTime created) : base(id, created)
        {
        }
    }

    private class TestResult_SubscribeTest_With_Long_Funnel : Event
    {
        public string Name { get; set; }

        public TestResult_SubscribeTest_With_Long_Funnel(Guid id, DateTime created) : base(id, created)
        {
        }
    }
}