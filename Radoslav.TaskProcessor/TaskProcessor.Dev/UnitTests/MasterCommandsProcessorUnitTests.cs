using System;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.TaskProcessor.MessageBus;
using Radoslav.TaskProcessor.Model;
using Radoslav.TaskProcessor.Repository.Redis;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public sealed class MasterCommandsProcessorUnitTests
    {
        #region Properties & Initialize

        public TestContext TestContext { get; set; }

        private MasterCommandsProcessor MasterCommandsProcessor
        {
            get
            {
                return (MasterCommandsProcessor)this.TestContext.Properties["MasterCommandsProcessor"];
            }
        }

        private FakeTaskProcessorRepository Repository
        {
            get
            {
                return (FakeTaskProcessorRepository)this.MasterCommandsProcessor.Repository;
            }
        }

        private FakeMessageBus MessageBus
        {
            get
            {
                return (FakeMessageBus)this.MasterCommandsProcessor.MessageBus;
            }
        }

        private FakeTaskDistributor TaskDistributor
        {
            get
            {
                return (FakeTaskDistributor)this.MasterCommandsProcessor.TaskDistributor;
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            this.TestContext.Properties.Add("MasterCommandsProcessor", new MasterCommandsProcessor(new FakeTaskProcessorRepository(), new FakeMessageBus(), new FakeTaskDistributor()));

            this.MessageBus.MasterCommands.ReceiveMessages = true;

            this.MessageBus.Tasks.Receiver.SubscribeForChannels(MessageBusChannel.TaskStarted);

            this.MasterCommandsProcessor.IsActive = true;
        }

        #endregion Properties & Initialize

        #region Constructor

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullRepository()
        {
            new MasterCommandsProcessor(null, new FakeMessageBus(), new FakeTaskDistributor());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullMessageBus()
        {
            new MasterCommandsProcessor(new FakeTaskProcessorRepository(), null, new FakeTaskDistributor());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullTaskDistributor()
        {
            new MasterCommandsProcessor(new FakeTaskProcessorRepository(), new FakeMessageBus(), null);
        }

        #endregion Constructor

        #region Task Requested

        [TestMethod]
        public void TaskRequestedTaskInfoNotFound()
        {
            Guid taskId = Guid.NewGuid();

            this.MessageBus.Tasks.NotifyTaskSubmitted(taskId, DateTime.UtcNow, false);

            Thread.Sleep(TimeSpan.FromSeconds(1));

            this.TaskDistributor.AssertNoMethodCall(d => d.ChooseProcessorForTask(null));

            Assert.IsFalse(this.MessageBus.MasterCommands
                .OfType<TaskSubmittedMasterCommand>()
                .Any(c => c.TaskId == taskId));
        }

        [TestMethod]
        public void TaskRequestedButStatusIsInProgress()
        {
            this.TaskRequestedButStatusIsNotPending(taskId => this.Repository.TaskRuntimeInfo.Start(taskId, Guid.NewGuid(), DateTime.UtcNow));
        }

        [TestMethod]
        public void TaskRequestedButStatusIsCanceled()
        {
            this.TaskRequestedButStatusIsNotPending(taskId => this.Repository.TaskRuntimeInfo.RequestCancel(taskId, DateTime.UtcNow));
        }

        [TestMethod]
        public void TaskRequestedButStatusIsFailed()
        {
            this.TaskRequestedButStatusIsNotPending(taskId => this.Repository.TaskRuntimeInfo.Fail(taskId, DateTime.UtcNow, new TypeNotFoundInRedisException()));
        }

        [TestMethod]
        public void TaskRequestedButStatusIsSuccess()
        {
            this.TaskRequestedButStatusIsNotPending(taskId => this.Repository.TaskRuntimeInfo.Complete(taskId, DateTime.UtcNow));
        }

        [TestMethod]
        public void TaskRequestedAssignToFirstProcessor()
        {
            Guid taskProcessorId1 = Guid.NewGuid();
            Guid taskProcessorId2 = Guid.NewGuid();

            ITaskProcessorRuntimeInfo processor1 = this.Repository.TaskProcessorRuntimeInfo.Create(taskProcessorId1, "First");
            ITaskProcessorRuntimeInfo processor2 = this.Repository.TaskProcessorRuntimeInfo.Create(taskProcessorId2, "Second");

            this.Repository.TaskProcessorRuntimeInfo.Add(processor1);
            this.Repository.TaskProcessorRuntimeInfo.Add(processor2);

            ITaskRuntimeInfo pendingTaskInfo = this.Repository.TaskRuntimeInfo.Create();

            this.Repository.TaskRuntimeInfo.Add(pendingTaskInfo);

            this.TaskDistributor.PredefineResult(
                new ITaskProcessorRuntimeInfo[] { processor1, processor2 },
                d => d.ChooseProcessorForTask(pendingTaskInfo));

            this.MessageBus.Tasks.NotifyTaskSubmitted(pendingTaskInfo.TaskId, DateTime.UtcNow, false);

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            this.TaskDistributor.AssertMethodCallOnceWithArguments(d => d.ChooseProcessorForTask(pendingTaskInfo));

            this.MessageBus.Tasks.AssertMethodCallOnceWithArguments(mb => mb.NotifyTaskAssigned(pendingTaskInfo.TaskId, taskProcessorId1));

            this.MessageBus.Tasks.NotifyTaskStarted(pendingTaskInfo.TaskId, taskProcessorId1, DateTime.UtcNow);

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            this.MessageBus.Tasks.AssertNoMethodCallWithArguments(mb => mb.NotifyTaskAssigned(pendingTaskInfo.TaskId, taskProcessorId2));

            Assert.IsFalse(this.MessageBus.MasterCommands
                .OfType<TaskSubmittedMasterCommand>()
                .Any(c => c.TaskId == pendingTaskInfo.TaskId));

            pendingTaskInfo = this.Repository.TaskRuntimeInfo.GetById(pendingTaskInfo.TaskId);

            Assert.AreEqual(taskProcessorId1, pendingTaskInfo.TaskProcessorId);
        }

        [TestMethod]
        public void TaskRequestedAssignToSecondProcessor()
        {
            Guid taskProcessorId1 = Guid.NewGuid();
            Guid taskProcessorId2 = Guid.NewGuid();

            ITaskProcessorRuntimeInfo firstProcessor = this.Repository.TaskProcessorRuntimeInfo.Create(taskProcessorId1, "First");
            ITaskProcessorRuntimeInfo secondProcessor = this.Repository.TaskProcessorRuntimeInfo.Create(taskProcessorId2, "Second");

            this.Repository.TaskProcessorRuntimeInfo.Add(firstProcessor);
            this.Repository.TaskProcessorRuntimeInfo.Add(secondProcessor);

            ITaskRuntimeInfo pendingTaskInfo = this.Repository.TaskRuntimeInfo.Create();

            this.Repository.TaskRuntimeInfo.Add(pendingTaskInfo);

            this.TaskDistributor.PredefineResult(
                new ITaskProcessorRuntimeInfo[] { firstProcessor, secondProcessor },
                d => d.ChooseProcessorForTask(pendingTaskInfo));

            this.MasterCommandsProcessor.AssignTaskTimeout = TimeSpan.FromSeconds(1);

            this.MessageBus.Tasks.NotifyTaskSubmitted(pendingTaskInfo.TaskId, DateTime.UtcNow, false);

            Thread.Sleep(TimeSpan.FromSeconds(1.5));

            this.TaskDistributor.AssertMethodCallWithArguments(d => d.ChooseProcessorForTask(pendingTaskInfo));

            this.MessageBus.Tasks.AssertMethodCallWithArguments(mb => mb.NotifyTaskAssigned(pendingTaskInfo.TaskId, taskProcessorId1));
            this.MessageBus.Tasks.AssertMethodCallWithArguments(mb => mb.NotifyTaskAssigned(pendingTaskInfo.TaskId, taskProcessorId2));

            this.MessageBus.Tasks.AssertNoMethodCall(mb => mb.NotifyTaskAssigned(Guid.Empty, Guid.Empty));

            this.MessageBus.Tasks.NotifyTaskStarted(pendingTaskInfo.TaskId, taskProcessorId2, DateTime.UtcNow);

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            Assert.IsFalse(this.MessageBus.MasterCommands.OfType<TaskSubmittedMasterCommand>().Any(c => c.TaskId == pendingTaskInfo.TaskId));

            pendingTaskInfo = this.Repository.TaskRuntimeInfo.GetById(pendingTaskInfo.TaskId);

            Assert.AreEqual(taskProcessorId2, pendingTaskInfo.TaskProcessorId);
        }

        [TestMethod]
        public void TaskRequestedAllAssignAttemptsFail()
        {
            Guid taskProcessorId1 = Guid.NewGuid();
            Guid taskProcessorId2 = Guid.NewGuid();

            ITaskProcessorRuntimeInfo firstProcessor = this.Repository.TaskProcessorRuntimeInfo.Create(taskProcessorId1, "First");
            ITaskProcessorRuntimeInfo secondProcessor = this.Repository.TaskProcessorRuntimeInfo.Create(taskProcessorId2, "Second");

            this.Repository.TaskProcessorRuntimeInfo.Add(firstProcessor);
            this.Repository.TaskProcessorRuntimeInfo.Add(secondProcessor);

            ITaskRuntimeInfo pendingTaskInfo = this.Repository.TaskRuntimeInfo.Create();

            this.Repository.TaskRuntimeInfo.Add(pendingTaskInfo);

            this.TaskDistributor.PredefineResult(
                new ITaskProcessorRuntimeInfo[] { firstProcessor, secondProcessor },
                d => d.ChooseProcessorForTask(pendingTaskInfo));

            this.MasterCommandsProcessor.AssignTaskTimeout = TimeSpan.FromSeconds(1);

            this.MessageBus.Tasks.NotifyTaskSubmitted(pendingTaskInfo.TaskId, DateTime.UtcNow, false);

            Thread.Sleep(TimeSpan.FromSeconds(3));

            this.TaskDistributor.AssertMethodCallWithArguments(d => d.ChooseProcessorForTask(pendingTaskInfo));

            this.MessageBus.Tasks.AssertMethodCallWithArguments(mb => mb.NotifyTaskAssigned(pendingTaskInfo.TaskId, taskProcessorId1));
            this.MessageBus.Tasks.AssertMethodCallWithArguments(mb => mb.NotifyTaskAssigned(pendingTaskInfo.TaskId, taskProcessorId2));

            this.MessageBus.Tasks.AssertNoMethodCall(mb => mb.NotifyTaskAssigned(Guid.Empty, Guid.Empty));

            Assert.IsFalse(this.MessageBus.MasterCommands.OfType<TaskSubmittedMasterCommand>().Any(c => c.TaskId == pendingTaskInfo.TaskId));

            pendingTaskInfo = this.Repository.TaskRuntimeInfo.GetById(pendingTaskInfo.TaskId);

            Assert.IsNull(pendingTaskInfo.TaskProcessorId);
        }

        #endregion Task Requested

        [TestMethod]
        public void TaskStarted()
        {
            Guid taskId = this.Repository.TaskRuntimeInfo.Add().TaskId;

            Guid taskProcessorId = Guid.NewGuid();

            DateTime timestamp = DateTime.UtcNow;

            this.MessageBus.Tasks.NotifyTaskStarted(taskId, taskProcessorId, timestamp);

            Thread.Sleep(TimeSpan.FromSeconds(1));
        }

        [TestMethod]
        public void TaskProgress()
        {
            Guid taskId = this.Repository.TaskRuntimeInfo.Add().TaskId;

            this.MessageBus.Tasks.NotifyTaskProgress(taskId, 33);

            Thread.Sleep(TimeSpan.FromSeconds(1));
        }

        [TestMethod]
        public void TaskProcessorRegistered()
        {
            Guid taskProcessorId = Guid.NewGuid();

            this.AssignTasksToProcessorTest(taskProcessorId, () => this.MessageBus.TaskProcessors.NotifyStateChanged(taskProcessorId, TaskProcessorState.Active));

            Assert.IsFalse(this.MessageBus.MasterCommands
                .OfType<TaskProcessorRegisteredMasterCommand>()
                .Any(c => c.TaskProcessorId == taskProcessorId));
        }

        [TestMethod]
        public void TaskProcessorConfigurationChanged()
        {
            Guid taskProcessorId = Guid.NewGuid();

            this.AssignTasksToProcessorTest(taskProcessorId, () => this.MessageBus.TaskProcessors.NotifyConfigurationChanged(taskProcessorId));

            Assert.IsFalse(this.MessageBus.MasterCommands
               .OfType<ConfigurationChangedMasterCommand>()
               .Any(c => c.TaskProcessorId == taskProcessorId));
        }

        [TestMethod]
        public void TaskCompleted()
        {
            Guid taskId = this.Repository.TaskRuntimeInfo.Add().TaskId;

            DateTime timestampUtc = DateTime.UtcNow;

            Guid taskProcessorId = Guid.NewGuid();

            this.MessageBus.Tasks.NotifyTaskCompleted(taskId, timestampUtc, taskProcessorId, false, TimeSpan.Zero);

            Thread.Sleep(TimeSpan.FromSeconds(1));

            this.TaskDistributor.AssertMethodCallOnceWithArguments(d => d.ChooseNextTasksForProcessor(taskProcessorId));

            Assert.IsFalse(this.MessageBus.MasterCommands.OfType<TaskCompletedMasterCommand>().Any(c => c.TaskId == taskId));
        }

        [TestMethod]
        public void TaskFailed()
        {
            Guid taskId = this.Repository.TaskRuntimeInfo.Add().TaskId;

            Guid taskProcessorId = Guid.NewGuid();

            DateTime timestampUtc = DateTime.UtcNow;

            this.MessageBus.Tasks.NotifyTaskFailed(taskId, timestampUtc, taskProcessorId, false, "Hello Error");

            Thread.Sleep(TimeSpan.FromSeconds(1));

            this.TaskDistributor.AssertMethodCallOnceWithArguments(d => d.ChooseNextTasksForProcessor(taskProcessorId));

            Assert.IsFalse(this.MessageBus.MasterCommands.OfType<TaskFailedMasterCommand>().Any(c => c.TaskId == taskId));
        }

        [TestMethod]
        public void TaskCancelCompleted()
        {
            Guid taskId = this.Repository.TaskRuntimeInfo.Add().TaskId;

            Guid taskProcessorId = Guid.NewGuid();

            DateTime timestampUtc = DateTime.UtcNow;

            this.MessageBus.Tasks.NotifyTaskCancelCompleted(taskId, timestampUtc, taskProcessorId, false);

            Thread.Sleep(TimeSpan.FromSeconds(1));

            this.TaskDistributor.AssertMethodCallOnceWithArguments(d => d.ChooseNextTasksForProcessor(taskProcessorId));

            Assert.IsFalse(this.MessageBus.MasterCommands.OfType<TaskCancelCompletedMasterCommand>().Any(c => c.TaskId == taskId));
        }

        private void AssignTasksToProcessorTest(Guid taskProcessorId, Action callback)
        {
            this.MasterCommandsProcessor.IsActive = false;

            ITaskProcessorRuntimeInfo processorInfo = this.Repository.TaskProcessorRuntimeInfo.Create(taskProcessorId, "Test");

            this.Repository.TaskProcessorRuntimeInfo.Add(processorInfo);

            ITaskRuntimeInfo pendingTaskInfo1 = new FakeTaskRuntimeInfo()
            {
                TaskId = Guid.NewGuid(),
                SubmittedUtc = DateTime.UtcNow
            };

            ITaskRuntimeInfo pendingTaskInfo2 = new FakeTaskRuntimeInfo()
            {
                TaskId = Guid.NewGuid(),
                SubmittedUtc = DateTime.UtcNow
            };

            ITaskRuntimeInfo activeTaskInfo1 = new FakeTaskRuntimeInfo()
            {
                TaskId = Guid.NewGuid(),
                SubmittedUtc = DateTime.UtcNow
            };

            ITaskRuntimeInfo activeTaskInfo2 = new FakeTaskRuntimeInfo()
            {
                TaskId = Guid.NewGuid(),
                SubmittedUtc = DateTime.UtcNow
            };

            this.Repository.TaskRuntimeInfo.Add(pendingTaskInfo1);
            this.Repository.TaskRuntimeInfo.Add(pendingTaskInfo2);
            this.Repository.TaskRuntimeInfo.Add(activeTaskInfo1);
            this.Repository.TaskRuntimeInfo.Add(activeTaskInfo2);

            this.Repository.TaskRuntimeInfo.Start(activeTaskInfo1.TaskId, Guid.Empty, DateTime.UtcNow);
            this.Repository.TaskRuntimeInfo.Start(activeTaskInfo2.TaskId, Guid.Empty, DateTime.UtcNow);

            this.TaskDistributor.PredefineResult(
                new ITaskRuntimeInfo[] { pendingTaskInfo1, pendingTaskInfo2 },
                d => d.ChooseNextTasksForProcessor(processorInfo.TaskProcessorId));

            this.MasterCommandsProcessor.AssignTaskTimeout = TimeSpan.FromSeconds(0.5);

            this.MasterCommandsProcessor.IsActive = true;

            callback();

            Thread.Sleep(TimeSpan.FromSeconds(1.5));

            this.TaskDistributor.AssertMethodCallOnceWithArguments(d => d.ChooseNextTasksForProcessor(processorInfo.TaskProcessorId));

            foreach (ITaskRuntimeInfo taskInfo in new ITaskRuntimeInfo[] { pendingTaskInfo1, pendingTaskInfo2 })
            {
                this.MessageBus.Tasks.AssertMethodCallWithArguments(mb => mb.NotifyTaskAssigned(taskInfo.TaskId, taskProcessorId));
            }

            this.MessageBus.Tasks.AssertNoMethodCall(mb => mb.NotifyTaskAssigned(Guid.Empty, Guid.Empty));
        }

        private void TaskRequestedButStatusIsNotPending(Action<Guid> callback)
        {
            Guid taskId = Guid.NewGuid();

            this.Repository.TaskRuntimeInfo.Add(new FakeTaskRuntimeInfo()
            {
                TaskId = taskId
            });

            callback(taskId);

            this.MessageBus.Tasks.NotifyTaskSubmitted(taskId, DateTime.UtcNow, false);

            Thread.Sleep(TimeSpan.FromSeconds(1));

            this.TaskDistributor.AssertNoMethodCall(d => d.ChooseProcessorForTask(null));

            Assert.IsFalse(this.MessageBus.MasterCommands.OfType<TaskSubmittedMasterCommand>().Any(c => c.TaskId == taskId));
        }
    }
}