using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.DateTimeProvider;
using Radoslav.TaskProcessor.Configuration;
using Radoslav.TaskProcessor.Facade;
using Radoslav.TaskProcessor.MessageBus;
using Radoslav.TaskProcessor.Model;
using Radoslav.Timers;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public sealed class TaskProcessorUnitTests
    {
        private static readonly object LockObject = new object();

        #region Properties & Initialize

        public TestContext TestContext { get; set; }

        private RadoslavTaskProcessor TaskProcessor
        {
            get
            {
                return (RadoslavTaskProcessor)this.TestContext.Properties["RadoslavTaskProcessor"];
            }
        }

        private TaskProcessorFacade TaskProcessorFacade
        {
            get
            {
                return (TaskProcessorFacade)this.TestContext.Properties["TaskProcessorFacade"];
            }
        }

        private FakeTaskProcessorRepository Repository
        {
            get
            {
                return (FakeTaskProcessorRepository)this.TaskProcessor.Repository;
            }
        }

        private FakeMessageBus MessageBus
        {
            get
            {
                return (FakeMessageBus)this.TaskProcessor.MessageBus;
            }
        }

        private FakeDateTimeProvider DateTimeProvider
        {
            get
            {
                return (FakeDateTimeProvider)this.TaskProcessor.DateTimeProvider;
            }
        }

        private FakeMasterCommandsProcessor MasterCommandsProcessor
        {
            get
            {
                return (FakeMasterCommandsProcessor)this.TaskProcessor.MasterCommandsProcessor;
            }
        }

        private FakeTaskExecutor TaskExecutor
        {
            get
            {
                return (FakeTaskExecutor)this.TaskProcessor.TaskExecutor;
            }
        }

        private FakeTimer HeartbeatTimer
        {
            get
            {
                return (FakeTimer)this.TaskProcessor.HeartbeatTimer;
            }
        }

        private FakeTimer ReportPerformanceTimer
        {
            get
            {
                return (FakeTimer)this.TaskProcessor.ReportPerformanceTimer;
            }
        }

        private FakeApplicationKiller ApplicationKiller
        {
            get
            {
                return (FakeApplicationKiller)this.TaskProcessor.ApplicationKiller;
            }
        }

        private FakeDelayStrategy RetryHeartbeatDelayStrategy
        {
            get
            {
                return (FakeDelayStrategy)this.TaskProcessor.RetryHeartbeatDelayStrategy;
            }
        }

        private FakeConfigurationProvider ConfigurationProvider
        {
            get
            {
                return (FakeConfigurationProvider)this.TaskProcessor.AppConfigConfigurationProvider;
            }
        }

        private FakeServiceLocator ServiceLocator
        {
            get
            {
                return (FakeServiceLocator)this.TaskProcessor.ServiceLocator;
            }
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = SuppressMessages.DisposedOnTestCleanup)]
        [TestInitialize]
        public void TestInitialize()
        {
            Monitor.Enter(TaskProcessorUnitTests.LockObject);

            FakeTaskProcessorRepository repository = new FakeTaskProcessorRepository();
            FakeMessageBus messageBus = new FakeMessageBus();
            FakeDateTimeProvider dateTimeProvider = new FakeDateTimeProvider();
            FakeConfigurationProvider configProvider = new FakeConfigurationProvider();

            configProvider.PredefineResult(new FakeClientConfiguration(), p => p.GetClientConfiguration());

            FakeServiceLocator locator = new FakeServiceLocator();

            locator.PredefineResult(new FakeTimer(), l => l.ResolveSingle(typeof(ITimer))); // Heartbeat timer
            locator.PredefineResult(new FakeTimer(), l => l.ResolveSingle(typeof(ITimer))); // Report performance timer.

            this.TestContext.Properties.Add("TaskProcessorFacade", new TaskProcessorFacade(repository, messageBus, dateTimeProvider, configProvider));
            this.TestContext.Properties.Add("RadoslavTaskProcessor", new RadoslavTaskProcessor(new FakeConfigurationProvider(), repository, messageBus, new FakeTaskExecutor(), new FakeMasterCommandsProcessor(), dateTimeProvider, new FakeApplicationKiller(), new FakeDelayStrategy(), locator));

            FakePollingJob.InitializeFakePollingJobsInThread();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            foreach (IDisposable disposable in this.ServiceLocator.CreatedObjects.OfType<IDisposable>())
            {
                disposable.Dispose();
            }

            this.TaskProcessor.Dispose();

            FakePollingJob.ClearFakePollingJobsInThread();

            Monitor.Exit(TaskProcessorUnitTests.LockObject);
        }

        #endregion Properties & Initialize

        #region Constructor

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullConfigurationProvider()
        {
            using (new RadoslavTaskProcessor(null, new FakeTaskProcessorRepository(), new FakeMessageBus(), new FakeTaskExecutor(), new FakeMasterCommandsProcessor(), new DefaultDateTimeProvider(), new FakeApplicationKiller(), new FakeDelayStrategy(), new FakeServiceLocator()))
            {
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullRepository()
        {
            using (new RadoslavTaskProcessor(new FakeConfigurationProvider(), null, new FakeMessageBus(), new FakeTaskExecutor(), new FakeMasterCommandsProcessor(), new DefaultDateTimeProvider(), new FakeApplicationKiller(), new FakeDelayStrategy(), new FakeServiceLocator()))
            {
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullMessageBus()
        {
            using (new RadoslavTaskProcessor(new FakeConfigurationProvider(), new FakeTaskProcessorRepository(), null, new FakeTaskExecutor(), new FakeMasterCommandsProcessor(), new DefaultDateTimeProvider(), new FakeApplicationKiller(), new FakeDelayStrategy(), new FakeServiceLocator()))
            {
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullMasterCommandsProcessor()
        {
            using (new RadoslavTaskProcessor(new FakeConfigurationProvider(), new FakeTaskProcessorRepository(), new FakeMessageBus(), new FakeTaskExecutor(), null, new DefaultDateTimeProvider(), new FakeApplicationKiller(), new FakeDelayStrategy(), new FakeServiceLocator()))
            {
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullTaskExecutor()
        {
            using (new RadoslavTaskProcessor(new FakeConfigurationProvider(), new FakeTaskProcessorRepository(), new FakeMessageBus(), null, new FakeMasterCommandsProcessor(), new DefaultDateTimeProvider(), new FakeApplicationKiller(), new FakeDelayStrategy(), new FakeServiceLocator()))
            {
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullDateTimeProvider()
        {
            using (new RadoslavTaskProcessor(new FakeConfigurationProvider(), new FakeTaskProcessorRepository(), new FakeMessageBus(), new FakeTaskExecutor(), new FakeMasterCommandsProcessor(), null, new FakeApplicationKiller(), new FakeDelayStrategy(), new FakeServiceLocator()))
            {
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullApplicationKiller()
        {
            using (new RadoslavTaskProcessor(new FakeConfigurationProvider(), new FakeTaskProcessorRepository(), new FakeMessageBus(), new FakeTaskExecutor(), new FakeMasterCommandsProcessor(), new DefaultDateTimeProvider(), null, new FakeDelayStrategy(), new FakeServiceLocator()))
            {
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullDelayStrategy()
        {
            using (new RadoslavTaskProcessor(new FakeConfigurationProvider(), new FakeTaskProcessorRepository(), new FakeMessageBus(), new FakeTaskExecutor(), new FakeMasterCommandsProcessor(), new DefaultDateTimeProvider(), new FakeApplicationKiller(), null, new FakeServiceLocator()))
            {
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullServiceLocator()
        {
            using (new RadoslavTaskProcessor(new FakeConfigurationProvider(), new FakeTaskProcessorRepository(), new FakeMessageBus(), new FakeTaskExecutor(), new FakeMasterCommandsProcessor(), new DefaultDateTimeProvider(), new FakeApplicationKiller(), new FakeDelayStrategy(), null))
            {
            }
        }

        [TestMethod]
        public void Constructor()
        {
            Assert.AreNotEqual(Guid.Empty, this.TaskProcessor.Id);

            Assert.AreNotEqual(TimeSpan.Zero, this.TaskProcessor.MonitoringTimeout);

            Assert.AreNotEqual(0, this.TaskProcessor.MaxHeartbeatRetries);
        }

        #endregion Constructor

        #region Configuration

        [TestMethod]
        public void GetDefaultConfigurationAppConfig()
        {
            ITaskProcessorConfiguration configuration = new FakeTaskProcessorConfiguration();

            configuration.Tasks.Add(typeof(IFakeTask)).MaxWorkers = 20;

            configuration.PollingJobs.Add(typeof(FakePollingJob)).PollInterval = TimeSpan.FromMinutes(1);

            this.ConfigurationProvider.PredefineResult(configuration, p => p.GetTaskProcessorConfiguration());

            this.TaskProcessor.Start();

            ITaskProcessorRuntimeInfo processorInfo = this.Repository.TaskProcessorRuntimeInfo.GetById(this.TaskProcessor.Id);

            Assert.AreEqual(20, processorInfo.Configuration.Tasks[typeof(IFakeTask)].MaxWorkers);
            Assert.AreEqual(TimeSpan.FromMinutes(1), configuration.PollingJobs[typeof(FakePollingJob)].PollInterval);
        }

        #endregion Configuration

        #region Start

        [TestMethod]
        public void Start()
        {
            this.TaskProcessor.Start();

            Assert.IsTrue(this.TaskProcessor.Repository.TaskProcessorRuntimeInfo.GetAll().Any(t => t.TaskProcessorId == this.TaskProcessor.Id));

            ITaskProcessorRuntimeInfo processorInfo = this.TaskProcessor.Repository.TaskProcessorRuntimeInfo.GetById(this.TaskProcessor.Id);

            Assert.IsNotNull(processorInfo);

            Assert.AreEqual(Environment.MachineName, processorInfo.MachineName);

            Assert.IsTrue(this.TaskProcessor.HeartbeatTimer.IsActive);

            Assert.AreEqual(TaskProcessorState.Active, this.TaskProcessor.State);

            this.MessageBus.TaskProcessors.AssertMethodCallWithArguments(mb => mb.SubscribeForChannels(new ExpectedCollection<MessageBusChannel>(
                MessageBusChannel.MasterModeChangeRequest,
                MessageBusChannel.MasterModeChanged,
                MessageBusChannel.StopTaskProcessor,
                MessageBusChannel.PerformanceMonitoringRequest,
                MessageBusChannel.ConfigurationChanged)));

            this.MessageBus.Tasks.AssertMethodCallWithArguments(mb => mb.SubscribeForChannels(new ExpectedCollection<MessageBusChannel>(
                MessageBusChannel.TaskAssigned,
                MessageBusChannel.TaskCancelRequest)));
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void StartAfterDispose()
        {
            this.TaskProcessor.Dispose();
            this.TaskProcessor.Start();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void StartWithHeartbeatIntervalEqualToRepositoryExpiration()
        {
            this.TaskProcessor.HeartbeatTimer.Interval = TimeSpan.FromMinutes(1);
            this.TaskProcessor.Repository.TaskProcessorRuntimeInfo.Expiration = TimeSpan.FromMinutes(1);

            this.TaskProcessor.Start();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void StartWithHeartbeatIntervalGreaterToRepositoryExpiration()
        {
            this.TaskProcessor.HeartbeatTimer.Interval = TimeSpan.FromMinutes(2);
            this.TaskProcessor.Repository.TaskProcessorRuntimeInfo.Expiration = TimeSpan.FromMinutes(1);

            this.TaskProcessor.Start();
        }

        [TestMethod]
        public void StartTwice()
        {
            this.TaskProcessor.Start();
            this.TaskProcessor.Start();
        }

        [TestMethod]
        public void StartAsMaster()
        {
            this.DateTimeProvider.UtcNow = Helpers.GetRandomDateTime();

            this.TaskProcessor.Start();

            Thread.Sleep(TimeSpan.FromSeconds(1));

            Assert.IsTrue(this.MessageBus.MasterCommands.ReceiveMessages);

            this.MessageBus.Tasks.AssertMethodCallWithArguments(mb => mb.SubscribeForChannels(new ExpectedCollection<MessageBusChannel>(MessageBusChannel.TaskStarted)));

            this.MessageBus.TaskProcessors.AssertMethodCallOnceWithArguments(mb => mb.NotifyMasterModeChanged(this.TaskProcessor.Id, true, MasterModeChangeReason.Start));

            this.MasterCommandsProcessor.AssertMethodCallOnce(mb => mb.ProcessMasterCommands());
        }

        [TestMethod]
        public void StartAsSlave()
        {
            this.TaskProcessor.Repository.TaskProcessorRuntimeInfo.SetMasterIfNotExists(Guid.NewGuid());

            this.TaskProcessor.Start();

            Thread.Sleep(TimeSpan.FromSeconds(1));

            Assert.IsTrue(this.TaskProcessor.MessageBus.MasterCommands
                .OfType<TaskProcessorRegisteredMasterCommand>()
                .Any(m => m.TaskProcessorId == this.TaskProcessor.Id));

            Assert.IsFalse(this.MessageBus.MasterCommands.ReceiveMessages);

            this.MessageBus.Tasks.AssertNoMethodCallWithArguments(mb => mb.SubscribeForChannels(new ExpectedCollection<MessageBusChannel>(MessageBusChannel.TaskStarted)));
            this.MessageBus.TaskProcessors.AssertNoMethodCall(mb => mb.NotifyMasterModeChanged(Guid.Empty, false, MasterModeChangeReason.None));
            this.MasterCommandsProcessor.AssertNoMethodCall(mb => mb.ProcessMasterCommands());
        }

        #endregion Start

        #region Stop

        [TestMethod]
        public void StopNoActiveTasks()
        {
            this.TaskProcessor.Start();

            this.MessageBus.TaskProcessors.NotifyStopRequested(this.TaskProcessor.Id);

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            Assert.IsFalse(this.Repository.TaskProcessorRuntimeInfo.GetAll().Any());
            Assert.IsNull(this.Repository.TaskProcessorRuntimeInfo.GetById(this.TaskProcessor.Id));

            Assert.IsFalse(this.TaskProcessor.HeartbeatTimer.IsActive);

            Assert.AreEqual(TaskProcessorState.Inactive, this.TaskProcessor.State);

            this.MessageBus.TaskProcessors.AssertMethodCallOnce(mb => mb.UnsubscribeFromAllChannels());
            this.MessageBus.Tasks.Receiver.AssertMethodCallOnce(mb => mb.UnsubscribeFromAllChannels());
        }

        [TestMethod]
        public void StopSlave()
        {
            this.Repository.TaskProcessorRuntimeInfo.SetMasterIfNotExists(Guid.NewGuid());

            this.TaskProcessor.Start();

            this.MessageBus.TaskProcessors.NotifyStopRequested(this.TaskProcessor.Id);

            Assert.IsNotNull(this.Repository.TaskProcessorRuntimeInfo.GetMasterId());
        }

        [TestMethod]
        public void StopMaster()
        {
            this.TaskProcessor.Start();

            this.MessageBus.TaskProcessors.NotifyStopRequested(this.TaskProcessor.Id);

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            Assert.IsNull(this.Repository.TaskProcessorRuntimeInfo.GetMasterId());
        }

        [TestMethod]
        public void StopBeforeStart()
        {
            this.MessageBus.TaskProcessors.NotifyStopRequested(this.TaskProcessor.Id);

            this.MessageBus.TaskProcessors.AssertNoMethodCall(mb => mb.UnsubscribeFromAllChannels());
            this.MessageBus.Tasks.Receiver.AssertNoMethodCall(mb => mb.UnsubscribeFromAllChannels());

            this.Repository.TaskProcessorRuntimeInfo.AssertNoMethodCall(r => r.GetMasterId());
        }

        [TestMethod]
        public void StopMasterWithActiveTasks()
        {
            this.StopWithActiveTasks(true);
        }

        [TestMethod]
        public void StopSlaveWithActiveTasks()
        {
            this.StopWithActiveTasks(false);
        }

        [TestMethod]
        public void IgnoreStopForAnotherProcessor()
        {
            this.TaskProcessor.Start();

            this.MessageBus.TaskProcessors.NotifyStopRequested(Guid.NewGuid());

            Assert.AreEqual(TaskProcessorState.Active, this.TaskProcessor.State);
        }

        [TestMethod]
        public void IgnoreStopIfDisposed()
        {
            this.TaskProcessor.Dispose();

            this.MessageBus.TaskProcessors.NotifyStopRequested(Guid.NewGuid());

            Assert.AreEqual(TaskProcessorState.Disposed, this.TaskProcessor.State);
        }

        #endregion Stop

        #region Dispose

        [TestMethod]
        public void DisposeBeforeStart()
        {
            this.TaskProcessor.Dispose();

            Assert.AreEqual(TaskProcessorState.Disposed, this.TaskProcessor.State);

            this.MessageBus.TaskProcessors.AssertMethodCallOnce(mb => mb.UnsubscribeFromAllChannels());
            this.MessageBus.Tasks.Receiver.AssertMethodCallOnce(mb => mb.UnsubscribeFromAllChannels());
            this.MessageBus.TaskProcessors.AssertMethodCallOnceWithArguments(mb => mb.NotifyStateChanged(this.TaskProcessor.Id, TaskProcessorState.Disposed));
        }

        [TestMethod]
        public void DisposeAfterStart()
        {
            this.TaskProcessor.Start();

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            this.MessageBus.TaskProcessors.RecordedMethodCalls.Clear();

            this.TaskProcessor.Dispose();

            Assert.AreEqual(TaskProcessorState.Disposed, this.TaskProcessor.State);

            Assert.IsFalse(this.MasterCommandsProcessor.IsActive);

            this.HeartbeatTimer.AssertMethodCallOnce(t => t.Stop());
            this.ReportPerformanceTimer.AssertMethodCallOnce(t => t.Stop());

            this.MessageBus.TaskProcessors.AssertMethodCallOnce(mb => mb.UnsubscribeFromAllChannels());
            this.MessageBus.Tasks.Receiver.AssertMethodCallOnce(mb => mb.UnsubscribeFromAllChannels());
            this.MessageBus.TaskProcessors.AssertMethodCallOnceWithArguments(mb => mb.NotifyStateChanged(this.TaskProcessor.Id, TaskProcessorState.Disposed));
        }

        [TestMethod]
        public void DisposeTwiceBeforeStart()
        {
            this.TaskProcessor.Dispose();
            this.TaskProcessor.Dispose();
        }

        [TestMethod]
        public void DisposeTwiceAfterStart()
        {
            this.TaskProcessor.Start();
            this.TaskProcessor.Dispose();
            this.TaskProcessor.Dispose();
        }

        #endregion Dispose

        #region Assign Task

        [TestMethod]
        public void TaskAssignedToProcessor()
        {
            this.TaskProcessor.Start();

            Guid taskId = this.TaskProcessorFacade.SubmitTask(new FakeTask());

            ITaskRuntimeInfo taskInfo = this.Repository.TaskRuntimeInfo.GetById(taskId);

            this.Repository.TaskRuntimeInfo.Assign(taskId, this.TaskProcessor.Id);

            this.DateTimeProvider.UtcNow = DateTime.UtcNow;

            this.MessageBus.Tasks.NotifyTaskAssigned(taskId, this.TaskProcessor.Id);

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            taskInfo = this.Repository.TaskRuntimeInfo.GetById(taskId);

            Assert.IsNotNull(taskInfo);
            Assert.AreEqual(this.TaskProcessor.Id, taskInfo.TaskProcessorId);
            Assert.AreEqual(TaskStatus.InProgress, taskInfo.Status);
            Assert.IsNotNull(taskInfo.StartedUtc);

            taskInfo = (FakeTaskRuntimeInfo)this.Repository.TaskRuntimeInfo.GetPendingAndActive().SelectMany(p => p.Value).Single();

            this.TaskExecutor.AssertMethodCallOnceWithArguments(te => te.StartTask(taskInfo.TaskId, taskInfo.Priority));

            this.MessageBus.Tasks.AssertMethodCallOnceWithArguments(mb => mb.NotifyTaskStarted(taskInfo.TaskId, this.TaskProcessor.Id, this.DateTimeProvider.UtcNow));

            this.Repository.TaskRuntimeInfo.AssertMethodCallOnceWithArguments(r => r.Start(taskId, this.TaskProcessor.Id, this.DateTimeProvider.UtcNow));
        }

        [TestMethod]
        public void TaskAssignedToAnotherProcessor()
        {
            this.TaskProcessor.Start();

            Guid taskId = this.TaskProcessorFacade.SubmitTask(new FakeTask());

            this.MessageBus.Tasks.NotifyTaskAssigned(taskId, Guid.NewGuid());

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            ITaskRuntimeInfo taskInfo = this.Repository.TaskRuntimeInfo.GetById(taskId);

            Assert.IsNull(taskInfo.TaskProcessorId);
            Assert.AreEqual(TaskStatus.Pending, taskInfo.Status);

            this.TaskExecutor.AssertNoMethodCall(te => te.StartTask(Guid.Empty, TaskPriority.Normal));

            this.MessageBus.Tasks.AssertNoMethodCall(mb => mb.NotifyTaskStarted(Guid.Empty, Guid.Empty, DateTime.UtcNow));
        }

        [TestMethod]
        public void TaskAssignedToProcessorInactive()
        {
            Assert.AreEqual(TaskProcessorState.Inactive, this.TaskProcessor.State);

            Guid taskId = this.TaskProcessorFacade.SubmitTask(new FakeTask());

            this.MessageBus.Tasks.NotifyTaskAssigned(taskId, this.TaskProcessor.Id);

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            ITaskRuntimeInfo taskInfo = this.Repository.TaskRuntimeInfo.GetById(taskId);

            Assert.IsNull(taskInfo.TaskProcessorId);
            Assert.AreEqual(TaskStatus.Pending, taskInfo.Status);

            this.TaskExecutor.AssertNoMethodCall(te => te.StartTask(Guid.Empty, TaskPriority.Normal));

            this.MessageBus.Tasks.AssertNoMethodCall(mb => mb.NotifyTaskStarted(Guid.Empty, Guid.Empty, DateTime.UtcNow));
        }

        [TestMethod]
        public void TaskAssignedToProcessorStopping()
        {
            this.TaskProcessor.Start();

            this.MakeTaskProcessorStopping();

            Guid taskId = this.TaskProcessorFacade.SubmitTask(new FakeTask());

            this.MessageBus.Tasks.NotifyTaskAssigned(taskId, this.TaskProcessor.Id);

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            ITaskRuntimeInfo taskInfo = this.Repository.TaskRuntimeInfo.GetById(taskId);

            Assert.AreEqual(TaskStatus.Pending, taskInfo.Status);
            Assert.IsNull(taskInfo.TaskProcessorId);
        }

        [TestMethod]
        public void TaskAssignedToProcessorStopped()
        {
            this.TaskProcessor.Start();

            this.MessageBus.TaskProcessors.NotifyStopRequested(this.TaskProcessor.Id);

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            Assert.AreEqual(TaskProcessorState.Inactive, this.TaskProcessor.State);

            Guid taskId = this.TaskProcessorFacade.SubmitTask(new FakeTask());

            this.MessageBus.Tasks.NotifyTaskAssigned(taskId, this.TaskProcessor.Id);

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            ITaskRuntimeInfo taskInfo = this.Repository.TaskRuntimeInfo.GetById(taskId);

            Assert.AreEqual(TaskStatus.Pending, taskInfo.Status);
            Assert.IsNull(taskInfo.TaskProcessorId);

            this.TaskExecutor.AssertNoMethodCall(te => te.StartTask(Guid.Empty, TaskPriority.Normal));

            this.MessageBus.Tasks.AssertNoMethodCall(mb => mb.NotifyTaskStarted(Guid.Empty, Guid.Empty, DateTime.UtcNow));
        }

        [TestMethod]
        public void TaskAssignedToProcessorDispose()
        {
            this.TaskProcessor.Dispose();

            Assert.AreEqual(TaskProcessorState.Disposed, this.TaskProcessor.State);

            Guid taskId = this.TaskProcessorFacade.SubmitTask(new FakeTask());

            this.MessageBus.Tasks.NotifyTaskAssigned(taskId, this.TaskProcessor.Id);

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            ITaskRuntimeInfo taskInfo = this.Repository.TaskRuntimeInfo.GetById(taskId);

            Assert.IsNull(taskInfo.TaskProcessorId);
            Assert.AreEqual(TaskStatus.Pending, taskInfo.Status);

            this.TaskExecutor.AssertNoMethodCall(te => te.StartTask(Guid.Empty, TaskPriority.Normal));

            this.MessageBus.Tasks.AssertNoMethodCall(mb => mb.NotifyTaskStarted(Guid.Empty, Guid.Empty, DateTime.UtcNow));
        }

        [TestMethod]
        public void TaskAssignedToProcessorTaskNotFound()
        {
            this.TaskProcessor.Start();

            Guid taskId = Guid.NewGuid();

            this.MessageBus.Tasks.NotifyTaskAssigned(taskId, this.TaskProcessor.Id);

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            this.TaskExecutor.AssertNoMethodCall(te => te.StartTask(Guid.Empty, TaskPriority.Normal));

            this.MessageBus.Tasks.AssertNoMethodCall(mb => mb.NotifyTaskStarted(Guid.Empty, Guid.Empty, DateTime.UtcNow));
        }

        [TestMethod]
        public void TaskAssignedToProcessorTaskAlreadyInProgress()
        {
            this.TaskProcessor.Start();

            Guid taskId = this.TaskProcessorFacade.SubmitTask(new FakeTask());

            this.Repository.TaskRuntimeInfo.Start(taskId, Guid.NewGuid(), DateTime.UtcNow);

            this.MessageBus.Tasks.NotifyTaskAssigned(taskId, this.TaskProcessor.Id);

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            ITaskRuntimeInfo taskInfo = this.Repository.TaskRuntimeInfo.GetById(taskId);

            Assert.AreNotEqual(this.TaskProcessor.Id, taskInfo.TaskProcessorId);

            this.TaskExecutor.AssertNoMethodCall(te => te.StartTask(Guid.Empty, TaskPriority.Normal));

            this.MessageBus.Tasks.AssertNoMethodCall(mb => mb.NotifyTaskStarted(Guid.Empty, Guid.Empty, DateTime.UtcNow));
        }

        [TestMethod]
        public void TaskAssignedToProcessorTaskAssignedToAnotherProcessor()
        {
            this.TaskProcessor.Start();

            Guid taskId = this.TaskProcessorFacade.SubmitTask(new FakeTask());

            this.Repository.TaskRuntimeInfo.Assign(taskId, Guid.NewGuid());

            this.MessageBus.Tasks.NotifyTaskAssigned(taskId, this.TaskProcessor.Id);

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            ITaskRuntimeInfo taskInfo = this.Repository.TaskRuntimeInfo.GetById(taskId);

            Assert.AreNotEqual(this.TaskProcessor.Id, taskInfo.TaskProcessorId);

            this.TaskExecutor.AssertNoMethodCall(te => te.StartTask(Guid.Empty, TaskPriority.Normal));

            this.MessageBus.Tasks.AssertNoMethodCall(mb => mb.NotifyTaskStarted(Guid.Empty, Guid.Empty, DateTime.UtcNow));
        }

        #endregion Assign Task

        #region Task Completed

        [TestMethod]
        public void TaskCompletedCancel()
        {
            this.TaskProcessor.Start();

            Guid taskId = this.TaskProcessorFacade.SubmitTask(new FakeTask());

            this.DateTimeProvider.UtcNow = DateTime.UtcNow;

            this.TaskExecutor.CompleteTask(taskId, TaskStatus.Canceled);

            this.MessageBus.Tasks.AssertMethodCallOnceWithArguments(mb => mb.NotifyTaskCancelCompleted(taskId, this.DateTimeProvider.UtcNow, this.TaskProcessor.Id, false));

            this.Repository.Tasks.AssertMethodCallOnceWithArguments(r => r.Delete(taskId));
        }

        [TestMethod]
        public void TaskCompletedFail()
        {
            this.TaskProcessor.Start();

            Guid taskId = this.TaskProcessorFacade.SubmitTask(new FakeTask());

            this.Repository.TaskRuntimeInfo[taskId].Error = "Hello Error";

            this.DateTimeProvider.UtcNow = DateTime.UtcNow;

            this.TaskExecutor.CompleteTask(taskId, TaskStatus.Failed);

            this.MessageBus.Tasks.AssertMethodCallOnceWithArguments(mb => mb.NotifyTaskFailed(taskId, this.DateTimeProvider.UtcNow, this.TaskProcessor.Id, false, "Hello Error"));

            this.Repository.Tasks.AssertMethodCallOnceWithArguments(r => r.Delete(taskId));
        }

        [TestMethod]
        public void TaskCompletedSuccess()
        {
            this.TaskProcessor.Start();

            Guid taskId = this.TaskProcessorFacade.SubmitTask(new FakeTask());

            this.DateTimeProvider.UtcNow = DateTime.UtcNow;

            this.TaskExecutor.CompleteTask(taskId, TaskStatus.Success);

            this.MessageBus.Tasks.AssertMethodCallOnceWithArguments(mb => mb.NotifyTaskCompleted(taskId, this.DateTimeProvider.UtcNow, this.TaskProcessor.Id, false, TimeSpan.FromSeconds(1)));

            this.Repository.TaskRuntimeInfo.AssertMethodCallOnceWithArguments(r => r.Complete(taskId, this.DateTimeProvider.UtcNow));

            this.Repository.Tasks.AssertMethodCallOnceWithArguments(r => r.Delete(taskId));
        }

        [TestMethod]
        public void TaskCompletedWhileStopping()
        {
            this.TaskProcessor.Start();

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            this.MakeTaskProcessorStopping();

            Guid taskId = this.Repository.TaskRuntimeInfo.GetPendingAndActive().SelectMany(p => p.Value).Single().TaskId;

            this.TaskExecutor.CompleteTask(taskId, TaskStatus.Success);

            Thread.Sleep(TimeSpan.FromSeconds(1));

            Assert.AreEqual(TaskProcessorState.Inactive, this.TaskProcessor.State);

            this.MessageBus.TaskProcessors.AssertMethodCallWithArguments(mb => mb.NotifyStateChanged(this.TaskProcessor.Id, TaskProcessorState.Inactive));

            this.MessageBus.TaskProcessors.AssertMethodCallOnce(mb => mb.UnsubscribeFromAllChannels());
            this.MessageBus.Tasks.Receiver.AssertMethodCallOnce(mb => mb.UnsubscribeFromAllChannels());
        }

        #endregion Task Completed

        #region Heartbeat

        [TestMethod]
        public void Heartbeat()
        {
            this.TaskProcessor.Start();

            this.HeartbeatTimer.RaiseTick();

            this.Repository.TaskProcessorRuntimeInfo.AssertMethodCallOnceWithArguments(r => r.Heartbeat(this.TaskProcessor.Id));
        }

        [TestMethod]
        public void IgnoreHeartbeatIfInactive()
        {
            this.HeartbeatTimer.RaiseTick();

            this.Repository.TaskProcessorRuntimeInfo.AssertNoMethodCall(r => r.Heartbeat(this.TaskProcessor.Id));
        }

        [TestMethod]
        public void IgnoreHeartbeatIfStopping()
        {
            this.TaskProcessor.Start();

            Guid taskId = this.TaskProcessorFacade.SubmitTask(new FakeTask());

            this.MessageBus.Tasks.NotifyTaskAssigned(taskId, this.TaskProcessor.Id);

            Thread.Sleep(TimeSpan.FromSeconds(0.3));

            this.MessageBus.TaskProcessors.NotifyStopRequested(this.TaskProcessor.Id);

            Thread.Sleep(TimeSpan.FromSeconds(0.3));

            this.HeartbeatTimer.RaiseTick();

            this.Repository.TaskProcessorRuntimeInfo.AssertNoMethodCall(r => r.Heartbeat(this.TaskProcessor.Id));
        }

        [TestMethod]
        public void IgnoreHeartbeatIfStopped()
        {
            this.TaskProcessor.Start();

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            this.MessageBus.TaskProcessors.NotifyStopRequested(this.TaskProcessor.Id);

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            Assert.AreEqual(TaskProcessorState.Inactive, this.TaskProcessor.State);

            this.HeartbeatTimer.RaiseTick();

            this.Repository.TaskProcessorRuntimeInfo.AssertNoMethodCall(r => r.Heartbeat(this.TaskProcessor.Id));
        }

        [TestMethod]
        public void IgnoreHeartbeatIfDisposed()
        {
            this.TaskProcessor.Start();

            this.TaskProcessor.Dispose();

            Assert.AreEqual(TaskProcessorState.Disposed, this.TaskProcessor.State);

            this.HeartbeatTimer.RaiseTick();

            this.Repository.TaskProcessorRuntimeInfo.AssertNoMethodCall(r => r.Heartbeat(this.TaskProcessor.Id));
        }

        [TestMethod]
        public void HeartbeatMaster()
        {
            this.TaskProcessor.Start();

            this.HeartbeatTimer.RaiseTick();

            this.Repository.TaskProcessorRuntimeInfo.AssertMethodCallOnce(r => r.MasterHeartbeat());
        }

        [TestMethod]
        public void HeartbeatSlaveNoMaster()
        {
            this.Repository.TaskProcessorRuntimeInfo.SetMasterIfNotExists(Guid.NewGuid());

            this.TaskProcessor.Start();

            this.Repository.TaskProcessorRuntimeInfo.ClearMaster();

            this.Repository.TaskProcessorRuntimeInfo.RecordedMethodCalls.Clear();

            this.HeartbeatTimer.RaiseTick();

            this.Repository.TaskProcessorRuntimeInfo.AssertMethodCallOnceWithArguments(r => r.SetMasterIfNotExists(this.TaskProcessor.Id));
        }

        [TestMethod]
        public void HeartbeatFailed()
        {
            this.TaskProcessor.Start();

            this.Repository.TaskProcessorRuntimeInfo.PredefineResult(false, r => r.Heartbeat(this.TaskProcessor.Id));

            this.HeartbeatTimer.RaiseTick();

            this.ApplicationKiller.AssertMethodCallOnce(ak => ak.Kill());
        }

        [TestMethod]
        public void MasterHeartbeatFailed()
        {
            this.TaskProcessor.Start();

            this.Repository.TaskProcessorRuntimeInfo.PredefineResult(false, r => r.MasterHeartbeat());

            this.HeartbeatTimer.RaiseTick();

            this.ApplicationKiller.AssertMethodCallOnce(ak => ak.Kill());
        }

        [TestMethod]
        public void MasterChanged()
        {
            this.TaskProcessor.Start();

            this.Repository.TaskProcessorRuntimeInfo.ClearMaster();
            this.Repository.TaskProcessorRuntimeInfo.SetMasterIfNotExists(Guid.NewGuid());

            this.HeartbeatTimer.RaiseTick();

            Assert.IsFalse(this.MessageBus.MasterCommands.ReceiveMessages);

            this.MessageBus.Tasks.AssertMethodCallOnceWithArguments(mb => mb.UnsubscribeFromChannels(new ExpectedCollection<MessageBusChannel>(MessageBusChannel.TaskStarted)));

            Assert.IsFalse(this.MasterCommandsProcessor.IsActive);
        }

        [TestMethod]
        public void RetryHeartbeat()
        {
            this.TaskProcessor.MaxHeartbeatRetries = 4;

            this.TaskProcessor.Start();

            this.Repository.TaskProcessorRuntimeInfo.PredefineMethodCall(r => r.Heartbeat(this.TaskProcessor.Id), () => { throw new NotSupportedException(); });
            this.Repository.TaskProcessorRuntimeInfo.PredefineMethodCall(r => r.Heartbeat(this.TaskProcessor.Id), () => { throw new NotSupportedException(); });

            this.Repository.TaskProcessorRuntimeInfo.PredefineResult(true, r => r.Heartbeat(this.TaskProcessor.Id));

            this.HeartbeatTimer.RaiseTick();

            this.RetryHeartbeatDelayStrategy.AssertMethodCallsCount(2, s => s.NextDelay());

            this.Repository.TaskProcessorRuntimeInfo.AssertMethodCallsCount(3, r => r.Heartbeat(this.TaskProcessor.Id));
        }

        [TestMethod]
        public void RetryMasterHeartbeat()
        {
            this.TaskProcessor.MaxHeartbeatRetries = 4;

            this.TaskProcessor.Start();

            this.Repository.TaskProcessorRuntimeInfo.PredefineMethodCall(r => r.MasterHeartbeat(), () => { throw new NotSupportedException(); });
            this.Repository.TaskProcessorRuntimeInfo.PredefineMethodCall(r => r.MasterHeartbeat(), () => { throw new NotSupportedException(); });

            this.Repository.TaskProcessorRuntimeInfo.PredefineResult(true, r => r.MasterHeartbeat());

            this.HeartbeatTimer.RaiseTick();

            this.RetryHeartbeatDelayStrategy.AssertMethodCallsCount(2, s => s.NextDelay());

            this.Repository.TaskProcessorRuntimeInfo.AssertMethodCallsCount(3, r => r.MasterHeartbeat());
        }

        #endregion Heartbeat

        #region Performance Monitoring

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MonitoringTimeoutNegative()
        {
            this.TaskProcessor.MonitoringTimeout = TimeSpan.FromSeconds(-1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MonitoringTimeoutZero()
        {
            this.TaskProcessor.MonitoringTimeout = TimeSpan.Zero;
        }

        [TestMethod]
        public void MonitoringTimeout()
        {
            this.TaskProcessor.MonitoringTimeout = TimeSpan.FromSeconds(7);

            Assert.AreEqual(TimeSpan.FromSeconds(7), this.TaskProcessor.MonitoringTimeout);
        }

        [TestMethod]
        public void PerformanceMonitoringStarted()
        {
            this.TaskProcessor.Start();

            Assert.IsFalse(this.TaskProcessor.ReportPerformanceTimer.IsActive);

            this.MessageBus.TaskProcessors.NotifyPerformanceMonitoring(TimeSpan.FromSeconds(1));

            Thread.Sleep(TimeSpan.FromSeconds(2));

            Assert.AreEqual(TimeSpan.FromSeconds(1), this.TaskProcessor.ReportPerformanceTimer.Interval);

            Assert.IsTrue(this.TaskProcessor.ReportPerformanceTimer.IsActive);

            this.MessageBus.TaskProcessors.NotifyPerformanceMonitoring(TimeSpan.FromSeconds(2));

            Thread.Sleep(TimeSpan.FromSeconds(1));

            Assert.AreEqual(TimeSpan.FromSeconds(2), this.TaskProcessor.ReportPerformanceTimer.Interval);

            Assert.IsTrue(this.TaskProcessor.ReportPerformanceTimer.IsActive);
        }

        [TestMethod]
        public void StartPerformanceMonitoringBeforeStart()
        {
            this.MessageBus.TaskProcessors.NotifyPerformanceMonitoring(TimeSpan.FromSeconds(1));

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            Assert.IsFalse(this.ReportPerformanceTimer.IsActive);
            Assert.IsFalse(this.TaskExecutor.MonitorPerformance);
        }

        [TestMethod]
        public void PerformanceMonitoringWhileStopping()
        {
            this.TaskProcessor.Start();

            Assert.IsFalse(this.TaskProcessor.ReportPerformanceTimer.IsActive);

            this.MessageBus.TaskProcessors.NotifyPerformanceMonitoring(TimeSpan.FromSeconds(1));

            Thread.Sleep(TimeSpan.FromSeconds(2));

            Assert.AreEqual(TimeSpan.FromSeconds(1), this.TaskProcessor.ReportPerformanceTimer.Interval);

            Assert.IsTrue(this.TaskProcessor.ReportPerformanceTimer.IsActive);

            this.MakeTaskProcessorStopping();

            this.MessageBus.TaskProcessors.NotifyPerformanceMonitoring(TimeSpan.FromSeconds(2));

            Thread.Sleep(TimeSpan.FromSeconds(0.25));

            Assert.AreEqual(TimeSpan.FromSeconds(2), this.TaskProcessor.ReportPerformanceTimer.Interval);

            Assert.IsTrue(this.TaskProcessor.ReportPerformanceTimer.IsActive);
        }

        [TestMethod]
        public void StartPerformanceMonitoringAfterDispose()
        {
            this.TaskProcessor.Dispose();

            this.MessageBus.TaskProcessors.NotifyPerformanceMonitoring(TimeSpan.FromSeconds(1));

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            Assert.IsFalse(this.ReportPerformanceTimer.IsActive);
            Assert.IsFalse(this.TaskExecutor.MonitorPerformance);
        }

        [TestMethod]
        public void StopPerformanceMonitoringOnTimeout()
        {
            this.TaskProcessor.Start();

            this.TaskProcessor.MonitoringTimeout = TimeSpan.FromSeconds(1);

            this.MessageBus.TaskProcessors.NotifyPerformanceMonitoring(TimeSpan.FromSeconds(1));

            Thread.Sleep(TimeSpan.FromSeconds(2));

            this.ReportPerformanceTimer.RaiseTick();

            Assert.IsFalse(this.TaskExecutor.MonitorPerformance);
            Assert.IsFalse(this.ReportPerformanceTimer.IsActive);

            this.TaskExecutor.AssertNoMethodCall(e => e.GetTasksPerformanceInfo());
            this.MessageBus.TaskProcessors.AssertNoMethodCall(mb => mb.NotifyPerformanceReport(null));
        }

        [TestMethod]
        public void StopPerformanceMonitoringOnStopped()
        {
            this.TaskProcessor.Start();

            this.MessageBus.TaskProcessors.NotifyPerformanceMonitoring(TimeSpan.FromSeconds(1));

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            this.MessageBus.TaskProcessors.NotifyStopRequested(this.TaskProcessor.Id);

            Thread.Sleep(TimeSpan.FromSeconds(3));

            this.ReportPerformanceTimer.RaiseTick();

            Assert.IsFalse(this.TaskExecutor.MonitorPerformance);

            this.ReportPerformanceTimer.AssertMethodCallOnce(t => t.Stop());

            this.TaskExecutor.AssertNoMethodCall(e => e.GetTasksPerformanceInfo());
            this.MessageBus.TaskProcessors.AssertNoMethodCall(mb => mb.NotifyPerformanceReport(null));
        }

        [TestMethod]
        public void ReportPerformance()
        {
            this.TaskProcessor.Start();

            this.TaskProcessor.MonitoringTimeout = TimeSpan.FromSeconds(2);

            this.MessageBus.TaskProcessors.NotifyPerformanceMonitoring(TimeSpan.FromSeconds(1));

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            this.ReportPerformanceTimer.RaiseTick();

            this.TaskExecutor.AssertMethodCallOnceWithArguments(e => e.GetTasksPerformanceInfo());

            this.MessageBus.TaskProcessors.EqualityComparerFactory.RegisterCallback<TaskProcessorPerformanceReport>((a, b) => a.TaskProcessorId == b.TaskProcessorId);

            this.MessageBus.TaskProcessors.AssertMethodCallOnceWithArguments(mb => mb.NotifyPerformanceReport(new TaskProcessorPerformanceReport(this.TaskProcessor.Id)));

            Thread.Sleep(TimeSpan.FromSeconds(2));

            this.ReportPerformanceTimer.RaiseTick();

            this.MessageBus.TaskProcessors.AssertNoMethodCall(mb => mb.NotifyPerformanceReport(null));

            Assert.IsFalse(this.ReportPerformanceTimer.IsActive);
        }

        [TestMethod]
        public void DoNotReportPerformanceIfInactive()
        {
            this.ReportPerformanceTimer.RaiseTick();

            this.MessageBus.TaskProcessors.AssertNoMethodCall(mb => mb.NotifyPerformanceReport(null));
        }

        [TestMethod]
        public void DoNotReportPerformanceIfDisposed()
        {
            this.TaskProcessor.Dispose();

            this.ReportPerformanceTimer.RaiseTick();

            this.MessageBus.TaskProcessors.AssertNoMethodCall(mb => mb.NotifyPerformanceReport(null));
        }

        [TestMethod]
        public void SendPerformanceReportWhileStopping()
        {
            this.TaskProcessor.Start();

            this.MessageBus.TaskProcessors.NotifyPerformanceMonitoring(TimeSpan.FromSeconds(1));

            Thread.Sleep(TimeSpan.FromSeconds(1));

            this.MakeTaskProcessorStopping();

            this.ReportPerformanceTimer.RaiseTick();

            this.MessageBus.TaskProcessors.AssertMethodCallOnce(mb => mb.NotifyPerformanceReport(null));
        }

        #endregion Performance Monitoring

        #region Change Master Mode

        [TestMethod]
        public void MakeSlave()
        {
            this.TaskProcessor.Start();

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            this.DateTimeProvider.UtcNow = Helpers.GetRandomDateTime();

            this.MessageBus.TaskProcessors.RecordedMethodCalls.Clear();

            this.MessageBus.TaskProcessors.NotifyMasterModeChangeRequest(this.TaskProcessor.Id, false);

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            Assert.IsFalse(this.MasterCommandsProcessor.IsActive);
            Assert.IsFalse(this.MessageBus.MasterCommands.ReceiveMessages);

            this.MessageBus.Tasks.AssertMethodCallOnceWithArguments(mb => mb.UnsubscribeFromChannels(new ExpectedCollection<MessageBusChannel>(MessageBusChannel.TaskStarted)));
            this.MessageBus.TaskProcessors.AssertMethodCallOnceWithArguments(mb => mb.NotifyMasterModeChanged(this.TaskProcessor.Id, false, MasterModeChangeReason.Explicit));
        }

        [TestMethod]
        public void MakeSlaveButAlreadySlave()
        {
            this.Repository.TaskProcessorRuntimeInfo.SetMaster(Guid.NewGuid());

            this.TaskProcessor.Start();

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            this.MessageBus.TaskProcessors.NotifyMasterModeChangeRequest(this.TaskProcessor.Id, false);

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            Assert.IsFalse(this.MasterCommandsProcessor.IsActive);

            this.MessageBus.Tasks.AssertNoMethodCall(mb => mb.UnsubscribeFromChannels(null));
            this.MessageBus.TaskProcessors.AssertNoMethodCall(mb => mb.NotifyMasterModeChanged(Guid.Empty, false, MasterModeChangeReason.None));
        }

        [TestMethod]
        public void MakeAnotherProcessorSlave()
        {
            this.TaskProcessor.Start();

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            this.MessageBus.TaskProcessors.RecordedMethodCalls.Clear();

            this.MessageBus.TaskProcessors.NotifyMasterModeChangeRequest(Guid.NewGuid(), false);

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            Assert.IsTrue(this.MasterCommandsProcessor.IsActive);

            this.MessageBus.Tasks.AssertNoMethodCall(mb => mb.UnsubscribeFromChannels(null));
            this.MessageBus.TaskProcessors.AssertNoMethodCall(mb => mb.NotifyMasterModeChanged(Guid.Empty, false, MasterModeChangeReason.None));
        }

        [TestMethod]
        public void MakeSlaveInactive()
        {
            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            this.MessageBus.TaskProcessors.NotifyMasterModeChangeRequest(this.TaskProcessor.Id, false);

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            this.MessageBus.Tasks.AssertNoMethodCall(mb => mb.UnsubscribeFromChannels(null));
            this.MessageBus.TaskProcessors.AssertNoMethodCall(mb => mb.NotifyMasterModeChanged(Guid.Empty, false, MasterModeChangeReason.None));
        }

        [TestMethod]
        public void MakeSlaveStopping()
        {
            this.TaskProcessor.Start();

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            this.MakeTaskProcessorStopping();

            this.MessageBus.TaskProcessors.RecordedMethodCalls.Clear();

            this.MessageBus.TaskProcessors.NotifyMasterModeChangeRequest(this.TaskProcessor.Id, false);

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            this.MessageBus.Tasks.AssertNoMethodCall(mb => mb.UnsubscribeFromChannels(null));
            this.MessageBus.TaskProcessors.AssertNoMethodCall(mb => mb.NotifyMasterModeChanged(Guid.Empty, false, MasterModeChangeReason.None));
        }

        [TestMethod]
        public void MakeSlaveDisposedBeforeStart()
        {
            this.TaskProcessor.Dispose();

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            this.MessageBus.TaskProcessors.NotifyMasterModeChangeRequest(this.TaskProcessor.Id, false);

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            this.MessageBus.Tasks.AssertNoMethodCall(mb => mb.UnsubscribeFromChannels(null));
            this.MessageBus.TaskProcessors.AssertNoMethodCall(mb => mb.NotifyMasterModeChanged(Guid.Empty, false, MasterModeChangeReason.None));
        }

        [TestMethod]
        public void MakeSlaveDisposedAfterStart()
        {
            this.TaskProcessor.Start();

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            this.TaskProcessor.Dispose();

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            this.DateTimeProvider.UtcNow = Helpers.GetRandomDateTime();

            this.MessageBus.TaskProcessors.RecordedMethodCalls.Clear();

            this.MessageBus.TaskProcessors.NotifyMasterModeChangeRequest(this.TaskProcessor.Id, false);

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            this.MessageBus.Tasks.AssertNoMethodCall(mb => mb.UnsubscribeFromChannels(null));
            this.MessageBus.TaskProcessors.AssertNoMethodCall(mb => mb.NotifyMasterModeChanged(Guid.Empty, false, MasterModeChangeReason.None));
        }

        [TestMethod]
        public void MakeMaster()
        {
            this.Repository.TaskProcessorRuntimeInfo.SetMaster(Guid.NewGuid());

            this.TaskProcessor.Start();

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            this.DateTimeProvider.UtcNow = Helpers.GetRandomDateTime();

            this.MessageBus.Tasks.RecordedMethodCalls.Clear();

            this.MessageBus.TaskProcessors.NotifyMasterModeChangeRequest(this.TaskProcessor.Id, true);

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            Assert.IsTrue(this.MasterCommandsProcessor.IsActive);
            Assert.IsTrue(this.MessageBus.MasterCommands.ReceiveMessages);

            this.MasterCommandsProcessor.AssertMethodCallOnce(p => p.ProcessMasterCommands());

            this.MessageBus.Tasks.AssertMethodCallOnceWithArguments(mb => mb.SubscribeForChannels(new ExpectedCollection<MessageBusChannel>(MessageBusChannel.TaskStarted)));
            this.MessageBus.TaskProcessors.AssertMethodCallOnceWithArguments(mb => mb.NotifyMasterModeChanged(this.TaskProcessor.Id, true, MasterModeChangeReason.Explicit));
        }

        [TestMethod]
        public void MakeMasterButAlreadyMaster()
        {
            this.TaskProcessor.Start();

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            this.MessageBus.Tasks.RecordedMethodCalls.Clear();
            this.MasterCommandsProcessor.RecordedMethodCalls.Clear();
            this.MessageBus.TaskProcessors.RecordedMethodCalls.Clear();

            this.MessageBus.TaskProcessors.NotifyMasterModeChangeRequest(this.TaskProcessor.Id, true);

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            Assert.IsTrue(this.MasterCommandsProcessor.IsActive);

            this.MasterCommandsProcessor.AssertNoMethodCall(p => p.ProcessMasterCommands());

            this.MessageBus.Tasks.AssertNoMethodCall(mb => mb.SubscribeForChannels(null));
            this.MessageBus.TaskProcessors.AssertNoMethodCall(mb => mb.NotifyMasterModeChanged(Guid.Empty, true, MasterModeChangeReason.None));
        }

        [TestMethod]
        public void MakeAnotherProcessorMasterWhenSlave()
        {
            this.Repository.TaskProcessorRuntimeInfo.SetMaster(Guid.NewGuid());

            this.TaskProcessor.Start();

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            this.MessageBus.TaskProcessors.NotifyMasterModeChangeRequest(Guid.NewGuid(), true);

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            Assert.IsFalse(this.MasterCommandsProcessor.IsActive);

            this.MessageBus.Tasks.AssertNoMethodCall(mb => mb.UnsubscribeFromChannels(null));
            this.MessageBus.TaskProcessors.AssertNoMethodCall(mb => mb.NotifyMasterModeChanged(Guid.Empty, false, MasterModeChangeReason.None));
        }

        [TestMethod]
        public void MakeAnotherProcessorMasterWhenMaster()
        {
            this.TaskProcessor.Start();

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            this.MessageBus.TaskProcessors.RecordedMethodCalls.Clear();

            this.DateTimeProvider.UtcNow = Helpers.GetRandomDateTime();

            this.MessageBus.TaskProcessors.NotifyMasterModeChangeRequest(Guid.NewGuid(), true);

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            Assert.IsFalse(this.MasterCommandsProcessor.IsActive);
            Assert.IsFalse(this.MessageBus.MasterCommands.ReceiveMessages);

            this.MasterCommandsProcessor.AssertMethodCallOnceWithArguments(p => p.ProcessMasterCommands());

            this.MessageBus.Tasks.AssertMethodCallOnceWithArguments(mb => mb.UnsubscribeFromChannels(new ExpectedCollection<MessageBusChannel>(MessageBusChannel.TaskStarted)));
            this.MessageBus.TaskProcessors.AssertMethodCallOnceWithArguments(mb => mb.NotifyMasterModeChanged(this.TaskProcessor.Id, false, MasterModeChangeReason.Explicit));
        }

        [TestMethod]
        public void MakeMasterInactive()
        {
            this.MessageBus.TaskProcessors.NotifyMasterModeChangeRequest(this.TaskProcessor.Id, true);

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            Assert.IsFalse(this.MasterCommandsProcessor.IsActive);

            Assert.IsNull(this.Repository.TaskProcessorRuntimeInfo.GetMasterId());

            this.MasterCommandsProcessor.AssertNoMethodCall(p => p.ProcessMasterCommands());

            this.MessageBus.Tasks.AssertNoMethodCall(mb => mb.SubscribeForChannels(null));
            this.MessageBus.TaskProcessors.AssertNoMethodCall(mb => mb.NotifyMasterModeChanged(Guid.Empty, true, MasterModeChangeReason.None));
        }

        [TestMethod]
        public void MakeMasterStopping()
        {
            this.Repository.TaskProcessorRuntimeInfo.SetMaster(Guid.NewGuid());

            this.TaskProcessor.Start();

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            this.MakeTaskProcessorStopping();

            this.MessageBus.Tasks.RecordedMethodCalls.Clear();

            this.MessageBus.TaskProcessors.NotifyMasterModeChangeRequest(this.TaskProcessor.Id, true);

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            Assert.IsFalse(this.MasterCommandsProcessor.IsActive);

            this.MasterCommandsProcessor.AssertNoMethodCall(p => p.ProcessMasterCommands());

            this.MessageBus.Tasks.AssertNoMethodCall(mb => mb.SubscribeForChannels(null));
            this.MessageBus.TaskProcessors.AssertNoMethodCall(mb => mb.NotifyMasterModeChanged(Guid.Empty, true, MasterModeChangeReason.None));
        }

        [TestMethod]
        public void MakeMasterDisposedBeforeStart()
        {
            this.TaskProcessor.Dispose();

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            this.MessageBus.TaskProcessors.NotifyMasterModeChangeRequest(this.TaskProcessor.Id, true);

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            Assert.IsFalse(this.MasterCommandsProcessor.IsActive);

            this.MasterCommandsProcessor.AssertNoMethodCall(p => p.ProcessMasterCommands());

            this.MessageBus.Tasks.AssertNoMethodCall(mb => mb.SubscribeForChannels(null));
            this.MessageBus.TaskProcessors.AssertNoMethodCall(mb => mb.NotifyMasterModeChanged(Guid.Empty, true, MasterModeChangeReason.None));
        }

        [TestMethod]
        public void MakeMasterDisposedAfterStart()
        {
            this.TaskProcessor.Start();

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            this.TaskProcessor.Dispose();

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            this.MessageBus.Tasks.RecordedMethodCalls.Clear();
            this.MasterCommandsProcessor.RecordedMethodCalls.Clear();
            this.MessageBus.TaskProcessors.RecordedMethodCalls.Clear();

            this.MessageBus.TaskProcessors.NotifyMasterModeChangeRequest(this.TaskProcessor.Id, true);

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            Assert.IsFalse(this.MasterCommandsProcessor.IsActive);

            this.MasterCommandsProcessor.AssertNoMethodCall(p => p.ProcessMasterCommands());

            this.MessageBus.Tasks.AssertNoMethodCall(mb => mb.SubscribeForChannels(null));
            this.MessageBus.TaskProcessors.AssertNoMethodCall(mb => mb.NotifyMasterModeChanged(Guid.Empty, true, MasterModeChangeReason.None));
        }

        [TestMethod]
        public void BecomeMasterWhenOldMasterIsStopped()
        {
            this.Repository.TaskProcessorRuntimeInfo.SetMaster(Guid.NewGuid());

            this.TaskProcessor.Start();

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            this.DateTimeProvider.UtcNow = Helpers.GetRandomDateTime();

            this.Repository.TaskProcessorRuntimeInfo.ClearMaster();

            this.MessageBus.Tasks.RecordedMethodCalls.Clear();
            this.Repository.TaskProcessorRuntimeInfo.RecordedMethodCalls.Clear();

            this.MessageBus.TaskProcessors.NotifyMasterModeChanged(Guid.NewGuid(), false, MasterModeChangeReason.Stop);

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            Assert.IsTrue(this.MasterCommandsProcessor.IsActive);

            this.MasterCommandsProcessor.AssertMethodCallOnce(p => p.ProcessMasterCommands());

            this.Repository.TaskProcessorRuntimeInfo.AssertMethodCallOnceWithArguments(r => r.SetMasterIfNotExists(this.TaskProcessor.Id));

            Assert.IsTrue(this.MessageBus.MasterCommands.ReceiveMessages);

            this.MessageBus.Tasks.AssertMethodCallOnceWithArguments(mb => mb.SubscribeForChannels(new ExpectedCollection<MessageBusChannel>(MessageBusChannel.TaskStarted)));
            this.MessageBus.TaskProcessors.AssertMethodCallWithArguments(mb => mb.NotifyMasterModeChanged(this.TaskProcessor.Id, true, MasterModeChangeReason.Stop));
        }

        [TestMethod]
        public void DoNotBecomeMasterWhenOldMasterIsStoppedBecauseInactive()
        {
            this.Repository.TaskProcessorRuntimeInfo.SetMaster(Guid.NewGuid());

            this.DateTimeProvider.UtcNow = Helpers.GetRandomDateTime();

            this.Repository.TaskProcessorRuntimeInfo.ClearMaster();

            this.MessageBus.TaskProcessors.NotifyMasterModeChanged(Guid.NewGuid(), false, MasterModeChangeReason.Stop);

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            Assert.IsFalse(this.MasterCommandsProcessor.IsActive);

            this.MasterCommandsProcessor.AssertNoMethodCall(p => p.ProcessMasterCommands());

            this.Repository.TaskProcessorRuntimeInfo.AssertNoMethodCall(r => r.SetMasterIfNotExists(Guid.Empty));

            this.MessageBus.Tasks.AssertNoMethodCall(mb => mb.SubscribeForChannels(null));
            this.MessageBus.TaskProcessors.AssertNoMethodCallWithArguments(mb => mb.NotifyMasterModeChanged(this.TaskProcessor.Id, true, MasterModeChangeReason.Stop));
        }

        [TestMethod]
        public void BecomeMasterWhenOldMasterIsStoppedButStopping()
        {
            this.Repository.TaskProcessorRuntimeInfo.SetMaster(Guid.NewGuid());

            this.TaskProcessor.Start();

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            this.MakeTaskProcessorStopping();

            this.DateTimeProvider.UtcNow = Helpers.GetRandomDateTime();

            this.Repository.TaskProcessorRuntimeInfo.ClearMaster();

            this.Repository.TaskProcessorRuntimeInfo.RecordedMethodCalls.Clear();

            this.MessageBus.Tasks.RecordedMethodCalls.Clear();

            this.MessageBus.TaskProcessors.NotifyMasterModeChanged(Guid.NewGuid(), false, MasterModeChangeReason.Stop);

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            Assert.IsFalse(this.MasterCommandsProcessor.IsActive);

            this.MasterCommandsProcessor.AssertNoMethodCall(p => p.ProcessMasterCommands());

            this.Repository.TaskProcessorRuntimeInfo.AssertNoMethodCall(r => r.SetMasterIfNotExists(Guid.Empty));

            this.MessageBus.Tasks.AssertNoMethodCall(mb => mb.SubscribeForChannels(null));
            this.MessageBus.TaskProcessors.AssertNoMethodCallWithArguments(mb => mb.NotifyMasterModeChanged(this.TaskProcessor.Id, true, MasterModeChangeReason.Stop));
        }

        [TestMethod]
        public void BecomeMasterWhenOldMasterIsStoppedButDisposedBeforeStart()
        {
            this.Repository.TaskProcessorRuntimeInfo.SetMaster(Guid.NewGuid());

            this.TaskProcessor.Dispose();

            this.DateTimeProvider.UtcNow = Helpers.GetRandomDateTime();

            this.Repository.TaskProcessorRuntimeInfo.ClearMaster();

            this.MessageBus.TaskProcessors.NotifyMasterModeChanged(Guid.NewGuid(), false, MasterModeChangeReason.Stop);

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            Assert.IsFalse(this.MasterCommandsProcessor.IsActive);

            this.MasterCommandsProcessor.AssertNoMethodCall(p => p.ProcessMasterCommands());

            this.Repository.TaskProcessorRuntimeInfo.AssertNoMethodCall(r => r.SetMasterIfNotExists(Guid.Empty));

            this.MessageBus.Tasks.AssertNoMethodCall(mb => mb.SubscribeForChannels(null));
            this.MessageBus.TaskProcessors.AssertNoMethodCallWithArguments(mb => mb.NotifyMasterModeChanged(this.TaskProcessor.Id, true, MasterModeChangeReason.Stop));
        }

        [TestMethod]
        public void BecomeMasterWhenOldMasterIsStoppedButDisposedAfterStart()
        {
            this.Repository.TaskProcessorRuntimeInfo.SetMaster(Guid.NewGuid());

            this.TaskProcessor.Start();

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            this.TaskProcessor.Dispose();

            this.DateTimeProvider.UtcNow = Helpers.GetRandomDateTime();

            this.Repository.TaskProcessorRuntimeInfo.ClearMaster();

            this.MessageBus.Tasks.RecordedMethodCalls.Clear();
            this.Repository.TaskProcessorRuntimeInfo.RecordedMethodCalls.Clear();

            this.MessageBus.TaskProcessors.NotifyMasterModeChanged(Guid.NewGuid(), false, MasterModeChangeReason.Stop);

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            Assert.IsFalse(this.MasterCommandsProcessor.IsActive);

            this.MasterCommandsProcessor.AssertNoMethodCall(p => p.ProcessMasterCommands());

            this.Repository.TaskProcessorRuntimeInfo.AssertNoMethodCall(r => r.SetMasterIfNotExists(Guid.Empty));

            this.MessageBus.Tasks.AssertNoMethodCall(mb => mb.SubscribeForChannels(null));
            this.MessageBus.TaskProcessors.AssertNoMethodCallWithArguments(mb => mb.NotifyMasterModeChanged(this.TaskProcessor.Id, true, MasterModeChangeReason.Stop));
        }

        [TestMethod]
        public void DoNotBecomeMasterWhenOldMasterIsStoppedBecauseAlreadyMaster()
        {
            this.TaskProcessor.Start();

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            this.Repository.TaskProcessorRuntimeInfo.RecordedMethodCalls.Clear();

            this.MessageBus.TaskProcessors.NotifyMasterModeChanged(Guid.NewGuid(), false, MasterModeChangeReason.Stop);

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            this.Repository.TaskProcessorRuntimeInfo.AssertNoMethodCall(r => r.SetMasterIfNotExists(Guid.Empty));
        }

        [TestMethod]
        public void DoNotBecomeMasterWhenOldMasterIsStopped1()
        {
            this.Repository.TaskProcessorRuntimeInfo.SetMaster(Guid.NewGuid());

            this.TaskProcessor.Start();

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            this.Repository.TaskProcessorRuntimeInfo.RecordedMethodCalls.Clear();

            this.MessageBus.TaskProcessors.NotifyMasterModeChanged(Guid.NewGuid(), true, MasterModeChangeReason.Stop);

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            this.Repository.TaskProcessorRuntimeInfo.AssertNoMethodCall(r => r.SetMasterIfNotExists(Guid.Empty));
        }

        [TestMethod]
        public void DoNotBecomeMasterWhenOldMasterIsStoppedBecauseReasonExplicit()
        {
            this.Repository.TaskProcessorRuntimeInfo.SetMaster(Guid.NewGuid());

            this.TaskProcessor.Start();

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            this.Repository.TaskProcessorRuntimeInfo.RecordedMethodCalls.Clear();

            this.MessageBus.TaskProcessors.NotifyMasterModeChanged(Guid.NewGuid(), false, MasterModeChangeReason.Explicit);

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            this.Repository.TaskProcessorRuntimeInfo.AssertNoMethodCall(r => r.SetMasterIfNotExists(Guid.Empty));
        }

        [TestMethod]
        public void DoNotBecomeMasterWhenOldMasterIsStoppedBecauseReasonHeartbeat()
        {
            this.Repository.TaskProcessorRuntimeInfo.SetMaster(Guid.NewGuid());

            this.TaskProcessor.Start();

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            this.Repository.TaskProcessorRuntimeInfo.RecordedMethodCalls.Clear();

            this.MessageBus.TaskProcessors.NotifyMasterModeChanged(Guid.NewGuid(), false, MasterModeChangeReason.Heartbeat);

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            this.Repository.TaskProcessorRuntimeInfo.AssertNoMethodCall(r => r.SetMasterIfNotExists(Guid.Empty));
        }

        #endregion Change Master Mode

        #region Cancel Task

        [TestMethod]
        public void CancelTask()
        {
            this.TaskProcessor.Start();

            Guid taskId = this.TaskProcessorFacade.SubmitTask(new FakeTask());

            this.Repository.TaskRuntimeInfo.Assign(taskId, this.TaskProcessor.Id);

            this.TaskExecutor.PredefineMethodCall(e => e.StartTask(taskId, TaskPriority.Normal), () => Thread.Sleep(TimeSpan.FromSeconds(2)));

            ThreadPool.QueueUserWorkItem(_ => this.MessageBus.Tasks.NotifyTaskAssigned(taskId, this.TaskProcessor.Id));

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            Assert.AreEqual(1, this.TaskExecutor.ActiveTasksCount);

            this.DateTimeProvider.UtcNow = DateTime.UtcNow;

            this.MessageBus.Tasks.NotifyTaskCancelRequest(taskId, DateTime.UtcNow);

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            this.TaskExecutor.AssertMethodCallOnceWithArguments(te => te.CancelTask(taskId));

            this.Repository.TaskRuntimeInfo.AssertMethodCallOnceWithArguments(r => r.CompleteCancel(taskId, this.DateTimeProvider.UtcNow));
        }

        [TestMethod]
        public void CancelTaskMaster()
        {
            Guid taskId = this.Repository.TaskRuntimeInfo.Add().TaskId;

            this.TaskProcessor.Start();

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            this.MessageBus.Tasks.NotifyTaskSubmitted(taskId, DateTime.UtcNow, false);

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            this.MessageBus.Tasks.NotifyTaskCancelRequest(taskId, DateTime.UtcNow);

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            this.MasterCommandsProcessor.AssertMethodCallOnceWithArguments(p => p.CancelTask(taskId));
        }

        #endregion Cancel Task

        #region Polling Jobs

        [TestMethod]
        public void DoNotStartInactivePollingJob()
        {
            FakeTaskProcessorConfiguration configuration = new FakeTaskProcessorConfiguration();

            configuration.PollingJobs.Add(typeof(FakePollingJob));

            configuration.PollingJobs[typeof(FakePollingJob)].IsActive = false;

            this.ConfigurationProvider.PredefineResult(configuration, p => p.GetTaskProcessorConfiguration());

            this.TaskProcessor.Start();

            Thread.Sleep(TimeSpan.FromSeconds(1));

            Assert.IsFalse(FakePollingJob.CreatedFakePollingJobsInThread.Any());
        }

        [TestMethod]
        public void DoNotStartMasterPollingJobWhenSlave()
        {
            FakeTaskProcessorConfiguration configuration = new FakeTaskProcessorConfiguration();

            configuration.PollingJobs.Add(typeof(FakePollingJob));

            configuration.PollingJobs[typeof(FakePollingJob)].IsActive = true;
            configuration.PollingJobs[typeof(FakePollingJob)].IsMaster = true;

            this.ConfigurationProvider.PredefineResult(configuration, p => p.GetTaskProcessorConfiguration());

            this.Repository.TaskProcessorRuntimeInfo.SetMaster(Guid.NewGuid());

            this.TaskProcessor.Start();

            Thread.Sleep(TimeSpan.FromSeconds(1));

            Assert.IsFalse(FakePollingJob.CreatedFakePollingJobsInThread.Any());
        }

        [TestMethod]
        public void StartMasterPollingJob()
        {
            this.StartPollingJob(true);
        }

        [TestMethod]
        public void StartSlavePollingJob()
        {
            this.StartPollingJob(false);
        }

        [TestMethod]
        public void ProcessPollingJob()
        {
            FakeTaskProcessorConfiguration configuration = new FakeTaskProcessorConfiguration();

            configuration.PollingJobs.Add(typeof(FakePollingJob));

            configuration.PollingJobs[typeof(FakePollingJob)].PollInterval = TimeSpan.FromSeconds(1);
            configuration.PollingJobs[typeof(FakePollingJob)].IsActive = true;

            this.ConfigurationProvider.PredefineResult(configuration, p => p.GetTaskProcessorConfiguration());

            using (FakeTimer timer = new FakeTimer())
            {
                this.ServiceLocator.PredefineResult(timer, l => l.ResolveSingle(typeof(ITimer)));

                this.TaskProcessor.Start();

                FakePollingJob createdPollingJob = FakePollingJob.CreatedFakePollingJobsInThread.Single();

                timer.RaiseTick();

                createdPollingJob.AssertMethodCallOnceWithArguments(q => q.Process());
            }
        }

        [TestMethod]
        public void ConcurrentPollingJob()
        {
            FakeTaskProcessorConfiguration configuration = new FakeTaskProcessorConfiguration();

            configuration.PollingJobs.Add(typeof(FakePollingJob));

            configuration.PollingJobs[typeof(FakePollingJob)].PollInterval = TimeSpan.FromSeconds(1);
            configuration.PollingJobs[typeof(FakePollingJob)].IsActive = true;
            configuration.PollingJobs[typeof(FakePollingJob)].IsConcurrent = true;

            this.ConfigurationProvider.PredefineResult(configuration, p => p.GetTaskProcessorConfiguration());

            using (FakeTimer timer = new FakeTimer())
            {
                this.ServiceLocator.PredefineResult(timer, l => l.ResolveSingle(typeof(ITimer)));

                this.TaskProcessor.Start();

                FakePollingJob createdPollingJob = FakePollingJob.CreatedFakePollingJobsInThread.Single();

                createdPollingJob.PredefineMethodCall(q => q.Process(), () => Thread.Sleep(TimeSpan.FromSeconds(2)));

                timer.RaiseTickInAnotherThread();

                Thread.Sleep(TimeSpan.FromSeconds(1));

                timer.RaiseTickInAnotherThread();

                Thread.Sleep(TimeSpan.FromSeconds(1));

                createdPollingJob.AssertMethodCallWithArguments(q => q.Process());
                createdPollingJob.AssertMethodCallWithArguments(q => q.Process());
                createdPollingJob.AssertNoMethodCallWithArguments(q => q.Process());
            }
        }

        [TestMethod]
        public void NotConcurrentPollingJob()
        {
            FakeTaskProcessorConfiguration configuration = new FakeTaskProcessorConfiguration();

            configuration.PollingJobs.Add(typeof(FakePollingJob));

            configuration.PollingJobs[typeof(FakePollingJob)].PollInterval = TimeSpan.FromSeconds(1);
            configuration.PollingJobs[typeof(FakePollingJob)].IsActive = true;

            this.ConfigurationProvider.PredefineResult(configuration, p => p.GetTaskProcessorConfiguration());

            using (FakeTimer timer = new FakeTimer())
            {
                this.ServiceLocator.PredefineResult(timer, l => l.ResolveSingle(typeof(ITimer)));

                this.TaskProcessor.Start();

                FakePollingJob createdPollingJob = FakePollingJob.CreatedFakePollingJobsInThread.Single();

                createdPollingJob.PredefineMethodCall(q => q.Process(), () => Thread.Sleep(TimeSpan.FromSeconds(2)));

                timer.RaiseTickInAnotherThread();

                Thread.Sleep(TimeSpan.FromSeconds(1));

                timer.RaiseTickInAnotherThread();

                createdPollingJob.AssertMethodCallOnceWithArguments(q => q.Process());
            }
        }

        [TestMethod]
        public void StartPollingJobOnConfigurationChanged()
        {
            using (FakeTimer timer = new FakeTimer())
            {
                this.ServiceLocator.PredefineResult(timer, l => l.ResolveSingle(typeof(ITimer)));

                this.TaskProcessor.Start();

                ITaskProcessorRuntimeInfo processorInfo = this.Repository.TaskProcessorRuntimeInfo.GetById(this.TaskProcessor.Id);

                processorInfo.Configuration.PollingJobs.Add(typeof(FakePollingJob));

                processorInfo.Configuration.PollingJobs[typeof(FakePollingJob)].PollInterval = TimeSpan.FromSeconds(1);
                processorInfo.Configuration.PollingJobs[typeof(FakePollingJob)].IsActive = true;

                this.Repository.TaskProcessorRuntimeInfo.Update(processorInfo);

                this.MessageBus.TaskProcessors.NotifyConfigurationChanged(this.TaskProcessor.Id);

                Thread.Sleep(TimeSpan.FromSeconds(2));

                Assert.AreEqual(TimeSpan.FromSeconds(1), timer.Interval);

                Assert.IsTrue(timer.IsActive);
            }

            Assert.AreEqual(1, FakePollingJob.CreatedFakePollingJobsInThread.Count());
        }

        [TestMethod]
        public void DoNotStartInactivePollingJobOnConfigurationChanged()
        {
            this.TaskProcessor.Start();

            ITaskProcessorRuntimeInfo processorInfo = this.Repository.TaskProcessorRuntimeInfo.GetById(this.TaskProcessor.Id);

            processorInfo.Configuration.PollingJobs.Add(typeof(FakePollingJob));

            processorInfo.Configuration.PollingJobs[typeof(FakePollingJob)].PollInterval = TimeSpan.FromSeconds(1);
            processorInfo.Configuration.PollingJobs[typeof(FakePollingJob)].IsActive = false;

            this.Repository.TaskProcessorRuntimeInfo.Update(processorInfo);

            this.ServiceLocator.RecordedMethodCalls.Clear();

            this.MessageBus.TaskProcessors.NotifyConfigurationChanged(this.TaskProcessor.Id);

            Thread.Sleep(TimeSpan.FromSeconds(2));

            this.ServiceLocator.AssertNoMethodCallWithArguments(l => l.ResolveSingle(typeof(ITimer)));

            Assert.IsFalse(FakePollingJob.CreatedFakePollingJobsInThread.Any());
        }

        [TestMethod]
        public void DoNotStartMasterPollingJobOnConfigurationChanged()
        {
            this.Repository.TaskProcessorRuntimeInfo.SetMaster(Guid.NewGuid());

            this.TaskProcessor.Start();

            ITaskProcessorRuntimeInfo processorInfo = this.Repository.TaskProcessorRuntimeInfo.GetById(this.TaskProcessor.Id);

            processorInfo.Configuration.PollingJobs.Add(typeof(FakePollingJob));

            processorInfo.Configuration.PollingJobs[typeof(FakePollingJob)].PollInterval = TimeSpan.FromSeconds(1);
            processorInfo.Configuration.PollingJobs[typeof(FakePollingJob)].IsActive = true;
            processorInfo.Configuration.PollingJobs[typeof(FakePollingJob)].IsMaster = true;

            this.Repository.TaskProcessorRuntimeInfo.Update(processorInfo);

            this.ServiceLocator.RecordedMethodCalls.Clear();

            this.MessageBus.TaskProcessors.NotifyConfigurationChanged(this.TaskProcessor.Id);

            Thread.Sleep(TimeSpan.FromSeconds(2));

            this.ServiceLocator.AssertNoMethodCallWithArguments(l => l.ResolveSingle(typeof(ITimer)));

            Assert.IsFalse(FakePollingJob.CreatedFakePollingJobsInThread.Any());
        }

        [TestMethod]
        public void StopPollingJobOnConfigurationChanged()
        {
            FakeTaskProcessorConfiguration configuration = new FakeTaskProcessorConfiguration();

            configuration.PollingJobs.Add(typeof(FakePollingJob));

            configuration.PollingJobs[typeof(FakePollingJob)].PollInterval = TimeSpan.FromSeconds(1);
            configuration.PollingJobs[typeof(FakePollingJob)].IsActive = true;

            this.ConfigurationProvider.PredefineResult(configuration, p => p.GetTaskProcessorConfiguration());

            using (FakeTimer timer = new FakeTimer())
            {
                this.ServiceLocator.PredefineResult(timer, l => l.ResolveSingle(typeof(ITimer)));

                this.TaskProcessor.Start();

                Thread.Sleep(TimeSpan.FromSeconds(0.5));

                ITaskProcessorRuntimeInfo processorInfo = this.Repository.TaskProcessorRuntimeInfo.GetById(this.TaskProcessor.Id);

                processorInfo.Configuration.PollingJobs[typeof(FakePollingJob)].IsActive = false;

                this.Repository.TaskProcessorRuntimeInfo.Update(processorInfo);

                this.MessageBus.TaskProcessors.NotifyConfigurationChanged(this.TaskProcessor.Id);

                Thread.Sleep(TimeSpan.FromSeconds(2));

                timer.AssertMethodCallOnce(t => t.Dispose());
            }

            FakePollingJob createdPollingJob = FakePollingJob.CreatedFakePollingJobsInThread.Single();

            createdPollingJob.AssertNoMethodCallWithArguments(q => q.Process());

            createdPollingJob.AssertMethodCallOnce(p => p.Dispose());
        }

        [TestMethod]
        public void StopMasterPollingJobOnConfigurationChanged()
        {
            FakeTaskProcessorConfiguration configuration = new FakeTaskProcessorConfiguration();

            configuration.PollingJobs.Add(typeof(FakePollingJob));

            configuration.PollingJobs[typeof(FakePollingJob)].PollInterval = TimeSpan.FromSeconds(1);
            configuration.PollingJobs[typeof(FakePollingJob)].IsActive = true;

            this.ConfigurationProvider.PredefineResult(configuration, p => p.GetTaskProcessorConfiguration());

            this.Repository.TaskProcessorRuntimeInfo.SetMaster(Guid.NewGuid());

            using (FakeTimer timer = new FakeTimer())
            {
                this.ServiceLocator.PredefineResult(timer, l => l.ResolveSingle(typeof(ITimer)));

                this.TaskProcessor.Start();

                Thread.Sleep(TimeSpan.FromSeconds(0.5));

                ITaskProcessorRuntimeInfo processorInfo = this.Repository.TaskProcessorRuntimeInfo.GetById(this.TaskProcessor.Id);

                processorInfo.Configuration.PollingJobs[typeof(FakePollingJob)].IsMaster = true;

                this.Repository.TaskProcessorRuntimeInfo.Update(processorInfo);

                this.MessageBus.TaskProcessors.NotifyConfigurationChanged(this.TaskProcessor.Id);

                Thread.Sleep(TimeSpan.FromSeconds(2));

                timer.AssertMethodCallOnce(t => t.Dispose());
            }

            FakePollingJob createdPollingJob = FakePollingJob.CreatedFakePollingJobsInThread.Single();

            createdPollingJob.AssertNoMethodCallWithArguments(q => q.Process());

            createdPollingJob.AssertMethodCallOnce(p => p.Dispose());
        }

        [TestMethod]
        public void StartMasterPollingJobOnBecameMaster()
        {
            FakeTaskProcessorConfiguration configuration = new FakeTaskProcessorConfiguration();

            configuration.PollingJobs.Add(typeof(FakePollingJob));

            configuration.PollingJobs[typeof(FakePollingJob)].PollInterval = TimeSpan.FromSeconds(1);
            configuration.PollingJobs[typeof(FakePollingJob)].IsActive = true;
            configuration.PollingJobs[typeof(FakePollingJob)].IsMaster = true;

            this.ConfigurationProvider.PredefineResult(configuration, p => p.GetTaskProcessorConfiguration());

            this.Repository.TaskProcessorRuntimeInfo.SetMaster(Guid.NewGuid());

            this.TaskProcessor.Start();

            this.Repository.TaskProcessorRuntimeInfo.ClearMaster();

            using (FakeTimer timer = new FakeTimer())
            {
                this.ServiceLocator.PredefineResult(timer, l => l.ResolveSingle(typeof(ITimer)));

                this.HeartbeatTimer.RaiseTick();

                Assert.AreEqual(TimeSpan.FromSeconds(1), timer.Interval);

                Assert.IsTrue(timer.IsActive);
            }

            Assert.AreEqual(1, FakePollingJob.CreatedFakePollingJobsInThread.Count());
        }

        [TestMethod]
        public void StopMasterPollingJobOnBecameSlave()
        {
            FakeTaskProcessorConfiguration configuration = new FakeTaskProcessorConfiguration();

            configuration.PollingJobs.Add(typeof(FakePollingJob));

            configuration.PollingJobs[typeof(FakePollingJob)].PollInterval = TimeSpan.FromSeconds(1);
            configuration.PollingJobs[typeof(FakePollingJob)].IsActive = true;
            configuration.PollingJobs[typeof(FakePollingJob)].IsMaster = true;

            this.ConfigurationProvider.PredefineResult(configuration, p => p.GetTaskProcessorConfiguration());

            using (FakeTimer timer = new FakeTimer())
            {
                this.ServiceLocator.PredefineResult(timer, l => l.ResolveSingle(typeof(ITimer)));

                this.TaskProcessor.Start();

                Thread.Sleep(TimeSpan.FromSeconds(0.5));

                this.Repository.TaskProcessorRuntimeInfo.SetMaster(Guid.NewGuid());

                this.HeartbeatTimer.RaiseTick();

                Thread.Sleep(TimeSpan.FromSeconds(0.5));

                timer.AssertMethodCallOnce(t => t.Dispose());
            }

            FakePollingJob createdPollingJob = FakePollingJob.CreatedFakePollingJobsInThread.Single();

            createdPollingJob.AssertNoMethodCallWithArguments(q => q.Process());

            createdPollingJob.AssertMethodCallOnce(p => p.Dispose());
        }

        [TestMethod]
        public void StopPollingJobWithProcessor()
        {
            FakeTaskProcessorConfiguration configuration = new FakeTaskProcessorConfiguration();

            configuration.PollingJobs.Add(typeof(FakePollingJob));

            configuration.PollingJobs[typeof(FakePollingJob)].PollInterval = TimeSpan.FromSeconds(1);
            configuration.PollingJobs[typeof(FakePollingJob)].IsActive = true;

            this.ConfigurationProvider.PredefineResult(configuration, p => p.GetTaskProcessorConfiguration());

            using (FakeTimer timer = new FakeTimer())
            {
                this.ServiceLocator.PredefineResult(timer, l => l.ResolveSingle(typeof(ITimer)));

                this.TaskProcessor.Start();

                Thread.Sleep(TimeSpan.FromSeconds(0.5));

                this.MessageBus.TaskProcessors.NotifyStopRequested(this.TaskProcessor.Id);

                timer.AssertMethodCallOnce(t => t.Dispose());
            }

            FakePollingJob createdPollingJob = FakePollingJob.CreatedFakePollingJobsInThread.Single();

            createdPollingJob.AssertNoMethodCallWithArguments(q => q.Process());

            createdPollingJob.AssertMethodCallOnce(p => p.Dispose());
        }

        [TestMethod]
        public void StopPollingJobOnDispose()
        {
            FakeTaskProcessorConfiguration configuration = new FakeTaskProcessorConfiguration();

            configuration.PollingJobs.Add(typeof(FakePollingJob));

            configuration.PollingJobs[typeof(FakePollingJob)].PollInterval = TimeSpan.FromSeconds(1);
            configuration.PollingJobs[typeof(FakePollingJob)].IsActive = true;

            this.ConfigurationProvider.PredefineResult(configuration, p => p.GetTaskProcessorConfiguration());

            using (FakeTimer timer = new FakeTimer())
            {
                this.ServiceLocator.PredefineResult(timer, l => l.ResolveSingle(typeof(ITimer)));

                this.TaskProcessor.Start();

                Thread.Sleep(TimeSpan.FromSeconds(10));

                this.TaskProcessor.Dispose();

                timer.AssertMethodCallOnce(t => t.Dispose());
            }

            FakePollingJob createdPollingJob = FakePollingJob.CreatedFakePollingJobsInThread.Single();

            createdPollingJob.AssertNoMethodCallWithArguments(q => q.Process());

            createdPollingJob.AssertMethodCallOnce(p => p.Dispose());
        }

        #endregion Polling Jobs

        #region Polling Queues

        [TestMethod]
        public void DoNotStartInactivePollingQueue()
        {
            FakeTaskProcessorConfiguration configuration = new FakeTaskProcessorConfiguration();

            configuration.PollingQueues.Add("Test");

            configuration.PollingQueues["Test"].IsActive = false;

            this.ConfigurationProvider.PredefineResult(configuration, p => p.GetTaskProcessorConfiguration());

            this.TaskProcessor.Start();

            Thread.Sleep(TimeSpan.FromSeconds(1));

            this.ServiceLocator.AssertMethodCallsCount(2, l => l.ResolveSingle(typeof(ITimer)));
        }

        [TestMethod]
        public void DoNotStartMasterPollingQueueWhenSlave()
        {
            FakeTaskProcessorConfiguration configuration = new FakeTaskProcessorConfiguration();

            configuration.PollingQueues.Add("Test");

            configuration.PollingQueues["Test"].IsActive = true;
            configuration.PollingQueues["Test"].IsMaster = true;

            this.ConfigurationProvider.PredefineResult(configuration, p => p.GetTaskProcessorConfiguration());

            this.Repository.TaskProcessorRuntimeInfo.SetMaster(Guid.NewGuid());

            this.ServiceLocator.RecordedMethodCalls.Clear();

            this.TaskProcessor.Start();

            Thread.Sleep(TimeSpan.FromSeconds(1));

            this.ServiceLocator.AssertNoMethodCallWithArguments(l => l.ResolveSingle(typeof(ITimer)));
        }

        [TestMethod]
        public void StartMasterPollingQueue()
        {
            this.StartPollingQueue(true);
        }

        [TestMethod]
        public void StartSlavePollingQueue()
        {
            this.StartPollingQueue(false);
        }

        [TestMethod]
        public void ProcessPollingQueue()
        {
            FakeTaskProcessorConfiguration configuration = new FakeTaskProcessorConfiguration();

            configuration.PollingQueues.Add("Test");

            configuration.PollingQueues["Test"].PollInterval = TimeSpan.FromSeconds(1);
            configuration.PollingQueues["Test"].IsActive = true;
            configuration.PollingQueues["Test"].MaxWorkers = 2;

            this.ConfigurationProvider.PredefineResult(configuration, p => p.GetTaskProcessorConfiguration());

            using (FakeTimer timer = new FakeTimer())
            {
                this.ServiceLocator.PredefineResult(timer, l => l.ResolveSingle(typeof(ITimer)));

                this.TaskProcessor.Start();

                List<FakeTaskRuntimeInfo> taskInfos = new List<FakeTaskRuntimeInfo>();

                for (int i = 0; i < 2 * configuration.PollingQueues["Test"].MaxWorkers; i++)
                {
                    FakeTaskRuntimeInfo taskInfo = this.Repository.TaskRuntimeInfo.Add<FakeTaskRuntimeInfo>();

                    taskInfo.PollingQueue = "Test";

                    taskInfos.Add(taskInfo);
                }

                taskInfos[0].Priority = TaskPriority.VeryHigh;
                taskInfos[1].Priority = TaskPriority.High;

                this.DateTimeProvider.UtcNow = DateTime.UtcNow;

                timer.RaiseTick();

                this.Repository.TaskRuntimeInfo.AssertMethodCallOnceWithArguments(r => r.ReservePollingQueueTasks("Test", 2));

                for (int i = 0; i < configuration.PollingQueues["Test"].MaxWorkers; i++)
                {
                    this.TaskExecutor.AssertMethodCallWithArguments(e => e.StartTask(taskInfos[i].TaskId, taskInfos[i].Priority));

                    this.Repository.TaskRuntimeInfo.AssertMethodCallWithArguments(r => r.Start(taskInfos[i].TaskId, this.TaskProcessor.Id, this.DateTimeProvider.UtcNow));
                }

                timer.RaiseTick();
            }

            this.TaskExecutor.AssertNoMethodCall(e => e.StartTask(Guid.Empty, TaskPriority.Normal));
            this.Repository.TaskRuntimeInfo.AssertNoMethodCall(r => r.Start(Guid.Empty, Guid.Empty, DateTime.MinValue));
        }

        [TestMethod]
        public void ProcessPollingQueueNoFreeTaskSlots()
        {
            FakeTaskProcessorConfiguration configuration = new FakeTaskProcessorConfiguration();

            configuration.PollingQueues.Add("Test");

            configuration.PollingQueues["Test"].PollInterval = TimeSpan.FromSeconds(1);
            configuration.PollingQueues["Test"].IsActive = true;
            configuration.PollingQueues["Test"].MaxWorkers = 0;

            this.ConfigurationProvider.PredefineResult(configuration, p => p.GetTaskProcessorConfiguration());

            using (FakeTimer timer = new FakeTimer())
            {
                this.ServiceLocator.PredefineResult(timer, l => l.ResolveSingle(typeof(ITimer)));

                this.TaskProcessor.Start();

                timer.RaiseTick();
            }

            this.Repository.TaskRuntimeInfo.AssertNoMethodCall(r => r.ReservePollingQueueTasks(null, 0));
        }

        [TestMethod]
        public void ConcurrentPollingQueue()
        {
            FakeTaskProcessorConfiguration configuration = new FakeTaskProcessorConfiguration();

            configuration.PollingQueues.Add("Test");

            configuration.PollingQueues["Test"].PollInterval = TimeSpan.FromSeconds(1);
            configuration.PollingQueues["Test"].IsActive = true;
            configuration.PollingQueues["Test"].IsConcurrent = true;
            configuration.PollingQueues["Test"].MaxWorkers = 10;

            this.ConfigurationProvider.PredefineResult(configuration, p => p.GetTaskProcessorConfiguration());

            using (FakeTimer timer = new FakeTimer())
            {
                this.ServiceLocator.PredefineResult(timer, l => l.ResolveSingle(typeof(ITimer)));

                this.TaskProcessor.Start();

                timer.RaiseTickInAnotherThread();

                Thread.Sleep(TimeSpan.FromSeconds(1));

                timer.RaiseTickInAnotherThread();
            }

            Thread.Sleep(TimeSpan.FromSeconds(1));

            this.Repository.TaskRuntimeInfo.AssertMethodCallsCount(2, r => r.ReservePollingQueueTasks(null, 0));
        }

        [TestMethod]
        public void NotConcurrentPollingQueue()
        {
            FakeTaskProcessorConfiguration configuration = new FakeTaskProcessorConfiguration();

            configuration.PollingQueues.Add("Test");

            configuration.PollingQueues["Test"].PollInterval = TimeSpan.FromSeconds(1);
            configuration.PollingQueues["Test"].IsActive = true;
            configuration.PollingQueues["Test"].MaxWorkers = 1;

            this.ConfigurationProvider.PredefineResult(configuration, p => p.GetTaskProcessorConfiguration());

            using (FakeTimer timer = new FakeTimer())
            {
                this.ServiceLocator.PredefineResult(timer, l => l.ResolveSingle(typeof(ITimer)));

                this.TaskProcessor.Start();

                this.Repository.TaskRuntimeInfo.PredefineMethodCall(r => r.ReservePollingQueueTasks("Test", 1), () => Thread.Sleep(TimeSpan.FromSeconds(1)));

                timer.RaiseTickInAnotherThread();

                Thread.Sleep(TimeSpan.FromSeconds(0.5));

                timer.RaiseTickInAnotherThread();
            }

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            this.Repository.TaskRuntimeInfo.AssertMethodCallOnce(r => r.ReservePollingQueueTasks(null, 0));
        }

        [TestMethod]
        public void StartPollingQueueOnConfigurationChanged()
        {
            this.TaskProcessor.Start();

            ITaskProcessorRuntimeInfo processorInfo = this.Repository.TaskProcessorRuntimeInfo.GetById(this.TaskProcessor.Id);

            processorInfo.Configuration.PollingQueues.Add("Test");

            processorInfo.Configuration.PollingQueues["Test"].PollInterval = TimeSpan.FromSeconds(1);
            processorInfo.Configuration.PollingQueues["Test"].IsActive = true;

            this.Repository.TaskProcessorRuntimeInfo.Update(processorInfo);

            using (FakeTimer timer = new FakeTimer())
            {
                this.ServiceLocator.PredefineResult(timer, l => l.ResolveSingle(typeof(ITimer)));

                this.MessageBus.TaskProcessors.NotifyConfigurationChanged(this.TaskProcessor.Id);

                Thread.Sleep(TimeSpan.FromSeconds(2));

                Assert.AreEqual(TimeSpan.FromSeconds(1), timer.Interval);

                Assert.IsTrue(timer.IsActive);
            }
        }

        [TestMethod]
        public void DoNotStartInactivePollingQueueOnConfigurationChanged()
        {
            this.TaskProcessor.Start();

            ITaskProcessorRuntimeInfo processorInfo = this.Repository.TaskProcessorRuntimeInfo.GetById(this.TaskProcessor.Id);

            processorInfo.Configuration.PollingQueues.Add("Test");

            processorInfo.Configuration.PollingQueues["Test"].PollInterval = TimeSpan.FromSeconds(1);
            processorInfo.Configuration.PollingQueues["Test"].IsActive = false;

            this.Repository.TaskProcessorRuntimeInfo.Update(processorInfo);

            this.ServiceLocator.RecordedMethodCalls.Clear();

            this.MessageBus.TaskProcessors.NotifyConfigurationChanged(this.TaskProcessor.Id);

            Thread.Sleep(TimeSpan.FromSeconds(2));

            this.ServiceLocator.AssertNoMethodCallWithArguments(l => l.ResolveSingle(typeof(ITimer)));
        }

        [TestMethod]
        public void DoNotStartMasterPollingQueueOnConfigurationChanged()
        {
            this.Repository.TaskProcessorRuntimeInfo.SetMaster(Guid.NewGuid());

            this.TaskProcessor.Start();

            ITaskProcessorRuntimeInfo processorInfo = this.Repository.TaskProcessorRuntimeInfo.GetById(this.TaskProcessor.Id);

            processorInfo.Configuration.PollingQueues.Add("Test");

            processorInfo.Configuration.PollingQueues["Test"].PollInterval = TimeSpan.FromSeconds(1);
            processorInfo.Configuration.PollingQueues["Test"].IsActive = true;
            processorInfo.Configuration.PollingQueues["Test"].IsMaster = true;

            this.Repository.TaskProcessorRuntimeInfo.Update(processorInfo);

            this.ServiceLocator.RecordedMethodCalls.Clear();

            this.MessageBus.TaskProcessors.NotifyConfigurationChanged(this.TaskProcessor.Id);

            Thread.Sleep(TimeSpan.FromSeconds(2));

            this.ServiceLocator.AssertNoMethodCallWithArguments(l => l.ResolveSingle(typeof(ITimer)));
        }

        [TestMethod]
        public void StopPollingQueueOnConfigurationChanged()
        {
            FakeTaskProcessorConfiguration configuration = new FakeTaskProcessorConfiguration();

            configuration.PollingQueues.Add("Test");

            configuration.PollingQueues["Test"].PollInterval = TimeSpan.FromSeconds(1);
            configuration.PollingQueues["Test"].IsActive = true;

            this.ConfigurationProvider.PredefineResult(configuration, p => p.GetTaskProcessorConfiguration());

            using (FakeTimer timer = new FakeTimer())
            {
                this.ServiceLocator.PredefineResult(timer, l => l.ResolveSingle(typeof(ITimer)));

                this.TaskProcessor.Start();

                Thread.Sleep(TimeSpan.FromSeconds(0.5));

                ITaskProcessorRuntimeInfo processorInfo = this.Repository.TaskProcessorRuntimeInfo.GetById(this.TaskProcessor.Id);

                processorInfo.Configuration.PollingQueues["Test"].IsActive = false;

                this.Repository.TaskProcessorRuntimeInfo.Update(processorInfo);

                this.MessageBus.TaskProcessors.NotifyConfigurationChanged(this.TaskProcessor.Id);

                Thread.Sleep(TimeSpan.FromSeconds(2));

                timer.AssertMethodCallOnce(t => t.Dispose());
            }
        }

        [TestMethod]
        public void StopMasterPollingQueueOnConfigurationChanged()
        {
            FakeTaskProcessorConfiguration configuration = new FakeTaskProcessorConfiguration();

            configuration.PollingQueues.Add("Test");

            configuration.PollingQueues["Test"].PollInterval = TimeSpan.FromSeconds(1);
            configuration.PollingQueues["Test"].IsActive = true;

            this.ConfigurationProvider.PredefineResult(configuration, p => p.GetTaskProcessorConfiguration());

            this.Repository.TaskProcessorRuntimeInfo.SetMaster(Guid.NewGuid());

            using (FakeTimer timer = new FakeTimer())
            {
                this.ServiceLocator.PredefineResult(timer, l => l.ResolveSingle(typeof(ITimer)));

                this.TaskProcessor.Start();

                Thread.Sleep(TimeSpan.FromSeconds(0.5));

                ITaskProcessorRuntimeInfo processorInfo = this.Repository.TaskProcessorRuntimeInfo.GetById(this.TaskProcessor.Id);

                processorInfo.Configuration.PollingQueues["Test"].IsMaster = true;

                this.Repository.TaskProcessorRuntimeInfo.Update(processorInfo);

                this.MessageBus.TaskProcessors.NotifyConfigurationChanged(this.TaskProcessor.Id);

                Thread.Sleep(TimeSpan.FromSeconds(2));

                timer.AssertMethodCallOnce(t => t.Dispose());
            }
        }

        [TestMethod]
        public void StartMasterPollingQueueOnBecameMaster()
        {
            FakeTaskProcessorConfiguration configuration = new FakeTaskProcessorConfiguration();

            configuration.PollingQueues.Add("Test");

            configuration.PollingQueues["Test"].PollInterval = TimeSpan.FromSeconds(1);
            configuration.PollingQueues["Test"].IsActive = true;
            configuration.PollingQueues["Test"].IsMaster = true;

            this.ConfigurationProvider.PredefineResult(configuration, p => p.GetTaskProcessorConfiguration());

            this.Repository.TaskProcessorRuntimeInfo.SetMaster(Guid.NewGuid());

            using (FakeTimer timer = new FakeTimer())
            {
                this.ServiceLocator.PredefineResult(timer, l => l.ResolveSingle(typeof(ITimer)));

                this.TaskProcessor.Start();

                this.Repository.TaskProcessorRuntimeInfo.ClearMaster();

                this.HeartbeatTimer.RaiseTick();

                Assert.AreEqual(TimeSpan.FromSeconds(1), timer.Interval);

                Assert.IsTrue(timer.IsActive);
            }
        }

        [TestMethod]
        public void StopMasterPollingQueueOnBecameSlave()
        {
            FakeTaskProcessorConfiguration configuration = new FakeTaskProcessorConfiguration();

            configuration.PollingQueues.Add("Test");

            configuration.PollingQueues["Test"].PollInterval = TimeSpan.FromSeconds(1);
            configuration.PollingQueues["Test"].IsActive = true;
            configuration.PollingQueues["Test"].IsMaster = true;

            this.ConfigurationProvider.PredefineResult(configuration, p => p.GetTaskProcessorConfiguration());

            using (FakeTimer timer = new FakeTimer())
            {
                this.ServiceLocator.PredefineResult(timer, l => l.ResolveSingle(typeof(ITimer)));

                this.TaskProcessor.Start();

                Thread.Sleep(TimeSpan.FromSeconds(0.5));

                this.Repository.TaskProcessorRuntimeInfo.SetMaster(Guid.NewGuid());

                this.HeartbeatTimer.RaiseTick();

                Thread.Sleep(TimeSpan.FromSeconds(0.5));

                timer.AssertMethodCallOnce(t => t.Dispose());
            }
        }

        [TestMethod]
        public void StopPollingQueueWithProcessor()
        {
            FakeTaskProcessorConfiguration configuration = new FakeTaskProcessorConfiguration();

            configuration.PollingQueues.Add("Test");

            configuration.PollingQueues["Test"].PollInterval = TimeSpan.FromSeconds(1);
            configuration.PollingQueues["Test"].IsActive = true;

            this.ConfigurationProvider.PredefineResult(configuration, p => p.GetTaskProcessorConfiguration());

            using (FakeTimer timer = new FakeTimer())
            {
                this.ServiceLocator.PredefineResult(timer, l => l.ResolveSingle(typeof(ITimer)));

                this.TaskProcessor.Start();

                Thread.Sleep(TimeSpan.FromSeconds(0.5));

                this.MessageBus.TaskProcessors.NotifyStopRequested(this.TaskProcessor.Id);

                timer.AssertMethodCallOnce(t => t.Dispose());
            }
        }

        [TestMethod]
        public void StopPollingQueueOnDispose()
        {
            FakeTaskProcessorConfiguration configuration = new FakeTaskProcessorConfiguration();

            configuration.PollingQueues.Add("Test");

            configuration.PollingQueues["Test"].PollInterval = TimeSpan.FromSeconds(1);
            configuration.PollingQueues["Test"].IsActive = true;

            this.ConfigurationProvider.PredefineResult(configuration, p => p.GetTaskProcessorConfiguration());

            using (FakeTimer timer = new FakeTimer())
            {
                this.ServiceLocator.PredefineResult(timer, l => l.ResolveSingle(typeof(ITimer)));

                this.TaskProcessor.Start();

                Thread.Sleep(TimeSpan.FromSeconds(10));

                this.TaskProcessor.Dispose();

                timer.AssertMethodCallOnce(t => t.Dispose());
            }
        }

        [TestMethod]
        public void ResetPollingQueueIntervalOnConfigurationChanged()
        {
            FakeTaskProcessorConfiguration configuration = new FakeTaskProcessorConfiguration();

            configuration.PollingQueues.Add("Test");

            configuration.PollingQueues["Test"].PollInterval = TimeSpan.FromSeconds(1);
            configuration.PollingQueues["Test"].IsActive = true;

            this.ConfigurationProvider.PredefineResult(configuration, p => p.GetTaskProcessorConfiguration());

            using (FakeTimer timer = new FakeTimer())
            {
                this.ServiceLocator.PredefineResult(timer, l => l.ResolveSingle(typeof(ITimer)));

                this.TaskProcessor.Start();

                ITaskProcessorRuntimeInfo processorInfo = this.Repository.TaskProcessorRuntimeInfo.GetById(this.TaskProcessor.Id);

                processorInfo.Configuration.PollingQueues["Test"].PollInterval = TimeSpan.FromSeconds(2);

                this.MessageBus.TaskProcessors.NotifyConfigurationChanged(this.TaskProcessor.Id);

                Thread.Sleep(TimeSpan.FromSeconds(1));

                Assert.AreEqual(TimeSpan.FromSeconds(2), timer.Interval);

                Assert.IsTrue(timer.IsActive);
            }
        }

        #endregion Polling Queues

        #region Common Methods

        private void MakeTaskProcessorStopping()
        {
            FakeTask task = new FakeTask();

            Guid taskId = this.TaskProcessorFacade.SubmitTask(task);

            this.TaskExecutor.PredefineMethodCall(e => e.StartTask(taskId, TaskPriority.Normal), () => Thread.Sleep(TimeSpan.FromSeconds(2)));

            this.Repository.TaskRuntimeInfo.Assign(taskId, this.TaskProcessor.Id);

            ThreadPool.QueueUserWorkItem(_ => this.MessageBus.Tasks.NotifyTaskAssigned(taskId, this.TaskProcessor.Id));

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            this.MessageBus.TaskProcessors.NotifyStopRequested(this.TaskProcessor.Id);

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            Assert.AreEqual(TaskProcessorState.Stopping, this.TaskProcessor.State);
        }

        private void StopWithActiveTasks(bool isMaster)
        {
            if (!isMaster)
            {
                this.Repository.TaskProcessorRuntimeInfo.SetMasterIfNotExists(Guid.NewGuid());
            }

            this.DateTimeProvider.UtcNow = Helpers.GetRandomDateTime();

            this.TaskProcessor.Start();

            this.MessageBus.TaskProcessors.RecordedMethodCalls.Clear();

            this.MakeTaskProcessorStopping();

            Assert.IsFalse(this.MasterCommandsProcessor.IsActive);

            this.MessageBus.TaskProcessors.AssertMethodCallOnceWithArguments(mb => mb.NotifyStateChanged(this.TaskProcessor.Id, TaskProcessorState.Stopping));

            if (isMaster)
            {
                Assert.IsNull(this.Repository.TaskProcessorRuntimeInfo.GetMasterId());

                this.MessageBus.TaskProcessors.AssertMethodCallOnce(mb => mb.NotifyMasterModeChanged(this.TaskProcessor.Id, false, MasterModeChangeReason.Stop));
            }
            else
            {
                this.MessageBus.TaskProcessors.AssertNoMethodCall(mb => mb.NotifyMasterModeChanged(Guid.Empty, false, MasterModeChangeReason.None));
            }

            Assert.IsNull(this.Repository.TaskProcessorRuntimeInfo.GetById(this.TaskProcessor.Id));
            Assert.IsFalse(this.Repository.TaskProcessorRuntimeInfo.GetAll().Any(p => p.TaskProcessorId == this.TaskProcessor.Id));

            this.MessageBus.TaskProcessors.AssertMethodCallOnceWithArguments(mb => mb.UnsubscribeFromAllChannelsExcept(new ExpectedCollection<MessageBusChannel>(MessageBusChannel.PerformanceMonitoringRequest)));
            this.MessageBus.Tasks.Receiver.AssertMethodCallOnceWithArguments(mb => mb.UnsubscribeFromAllChannelsExcept(new ExpectedCollection<MessageBusChannel>(MessageBusChannel.TaskCancelRequest)));

            Thread.Sleep(TimeSpan.FromSeconds(1.5));

            Assert.AreEqual(TaskProcessorState.Inactive, this.TaskProcessor.State);

            this.MessageBus.TaskProcessors.AssertMethodCallOnceWithArguments(mb => mb.NotifyStateChanged(this.TaskProcessor.Id, TaskProcessorState.Inactive));

            this.MessageBus.TaskProcessors.AssertMethodCallOnce(mb => mb.UnsubscribeFromAllChannels());
            this.MessageBus.Tasks.Receiver.AssertMethodCallOnce(mb => mb.UnsubscribeFromAllChannels());
        }

        private void StartPollingJob(bool isMaster)
        {
            FakeTaskProcessorConfiguration configuration = new FakeTaskProcessorConfiguration();

            configuration.PollingJobs.Add(typeof(FakePollingJob));

            configuration.PollingJobs[typeof(FakePollingJob)].PollInterval = TimeSpan.FromSeconds(1);
            configuration.PollingJobs[typeof(FakePollingJob)].IsActive = true;
            configuration.PollingJobs[typeof(FakePollingJob)].IsMaster = isMaster;

            this.ConfigurationProvider.PredefineResult(configuration, p => p.GetTaskProcessorConfiguration());

            if (!isMaster)
            {
                this.Repository.TaskProcessorRuntimeInfo.SetMaster(Guid.NewGuid());
            }

            using (FakeTimer timer = new FakeTimer())
            {
                this.ServiceLocator.PredefineResult(timer, l => l.ResolveSingle(typeof(ITimer)));

                this.TaskProcessor.Start();

                Thread.Sleep(TimeSpan.FromSeconds(1.5));

                Assert.AreEqual(TimeSpan.FromSeconds(1), timer.Interval);

                Assert.IsTrue(timer.IsActive);
            }

            FakePollingJob pollingJob = FakePollingJob.CreatedFakePollingJobsInThread.Single();

            pollingJob.AssertMethodCallOnce(p => p.Initialize());
        }

        private void StartPollingQueue(bool isMaster)
        {
            FakeTaskProcessorConfiguration configuration = new FakeTaskProcessorConfiguration();

            configuration.PollingQueues.Add("Test");

            configuration.PollingQueues["Test"].PollInterval = TimeSpan.FromSeconds(1);
            configuration.PollingQueues["Test"].IsActive = true;
            configuration.PollingQueues["Test"].IsMaster = isMaster;

            this.ConfigurationProvider.PredefineResult(configuration, p => p.GetTaskProcessorConfiguration());

            if (!isMaster)
            {
                this.Repository.TaskProcessorRuntimeInfo.SetMaster(Guid.NewGuid());
            }

            using (FakeTimer timer = new FakeTimer())
            {
                this.ServiceLocator.PredefineResult(timer, l => l.ResolveSingle(typeof(ITimer)));

                this.TaskProcessor.Start();

                Thread.Sleep(TimeSpan.FromSeconds(1.5));

                Assert.AreEqual(TimeSpan.FromSeconds(1), timer.Interval);

                Assert.IsTrue(timer.IsActive);
            }
        }

        #endregion Common Methods
    }
}