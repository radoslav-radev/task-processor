using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.TaskProcessor.Configuration;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public sealed class MergeConfigurationUnitTests
    {
        #region Task Jobs

        [TestMethod]
        public void AddTaskJob()
        {
            FakeTaskProcessorConfiguration config1 = new FakeTaskProcessorConfiguration();
            FakeTaskProcessorConfiguration config2 = new FakeTaskProcessorConfiguration();

            config2.Tasks.Add(typeof(IFakeTask)).MaxWorkers = 20;

            config1.MergeWith(config2);

            Assert.IsNotNull(config1.Tasks[typeof(IFakeTask)]);
            Assert.AreEqual(20, config1.Tasks[typeof(IFakeTask)].MaxWorkers);
        }

        [TestMethod]
        public void RemoveTaskJob()
        {
            FakeTaskProcessorConfiguration config1 = new FakeTaskProcessorConfiguration();

            config1.Tasks.Add(typeof(IFakeTask));

            FakeTaskProcessorConfiguration config2 = new FakeTaskProcessorConfiguration();

            config1.MergeWith(config2);

            Assert.IsNull(config1.Tasks[typeof(IFakeTask)]);
        }

        #endregion Task Jobs

        #region Polling Jobs

        [TestMethod]
        public void DoNotChangePollingJob()
        {
            FakeTaskProcessorConfiguration config1 = new FakeTaskProcessorConfiguration();

            config1.PollingJobs.Add(typeof(FakePollingJob));

            config1.PollingJobs[typeof(FakePollingJob)].PollInterval = TimeSpan.FromMinutes(1);

            FakeTaskProcessorConfiguration config2 = new FakeTaskProcessorConfiguration();

            config2.PollingJobs.Add(typeof(FakePollingJob));

            config1.MergeWith(config2);

            Assert.AreEqual(TimeSpan.FromMinutes(1), config1.PollingJobs[typeof(FakePollingJob)].PollInterval);
        }

        [TestMethod]
        public void AddPollingJob()
        {
            FakeTaskProcessorConfiguration config1 = new FakeTaskProcessorConfiguration();
            FakeTaskProcessorConfiguration config2 = new FakeTaskProcessorConfiguration();

            config2.PollingJobs.Add(typeof(FakePollingJob));

            config1.MergeWith(config2);

            Assert.IsNotNull(config1.PollingJobs[typeof(FakePollingJob)]);
        }

        [TestMethod]
        public void RemovePollingJob()
        {
            FakeTaskProcessorConfiguration config1 = new FakeTaskProcessorConfiguration();

            config1.PollingJobs.Add(typeof(FakePollingJob));

            FakeTaskProcessorConfiguration config2 = new FakeTaskProcessorConfiguration();

            config1.MergeWith(config2);

            Assert.IsNull(config1.PollingJobs[typeof(FakePollingJob)]);
        }

        #endregion Polling Jobs

        #region Polling Queues

        [TestMethod]
        public void DoNotChangePollingQueue()
        {
            FakeTaskProcessorConfiguration config1 = new FakeTaskProcessorConfiguration();

            config1.PollingQueues.Add("A").PollInterval = TimeSpan.FromMinutes(1);

            FakeTaskProcessorConfiguration config2 = new FakeTaskProcessorConfiguration();

            config2.PollingQueues.Add("A");

            config1.MergeWith(config2);

            Assert.AreEqual(TimeSpan.FromMinutes(1), config1.PollingQueues["A"].PollInterval);
        }

        [TestMethod]
        public void AddPollingQueue()
        {
            FakeTaskProcessorConfiguration config1 = new FakeTaskProcessorConfiguration();
            FakeTaskProcessorConfiguration config2 = new FakeTaskProcessorConfiguration();

            ITaskProcessorPollingQueueConfiguration pollingQueueConfig2 = config2.PollingQueues.Add("A");

            pollingQueueConfig2.MaxWorkers = 11;

            config1.MergeWith(config2);

            ITaskProcessorPollingQueueConfiguration pollingQueueConfig1 = config1.PollingQueues["A"];

            Assert.AreEqual(pollingQueueConfig2.MaxWorkers, pollingQueueConfig1.MaxWorkers);
        }

        [TestMethod]
        public void RemovePollingQueue()
        {
            FakeTaskProcessorConfiguration config1 = new FakeTaskProcessorConfiguration();

            config1.PollingQueues.Add("A");

            FakeTaskProcessorConfiguration config2 = new FakeTaskProcessorConfiguration();

            config1.MergeWith(config2);

            Assert.IsNull(config1.PollingQueues["A"]);
        }

        #endregion Polling Queues
    }
}