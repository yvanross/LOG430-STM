namespace ApplicationLogicTests.Usecases
{
    /*
    [TestClass()]
    public class ChaosExperimentTest
    {
        private ChaosExperiment _chaosExperimentUc;

        [TestInitialize]
        public void Init()
        {
            _chaosExperimentUc = new ChaosExperiment(
                MockProvider.GetEnvironmentMock().Object,
                MockProvider.GetReadModelMock().Object,
                MockProvider.GetWriteModelMock().Object,
                MockProvider.GetChaosCodexMock().Object,
                MockProvider.GetSystemStateWriteMock().Object,
                MockProvider.GetDataStreamReadMock().Object
            );
        }

        [TestMethod]
        public void SerializeExperiment()
        {
            var json = JsonProvider.GenerateExperimentBody();

            var experimentDto = JsonConvert.DeserializeObject<ExperimentDto>(json);

            experimentDto.Should().NotBeNull();

            //experimentDto.ChaosCodex.ChaosConfigs.Should().HaveCount(3);
        }

        [TestMethod]
        public async Task InduceChaos_GenericSettings()
        {
            await _chaosExperimentUc.InduceChaos();

            _chaosExperimentUc.KillsThisMinute.Should().Be(0);
        }

        [TestMethod]
        public async Task InduceChaos_ZeroComputationPodsAllowed()
        {
            var chaosConfigMock = MockProvider.GetChaosConfigMock();

            chaosConfigMock.Setup(x => x.MaxNumberOfPods).Returns(0);

            var preMocked = new KeyValuePair<ArtifactTypeEnum, IChaosConfig>
                (ArtifactTypeEnum.Computation, chaosConfigMock.Object);

            var chaosCodexMock = MockProvider.GetChaosCodexMock(preMocked);

            _chaosExperimentUc = new ChaosExperiment(
                MockProvider.GetEnvironmentMock().Object,
                MockProvider.GetReadModelMock().Object,
                MockProvider.GetWriteModelMock().Object,
                chaosCodexMock.Object,
                MockProvider.GetSystemStateWriteMock().Object,
                MockProvider.GetDataStreamReadMock().Object
            );

            await _chaosExperimentUc.InduceChaos();

            _chaosExperimentUc.KillsThisMinute.Should().Be(3);
        }

        [TestMethod]
        public async Task InduceChaos_ExtremelyHighKillRate()
        {
            var chaosConfigMock = MockProvider.GetChaosConfigMock();

            chaosConfigMock.Setup(x => x.KillRate).Returns(100000);

            var preMocked = new KeyValuePair<ArtifactTypeEnum, IChaosConfig>
                (ArtifactTypeEnum.Computation, chaosConfigMock.Object);

            var chaosCodexMock = MockProvider.GetChaosCodexMock(preMocked);

            _chaosExperimentUc = new ChaosExperiment(
                MockProvider.GetEnvironmentMock().Object,
                MockProvider.GetReadModelMock().Object,
                MockProvider.GetWriteModelMock().Object,
                chaosCodexMock.Object,
                MockProvider.GetSystemStateWriteMock().Object,
                MockProvider.GetDataStreamReadMock().Object
            );

            await _chaosExperimentUc.InduceChaos();

            _chaosExperimentUc.KillsThisMinute.Should().BeGreaterThan(3);
        }

        [TestMethod]
        public async Task ReportTestResult_GenericSettings()
        {
            _chaosExperimentUc.ReportTestResult(MockProvider.GetSagaMock(0,1).Object);
            _chaosExperimentUc.ReportTestResult(MockProvider.GetSagaMock(0, 2).Object);
            _chaosExperimentUc.ReportTestResult(MockProvider.GetSagaMock(1, 0).Object);
            _chaosExperimentUc.ReportTestResult(MockProvider.GetSagaMock(1,1).Object);

            _chaosExperimentUc.ExperimentResult.Should().NotBeNull();
            _chaosExperimentUc.ExperimentResult.Message.Should().Be("Test message");
            _chaosExperimentUc.ExperimentResult.AverageLatency.Should().BeApproximately(1, 0.1);
            _chaosExperimentUc.ExperimentResult.ErrorCount.Should().Be(0);
        }

        [TestMethod]
        public async Task ReportTestResult_IncreasedLatency()
        {
            _chaosExperimentUc.ReportTestResult(MockProvider.GetSagaMock(0, 2).Object);
            _chaosExperimentUc.ReportTestResult(MockProvider.GetSagaMock(0, 4).Object);
            _chaosExperimentUc.ReportTestResult(MockProvider.GetSagaMock(1, 0).Object);
            _chaosExperimentUc.ReportTestResult(MockProvider.GetSagaMock(1, 2).Object);

            _chaosExperimentUc.ExperimentResult.Should().NotBeNull();
            _chaosExperimentUc.ExperimentResult.Message.Should().Be("Test message");
            _chaosExperimentUc.ExperimentResult.AverageLatency.Should().BeApproximately(2, 0.1);
            _chaosExperimentUc.ExperimentResult.ErrorCount.Should().Be(0);
        }

        [TestMethod]
        public async Task ReportTestResult_ErrorCount()
        {
            _chaosExperimentUc.ReportTestResult(MockProvider.GetSagaMock().Object);
            _chaosExperimentUc.ReportTestResult(MockProvider.GetSagaMock(0, 1).Object);
            _chaosExperimentUc.ReportTestResult(MockProvider.GetSagaMock(0, 2).Object);
            _chaosExperimentUc.ReportTestResult(MockProvider.GetSagaMock(1, 1).Object);
            _chaosExperimentUc.ReportTestResult(MockProvider.GetSagaMock(1, 0).Object);

            _chaosExperimentUc.ExperimentResult.Should().NotBeNull();
            _chaosExperimentUc.ExperimentResult.Message.Should().Be("Test message");
            _chaosExperimentUc.ExperimentResult.ErrorCount.Should().Be(1);
        }
    }
*/
}