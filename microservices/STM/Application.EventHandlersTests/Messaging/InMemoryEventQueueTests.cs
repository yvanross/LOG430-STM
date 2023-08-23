using Microsoft.VisualStudio.TestTools.UnitTesting;
using Application.EventHandlers.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.EventHandlers.Messaging.PipeAndFilter;
using Application.EventHandlersTests;
using FluentAssertions;

namespace Application.EventHandlers.Messaging.Tests
{
    [TestClass()]
    public class InMemoryEventQueueTests
    {
        public TestContext TestContext { get; set; }

        private InMemoryEventQueue _queue;

        [TestInitialize]
        public void Setup()
        {
            _queue = new InMemoryEventQueue();
        }

        [TestMethod()]
        public void PublishTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ConsumeNextTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public async Task SubscribeTest()
        {
            const string baseName = "test ";
            const string newName = nameof(SubscribeTest);

            _queue.Subscribe<TestEvent, TestEvent>(async (result, token) =>
                {
                    result.Name.Should().Be(baseName);

                    result.GetType().Should().Be<TestEvent>();

                    _queue.Publish(new TestResult_SubscribeTest()
                    {
                        Name = newName
                    });
                }, new TestLogger(TestContext));

            _queue.Publish(new TestEvent()
            {
                Name = baseName
            });

            var result = await _queue.ConsumeNext<TestResult_SubscribeTest>();

            result.Name.Should().Be(newName);

            result.GetType().Should().Be<TestResult_SubscribeTest>();
        }

        [TestMethod()]
        public async Task SubscribeTest_WithFunnel()
        {
            const string baseName = "test ";
            const string newName = nameof(SubscribeTest_WithFunnel);

            _queue.Subscribe<TestEvent, TestResult>(async (result, token) =>
            {
                result.Name.Should().Be(newName);

                result.GetType().Should().Be<TestResult>();

                _queue.Publish(new TestResult_SubscribeTest_WithFunnel()
                {
                    Name = result.Name
                });
            }, new TestLogger(TestContext),
                new Funnel(async (reader, writer, cancellationToken) =>
                {
                    await foreach (var @event in reader.ReadAllAsync(cancellationToken))
                    {
                        var eventTransform = new TestResult()
                        {
                            Name = newName
                        };

                        await writer.WriteAsync(eventTransform, cancellationToken);
                    }
                }, typeof(TestEvent), typeof(TestResult)));

            _queue.Publish(new TestEvent()
            {
                Name = baseName
            });

            var result = await _queue.ConsumeNext<TestResult_SubscribeTest_WithFunnel>();

            result.Name.Should().Be(newName);

            result.GetType().Should().Be<TestResult_SubscribeTest_WithFunnel>();
        }

        [TestMethod()]
        public async Task SubscribeTest_With_Long_Funnel()
        {
            const string baseName = "test ";
            const string newName = nameof(SubscribeTest_WithFunnel);

            _queue.Subscribe<TestEvent, TestResult_SubscribeTest_With_Long_Funnel>(async (result, token) =>
                {
                    result.GetType().Should().Be<TestResult_SubscribeTest_With_Long_Funnel>();

                    _queue.Publish(result);
                }, logger: new TestLogger(TestContext),
                new Funnel(async (reader, writer, cancellationToken) =>
                {
                    await foreach (var @event in reader.ReadAllAsync(cancellationToken))
                    {
                        var eventTransform = new TestResult()
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
                        var eventTransform = new TestResult_SubscribeTest_With_Long_Funnel()
                        {
                            Name = baseName
                        };
                        await writer.WriteAsync(eventTransform, cancellationToken);
                    }
                }, typeof(TestResult), typeof(TestResult_SubscribeTest_With_Long_Funnel)));

            _queue.Publish(new TestEvent()
            {
                Name = baseName
            });

            var result = await _queue.ConsumeNext<TestResult_SubscribeTest_With_Long_Funnel>();

            result.Name.Should().Be(baseName);

            result.GetType().Should().Be<TestResult_SubscribeTest_With_Long_Funnel>();
        }

        [TestMethod()]
        public void UnSubscribeTest()
        {
            Assert.Fail();
        }

        private class TestEvent
        {
            public string Name { get; set; }
        }

        private class TestResult
        {
            public string Name { get; set; }
        }

        private class TestResult_SubscribeTest
        {
            public string Name { get; set; }
        }

        private class TestResult_SubscribeTest_WithFunnel
        {
            public string Name { get; set; }
        }

        private class TestResult_SubscribeTest_With_Long_Funnel
        {
            public string Name { get; set; }
        }
    }
}