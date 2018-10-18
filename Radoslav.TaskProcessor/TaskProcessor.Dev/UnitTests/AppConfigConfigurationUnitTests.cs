using System;
using System.Configuration;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Configuration;
using Radoslav.Serialization;
using Radoslav.TaskProcessor.Configuration;
using Radoslav.TaskProcessor.Configuration.AppConfig;
using Radoslav.TaskProcessor.Model;
using Radoslav.Timers;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public sealed class AppConfigConfigurationUnitTests
    {
        public TestContext TestContext { get; set; }

        #region Task Jobs

        [TestMethod]
        public void UndefinedTask()
        {
            ITaskProcessorConfiguration configuration = AppConfigConfigurationUnitTests.GetTaskProcessorConfiguration("Empty");

            Assert.IsNull(configuration.Tasks[typeof(IFakeTask)]);
        }

        [TestMethod]
        public void MaxTasks()
        {
            ITaskProcessorConfiguration configuration = AppConfigConfigurationUnitTests.GetTaskProcessorConfiguration("MaxTasks");

            Assert.AreEqual(100, configuration.Tasks.MaxWorkers);

            ITaskJobConfiguration jobConfig = configuration.Tasks[typeof(IFakeTask)];

            Assert.IsNotNull(jobConfig);

            Assert.AreEqual(typeof(IFakeTask), jobConfig.TaskType);

            Assert.AreEqual(10, jobConfig.MaxWorkers);
        }

        [TestMethod]
        public void NoMaxTasks()
        {
            ITaskProcessorConfiguration configuration = AppConfigConfigurationUnitTests.GetTaskProcessorConfiguration("NoMaxTasks");

            Assert.IsNull(configuration.Tasks.MaxWorkers);

            ITaskJobConfiguration jobConfig = configuration.Tasks[typeof(IFakeTask)];

            Assert.IsNotNull(jobConfig);

            Assert.AreEqual(typeof(IFakeTask), jobConfig.TaskType);

            Assert.IsNull(jobConfig.MaxWorkers);
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void NegativeMaxTasks1()
        {
            ITaskProcessorConfiguration configuration = AppConfigConfigurationUnitTests.GetTaskProcessorConfiguration("NegativeMaxTasks1");

            if (configuration.Tasks.MaxWorkers > 0)
            {
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void NegativeMaxTasks2()
        {
            ITaskProcessorConfiguration configuration = AppConfigConfigurationUnitTests.GetTaskProcessorConfiguration("NegativeMaxTasks2");

            if (configuration.Tasks[typeof(FakeTask)].MaxWorkers > 0)
            {
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void NoTaskType()
        {
            AppConfigConfigurationUnitTests.GetTaskProcessorConfiguration("NoTaskType");
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void EmptyTaskType()
        {
            AppConfigConfigurationUnitTests.GetTaskProcessorConfiguration("EmptyTaskType");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetTaskJobNullTaskType()
        {
            ITaskProcessorConfiguration configuration = AppConfigConfigurationUnitTests.GetTaskProcessorConfiguration("Empty");

            if (configuration.Tasks[null] == null)
            {
            }
        }

        #endregion Task Jobs

        #region Polling Jobs

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void NoPollingJobImplementationType()
        {
            AppConfigConfigurationUnitTests.GetTaskProcessorConfiguration("NoPollingJobType");
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void EmptyPollingJobImplementationType()
        {
            AppConfigConfigurationUnitTests.GetTaskProcessorConfiguration("EmptyPollingJobType");
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void AbstractPollingJobImplementationType()
        {
            AppConfigConfigurationUnitTests.GetTaskProcessorConfiguration("EmptyPollingJobType");
        }

        [TestMethod]
        public void PollingJobTypeWithoutParameterlessConstructor()
        {
            AppConfigConfigurationUnitTests.GetTaskProcessorConfiguration("PollingJobTypeWithoutParameterlessConstructor");
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void PollingJobNoInterval()
        {
            AppConfigConfigurationUnitTests.GetTaskProcessorConfiguration("PollingJobNoInterval");
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void PollingJobEmptyInterval()
        {
            AppConfigConfigurationUnitTests.GetTaskProcessorConfiguration("PollingJobEmptyInterval");
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void PollingJobZeroInterval()
        {
            AppConfigConfigurationUnitTests.GetTaskProcessorConfiguration("PollingJobZeroInterval");
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void DuplicatePollingJobs()
        {
            AppConfigConfigurationUnitTests.GetTaskProcessorConfiguration("DuplicatePollingJobs");
        }

        [TestMethod]
        public void PollingJobs()
        {
            ITaskProcessorConfiguration configuration = AppConfigConfigurationUnitTests.GetTaskProcessorConfiguration("PollingJobs");

            Assert.AreEqual(3, configuration.PollingJobs.Count());

            IPollingJobConfiguration config = configuration.PollingJobs.First();

            Assert.AreEqual(typeof(FakePollingJob), config.ImplementationType);
            Assert.AreEqual(TimeSpan.FromMinutes(1), config.PollInterval);
            Assert.IsTrue(config.IsMaster);
            Assert.IsFalse(config.IsActive);
            Assert.IsFalse(config.IsConcurrent);

            config = configuration.PollingJobs.Skip(1).First();

            Assert.AreEqual(typeof(FakePollingJob2), config.ImplementationType);
            Assert.AreEqual(TimeSpan.FromMinutes(2), config.PollInterval);
            Assert.IsFalse(config.IsMaster);
            Assert.IsTrue(config.IsActive);
            Assert.IsFalse(config.IsConcurrent);

            config = configuration.PollingJobs.Skip(2).First();

            Assert.AreEqual(typeof(FakePollingJob3), config.ImplementationType);
            Assert.AreEqual(TimeSpan.FromMinutes(3), config.PollInterval);
            Assert.IsFalse(config.IsMaster);
            Assert.IsTrue(config.IsActive);
            Assert.IsTrue(config.IsConcurrent);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetPollingJobNullImplementationType()
        {
            ITaskProcessorConfiguration configuration = AppConfigConfigurationUnitTests.GetTaskProcessorConfiguration("Empty");

            if (configuration.PollingJobs[null] == null)
            {
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetPollingJobImplementationTypeNotDescendant1()
        {
            ITaskProcessorConfiguration configuration = AppConfigConfigurationUnitTests.GetTaskProcessorConfiguration("Empty");

            if (configuration.PollingJobs[typeof(IPollingJob)] == null)
            {
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetPollingJobImplementationTypeNotDescendant2()
        {
            ITaskProcessorConfiguration configuration = AppConfigConfigurationUnitTests.GetTaskProcessorConfiguration("Empty");

            if (configuration.PollingJobs[typeof(ICloneable)] == null)
            {
            }
        }

        #endregion Polling Jobs

        #region Polling Queues

        [TestMethod]
        public void PollingQueues()
        {
            ITaskProcessorConfiguration configuration = AppConfigConfigurationUnitTests.GetTaskProcessorConfiguration("PollingQueues");

            Assert.AreEqual(3, configuration.PollingQueues.Count());

            ITaskProcessorPollingQueueConfiguration config = configuration.PollingQueues.First();

            Assert.AreEqual("A", config.Key);
            Assert.AreEqual(TimeSpan.FromMinutes(1), config.PollInterval);
            Assert.IsTrue(config.IsMaster);
            Assert.IsFalse(config.IsActive);
            Assert.IsFalse(config.IsConcurrent);
            Assert.AreEqual(0, config.MaxWorkers);

            config = configuration.PollingQueues.Skip(1).First();

            Assert.AreEqual("B", config.Key);
            Assert.AreEqual(TimeSpan.FromMinutes(2), config.PollInterval);
            Assert.IsFalse(config.IsMaster);
            Assert.IsTrue(config.IsActive);
            Assert.IsFalse(config.IsConcurrent);
            Assert.AreEqual(5, config.MaxWorkers);

            config = configuration.PollingQueues.Skip(2).First();

            Assert.AreEqual("C", config.Key);
            Assert.AreEqual(TimeSpan.FromMinutes(3), config.PollInterval);
            Assert.IsFalse(config.IsMaster);
            Assert.IsTrue(config.IsActive);
            Assert.IsTrue(config.IsConcurrent);
            Assert.AreEqual(10, config.MaxWorkers);
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void DuplicatePollingQueues()
        {
            AppConfigConfigurationUnitTests.GetTaskProcessorConfiguration("DuplicatePollingQueues");
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void PollingQueueNoKey()
        {
            AppConfigConfigurationUnitTests.GetTaskProcessorConfiguration("PollingQueueNoKey");
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void PollingQueueEmptyKey()
        {
            AppConfigConfigurationUnitTests.GetTaskProcessorConfiguration("PollingQueueEmptyKey");
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void NegativePollingQueueMaxWorkers()
        {
            var configuration = AppConfigConfigurationUnitTests.GetTaskProcessorConfiguration("NegativePollingQueueMaxWorkers");

            if (configuration.PollingQueues.First().MaxWorkers > 0)
            {
            }
        }

        #endregion Polling Queues

        #region Task Worker

        [TestMethod]
        public void TaskWorker()
        {
            ITaskWorkersConfiguration configuration = AppConfigConfigurationUnitTests.GetTaskWorkerConfiguration("TaskWorker");

            Assert.AreEqual(typeof(FakeTaskWorker), configuration[typeof(FakeTask)].WorkerType);
            Assert.AreEqual(typeof(FakeTaskWorker), configuration[typeof(FakePollingQueueTask)].WorkerType);

            Assert.IsTrue(configuration[typeof(FakeTask)].HasTaskJobSettings);
            Assert.IsFalse(configuration[typeof(FakePollingQueueTask)].HasTaskJobSettings);
        }

        [TestMethod]
        public void EmptyTaskWorkerConfiguration1()
        {
            ITaskWorkersConfiguration configuration = AppConfigConfigurationUnitTests.GetTaskWorkerConfiguration("Empty1");

            Assert.IsNotNull(configuration);

            Assert.IsNull(configuration[typeof(FakeTask)]);
        }

        [TestMethod]
        public void EmptyTaskWorkerConfiguration2()
        {
            ITaskWorkersConfiguration configuration = AppConfigConfigurationUnitTests.GetTaskWorkerConfiguration("Empty2");

            Assert.IsNotNull(configuration);

            Assert.IsNull(configuration[typeof(FakeTask)]);
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void TaskWorkerNoTaskType()
        {
            AppConfigConfigurationUnitTests.GetTaskWorkerConfiguration("NoTaskType");
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void TaskWorkerEmptyTaskType()
        {
            AppConfigConfigurationUnitTests.GetTaskWorkerConfiguration("EmptyTaskType");
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void TaskWorkerNoWorkerType()
        {
            AppConfigConfigurationUnitTests.GetTaskWorkerConfiguration("NoWorkerType");
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void TaskWorkerEmptyWorkerType()
        {
            AppConfigConfigurationUnitTests.GetTaskWorkerConfiguration("EmptyWorkerType");
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void TaskWorkerInvalidTaskType()
        {
            AppConfigConfigurationUnitTests.GetTaskWorkerConfiguration("InvalidTaskType");
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void TaskWorkerInvalidWorkerType()
        {
            AppConfigConfigurationUnitTests.GetTaskWorkerConfiguration("InvalidWorkerType");
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void TaskWorkerWorkerTypeNoPublicConstructor()
        {
            AppConfigConfigurationUnitTests.GetTaskWorkerConfiguration("WorkerTypeNoPublicConstructor");
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void TaskWorkerWorkerTypeNoParameterlessConstructor()
        {
            AppConfigConfigurationUnitTests.GetTaskWorkerConfiguration("WorkerTypeNoParameterlessConstructor");
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void TaskWorkerDuplicateTask()
        {
            AppConfigConfigurationUnitTests.GetTaskWorkerConfiguration("DuplicateTask");
        }

        #endregion Task Worker

        #region Client

        [TestMethod]
        public void ClientConfiguration()
        {
            ITaskProcessorClientConfiguration configuration = AppConfigConfigurationUnitTests.GetClientConfiguration("Client");

            Assert.IsNull(configuration.GetPollingQueueKey(typeof(FakeTask)));
            Assert.AreEqual("Demo", configuration.GetPollingQueueKey(typeof(FakePollingQueueTask)));
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void ClientConfigurationDuplicatePollingQueue()
        {
            AppConfigConfigurationUnitTests.GetClientConfiguration("DuplicatePollingQueue");
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void ClientConfigurationAmbiguousTask()
        {
            AppConfigConfigurationUnitTests.GetClientConfiguration("AmbigousTask");
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void ClientConfigurationAmbiguousTaskPollingQueue()
        {
            AppConfigConfigurationUnitTests.GetClientConfiguration("AmbigousTaskPollingQueue");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ClientConfigurationGetPollingQueueKeyNullTaskType()
        {
            ITaskProcessorClientConfiguration config = AppConfigConfigurationUnitTests.GetClientConfiguration("Client");

            config.GetPollingQueueKey(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ClientConfigurationGetPollingQueueKeyInvalidTaskType()
        {
            ITaskProcessorClientConfiguration config = AppConfigConfigurationUnitTests.GetClientConfiguration("Client");

            config.GetPollingQueueKey(typeof(ITimer));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ClientConfigurationGetPollingQueueKeyTaskTypeNotFound()
        {
            ITaskProcessorClientConfiguration config = AppConfigConfigurationUnitTests.GetClientConfiguration("Empty");

            config.GetPollingQueueKey(typeof(FakeTask));
        }

        #endregion Client

        #region Serialization

        [TestMethod]
        public void SerializationConfiguration()
        {
            ITaskProcessorSerializationConfiguration configuration = AppConfigConfigurationUnitTests.GetSerializationtConfiguration("Serialization");

            Assert.AreEqual(typeof(EntityBinarySerializer), configuration.GetSerializerType(typeof(FakeTask)));
            Assert.AreEqual(typeof(EntityXmlSerializer), configuration.GetSerializerType(typeof(ITask)));
            Assert.AreEqual(typeof(EntityBinaryXmlSerializer), configuration.GetSerializerType(typeof(object)));

            Assert.IsNull(configuration.GetSerializerType(typeof(FakePollingQueueTask)));
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void SerializationConfigurationDuplicateTask()
        {
            AppConfigConfigurationUnitTests.GetSerializationtConfiguration("DuplicateTask");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SerializationConfigurationGetSerializerTypeNull()
        {
            ITaskProcessorSerializationConfiguration configuration = AppConfigConfigurationUnitTests.GetSerializationtConfiguration("Serialization");

            configuration.GetSerializerType(null);
        }

        #endregion Serialization

        #region Task Scheduler

        [TestMethod]
        public void TaskScheduler()
        {
            ITaskSchedulerConfiguration configuration = AppConfigConfigurationUnitTests.GetTaskSchedulerConfiguration("TaskScheduler");

            Assert.IsFalse(configuration.ScheduledTasks[typeof(FakeScheduledTask)].WaitForPreviousSubmittedTaskToComplete);
        }

        [TestMethod]
        public void TaskSchedulerWaitFalse()
        {
            ITaskSchedulerConfiguration configuration = AppConfigConfigurationUnitTests.GetTaskSchedulerConfiguration("WaitFalse");

            Assert.IsFalse(configuration.ScheduledTasks[typeof(FakeScheduledTask)].WaitForPreviousSubmittedTaskToComplete);
        }

        [TestMethod]
        public void TaskSchedulerWaitTrue()
        {
            ITaskSchedulerConfiguration configuration = AppConfigConfigurationUnitTests.GetTaskSchedulerConfiguration("WaitTrue");

            Assert.IsTrue(configuration.ScheduledTasks[typeof(FakeScheduledTask)].WaitForPreviousSubmittedTaskToComplete);
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void TaskSchedulerNoScheduledTaskType()
        {
            AppConfigConfigurationUnitTests.GetTaskSchedulerConfiguration("NoScheduledTaskType");
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void TaskSchedulerEmptyScheduledTaskType()
        {
            AppConfigConfigurationUnitTests.GetTaskSchedulerConfiguration("EmptyScheduledTaskType");
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void InvalidScheduledTaskType()
        {
            AppConfigConfigurationUnitTests.GetTaskSchedulerConfiguration("InvalidScheduledTaskType");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetScheduledTaskNull()
        {
            ITaskSchedulerConfiguration configuration = AppConfigConfigurationUnitTests.GetTaskSchedulerConfiguration("TaskScheduler");

            if (configuration.ScheduledTasks[null] == null)
            {
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetScheduledTaskInvalid()
        {
            ITaskSchedulerConfiguration configuration = AppConfigConfigurationUnitTests.GetTaskSchedulerConfiguration("TaskScheduler");

            if (configuration.ScheduledTasks[typeof(object)] == null)
            {
            }
        }

        #endregion Task Scheduler

        #region Helpers

        private static ITaskProcessorConfiguration GetTaskProcessorConfiguration(string configFileName)
        {
            string configFilePath = Path.Combine("Configuration", "Processor", configFileName + ".config");

            return ConfigurationHelpers.Load<TaskProcessorConfigurationSection>(TaskProcessorConfigurationSection.SectionName, configFilePath);
        }

        private static ITaskWorkersConfiguration GetTaskWorkerConfiguration(string configFileName)
        {
            string configFilePath = Path.Combine("Configuration", "TaskWorker", configFileName + ".config");

            return ConfigurationHelpers.Load<TaskWorkerConfigurationSection>(TaskWorkerConfigurationSection.SectionName, configFilePath);
        }

        private static ITaskProcessorClientConfiguration GetClientConfiguration(string configFileName)
        {
            string configFilePath = Path.Combine("Configuration", "Client", configFileName + ".config");

            return ConfigurationHelpers.Load<ClientConfigurationSection>(ClientConfigurationSection.SectionName, configFilePath);
        }

        private static ITaskProcessorSerializationConfiguration GetSerializationtConfiguration(string configFileName)
        {
            string configFilePath = Path.Combine("Configuration", "Serialization", configFileName + ".config");

            return ConfigurationHelpers.Load<SerializationConfigurationSection>(SerializationConfigurationSection.SectionName, configFilePath);
        }

        private static ITaskSchedulerConfiguration GetTaskSchedulerConfiguration(string configFileName)
        {
            string configFilePath = Path.Combine("Configuration", "TaskScheduler", configFileName + ".config");

            return ConfigurationHelpers.Load<TaskSchedulerConfigurationSection>(TaskSchedulerConfigurationSection.SectionName, configFilePath);
        }

        #endregion Helpers
    }
}