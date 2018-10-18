using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.TaskProcessor.Model;
using Radoslav.TaskProcessor.TaskScheduler;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public sealed class TaskSchedulerUnitTests
    {
        public TestContext TestContext { get; set; }

        private FakeDateTimeProvider DateTimeProvider
        {
            get
            {
                return (FakeDateTimeProvider)this.TestContext.Properties["DateTimeProvider"];
            }
        }

        private FakeTaskProcessorRepository Repository
        {
            get
            {
                return (FakeTaskProcessorRepository)this.TestContext.Properties["Repository"];
            }
        }

        private FakeTaskProcessorFacade TaskProcessorFacade
        {
            get
            {
                return (FakeTaskProcessorFacade)this.TestContext.Properties["Facade"];
            }
        }

        private FakeConfigurationProvider ConfigurationProvider
        {
            get
            {
                return (FakeConfigurationProvider)this.TestContext.Properties["ConfigurationProvider"];
            }
        }

        private TaskSchedulerPollingJob Scheduler
        {
            get
            {
                return (TaskSchedulerPollingJob)this.TestContext.Properties["Scheduler"];
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            this.TestContext.Properties.Add("ConfigurationProvider", new FakeConfigurationProvider());
            this.TestContext.Properties.Add("Repository", new FakeTaskProcessorRepository());
            this.TestContext.Properties.Add("Facade", new FakeTaskProcessorFacade());
            this.TestContext.Properties.Add("DateTimeProvider", new FakeDateTimeProvider());
            this.TestContext.Properties.Add("Scheduler", new TaskSchedulerPollingJob(this.ConfigurationProvider, this.Repository, this.DateTimeProvider, this.TaskProcessorFacade));
        }

        [TestMethod]
        public void OneTimeScheduleDefinitionTest()
        {
            DateTime timestamp = DateTime.UtcNow;

            OneTimeScheduleDefinition definition = new OneTimeScheduleDefinition(timestamp);

            Assert.IsNull(definition.NextExecutionTimeUtc);

            definition.CalculateNextExecutionTime(timestamp.AddMinutes(-1));

            Assert.AreEqual(timestamp, definition.NextExecutionTimeUtc);

            definition.CalculateNextExecutionTime(timestamp.AddMinutes(1));

            Assert.IsNull(definition.NextExecutionTimeUtc);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullConfigurationProvider()
        {
            using (new TaskSchedulerPollingJob(null, new FakeTaskProcessorRepository(), new FakeDateTimeProvider(), new FakeTaskProcessorFacade()))
            {
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullRepository()
        {
            using (new TaskSchedulerPollingJob(new FakeConfigurationProvider(), null, new FakeDateTimeProvider(), new FakeTaskProcessorFacade()))
            {
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullDateTimeProvider()
        {
            using (new TaskSchedulerPollingJob(new FakeConfigurationProvider(), new FakeTaskProcessorRepository(), null, new FakeTaskProcessorFacade()))
            {
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullFacade()
        {
            using (new TaskSchedulerPollingJob(new FakeConfigurationProvider(), new FakeTaskProcessorRepository(), new FakeDateTimeProvider(), null))
            {
            }
        }

        [TestMethod]
        public void Initialize()
        {
            FakeScheduledTask scheduledTask = new FakeScheduledTask();

            this.Repository.ScheduledTasks.Add(scheduledTask);

            this.DateTimeProvider.UtcNow = DateTime.UtcNow;

            this.Scheduler.Initialize();

            scheduledTask.Schedule.AssertMethodCallOnceWithArguments(s => s.CalculateNextExecutionTime(this.DateTimeProvider.UtcNow));
        }

        [TestMethod]
        public void ProcessScheduledTask()
        {
            FakeScheduledTask scheduledTask = new FakeScheduledTask();

            this.Repository.ScheduledTasks.Add(scheduledTask);

            this.DateTimeProvider.UtcNow = DateTime.UtcNow.AddMinutes(-1);

            scheduledTask.Schedule.PredefineMethodCall(
                s => s.CalculateNextExecutionTime(this.DateTimeProvider.UtcNow),
                () => scheduledTask.Schedule.NextExecutionTimeUtc = DateTime.UtcNow);

            this.Scheduler.Initialize();

            scheduledTask.Schedule.RecordedMethodCalls.Clear();

            this.DateTimeProvider.UtcNow = DateTime.UtcNow.AddMinutes(1);

            scheduledTask.PredefineMethodCall(s => s.PrepareNextTask(), () =>
            {
                scheduledTask.NextTask = new FakeTask();
                scheduledTask.NextTaskPriority = TaskPriority.Low;
                scheduledTask.NextTaskSummary = new StringTaskSummary("Hello");
            });

            /* scheduledTask.Schedule.PredefineMethodCall(
                s => s.CalculateNextExecutionTime(this.DateTimeProvider.UtcNow),
                () => scheduledTask.Schedule.NextExecutionTimeUtc = DateTime.UtcNow); */

            this.Scheduler.Process();

            scheduledTask.AssertMethodCallOnce(s => s.PrepareNextTask());

            this.TaskProcessorFacade.AssertMethodCallOnceWithArguments(f => f.SubmitTask(scheduledTask.NextTask, scheduledTask.NextTaskSummary, scheduledTask.NextTaskPriority));

            scheduledTask.Schedule.AssertMethodCallOnceWithArguments(s => s.CalculateNextExecutionTime(this.DateTimeProvider.UtcNow));
        }

        [TestMethod]
        public void RemoveScheduledTaskAfterProcess()
        {
            // TODO ...
        }
    }
}