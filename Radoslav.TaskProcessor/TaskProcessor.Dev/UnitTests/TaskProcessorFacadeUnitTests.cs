using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.TaskProcessor.Facade;
using Radoslav.TaskProcessor.MessageBus;
using Radoslav.TaskProcessor.Model;
using Radoslav.TaskProcessor.Repository.Redis;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public sealed class TaskProcessorFacadeUnitTests
    {
        #region Properties & Initialization

        public TestContext TestContext { get; set; }

        private TaskProcessorFacade TaskProcessorFacade
        {
            get
            {
                return (TaskProcessorFacade)this.TestContext.Properties["TaskProcessorProxy"];
            }
        }

        private FakeTaskProcessorRepository Repository
        {
            get
            {
                return (FakeTaskProcessorRepository)this.TaskProcessorFacade.Repository;
            }
        }

        private FakeMessageBus MessageBus
        {
            get
            {
                return (FakeMessageBus)this.TaskProcessorFacade.MessageBus;
            }
        }

        private FakeDateTimeProvider DateTimeProvider
        {
            get
            {
                return (FakeDateTimeProvider)this.TaskProcessorFacade.DateTimeProvider;
            }
        }

        private FakeClientConfiguration Configuration
        {
            get
            {
                return (FakeClientConfiguration)this.TestContext.Properties["Configuration"];
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            this.TestContext.Properties.Add("Configuration", new FakeClientConfiguration());

            FakeConfigurationProvider configurationProvider = new FakeConfigurationProvider();

            configurationProvider.PredefineResult(this.Configuration, p => p.GetClientConfiguration());

            this.TestContext.Properties.Add("TaskProcessorProxy", new TaskProcessorFacade(new FakeTaskProcessorRepository(), new FakeMessageBus(), new FakeDateTimeProvider(), configurationProvider));
        }

        #endregion Properties & Initialization

        #region SubmitTask

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SubmitTaskNull()
        {
            this.TaskProcessorFacade.SubmitTask(null);
        }

        [TestMethod]
        public void SubmitTask()
        {
            this.SubmitTask(false, false, TaskPriority.Normal, (task, summary) => this.TaskProcessorFacade.SubmitTask(task));
        }

        [TestMethod]
        public void SubmitTaskPriority()
        {
            this.SubmitTask(false, false, TaskPriority.High, (task, summary) => this.TaskProcessorFacade.SubmitTask(task, TaskPriority.High));
        }

        [TestMethod]
        public void SubmitTaskPollingQueue()
        {
            this.SubmitTask(true, false, TaskPriority.Normal, (task, summary) => this.TaskProcessorFacade.SubmitTask(task));
        }

        [TestMethod]
        public void SubmitTaskSummary()
        {
            this.SubmitTask(false, true, TaskPriority.Normal, (task, summary) => this.TaskProcessorFacade.SubmitTask(task, summary));
        }

        [TestMethod]
        public void SubmitTaskSummaryPriority()
        {
            this.SubmitTask(false, true, TaskPriority.Low, (task, summary) => this.TaskProcessorFacade.SubmitTask(task, summary, TaskPriority.Low));
        }

        [TestMethod]
        public void SubmitTaskSession()
        {
            ISubmitTaskSession session = this.TaskProcessorFacade.CreateSubmitTaskSession();

            this.SubmitTask(false, false, TaskPriority.Normal, (task, summary) =>
            {
                session.Complete(task);

                return session.TaskId;
            });
        }

        #endregion SubmitTask

        #region CancelTask

        [TestMethod]
        public void CancelTaskUpdateTaskStatus()
        {
            this.Configuration.PredefineResult(null, c => c.GetPollingQueueKey(typeof(FakeTask)));

            Guid taskId = this.TaskProcessorFacade.SubmitTask(new FakeTask());

            this.DateTimeProvider.UtcNow = DateTime.UtcNow;

            this.TaskProcessorFacade.CancelTask(taskId);

            this.Repository.TaskRuntimeInfo.AssertMethodCallOnceWithArguments(r => r.RequestCancel(taskId, this.DateTimeProvider.UtcNow));
        }

        [TestMethod]
        public void CancelTaskNotifyTaskCanceled()
        {
            this.Configuration.PredefineResult(null, c => c.GetPollingQueueKey(typeof(FakeTask)));

            Guid taskId = this.TaskProcessorFacade.SubmitTask(new FakeTask());

            this.TaskProcessorFacade.MessageBus.Tasks.Receiver.SubscribeForChannels(MessageBusChannel.TaskCancelRequest);

            TaskEventEventArgs args = Helpers.WaitForEvent<TaskEventEventArgs>(
                TimeSpan.FromSeconds(1),
                handler => this.TaskProcessorFacade.MessageBus.Tasks.Receiver.TaskCancelRequested += handler,
                () => this.TaskProcessorFacade.CancelTask(taskId));

            Assert.AreEqual(taskId, args.TaskId);
        }

        [TestMethod]
        public void CancelTaskMoveTaskToArchive()
        {
            this.Configuration.PredefineResult(null, c => c.GetPollingQueueKey(typeof(FakeTask)));

            Guid taskId = this.TaskProcessorFacade.SubmitTask(new FakeTask());

            this.TaskProcessorFacade.CancelTask(taskId);

            Assert.IsFalse(this.Repository.TaskRuntimeInfo.GetPending(false).Any(t => t.TaskId == taskId));
            Assert.IsFalse(this.Repository.TaskRuntimeInfo.GetPending(true).Any(t => t.TaskId == taskId));
            Assert.IsFalse(this.Repository.TaskRuntimeInfo.GetActive().Any(t => t.TaskId == taskId));
            Assert.IsTrue(this.Repository.TaskRuntimeInfo.GetArchive().Any(t => t.TaskId == taskId));
        }

        [TestMethod]
        public void CancelTaskCallRepositoryMethods()
        {
            this.Configuration.PredefineResult(null, c => c.GetPollingQueueKey(typeof(FakeTask)));

            Guid taskId = this.TaskProcessorFacade.SubmitTask(new FakeTask());

            this.DateTimeProvider.UtcNow = DateTime.UtcNow;

            this.TaskProcessorFacade.CancelTask(taskId);

            this.Repository.TaskRuntimeInfo.AssertMethodCallOnce(t => t.RequestCancel(taskId, this.DateTimeProvider.UtcNow));
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void CancelTaskTaskNotFound()
        {
            this.TaskProcessorFacade.CancelTask(Guid.NewGuid());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CancelCanceledTask()
        {
            this.Configuration.PredefineResult(null, c => c.GetPollingQueueKey(typeof(FakeTask)));

            Guid taskId = this.TaskProcessorFacade.SubmitTask(new FakeTask());

            this.TaskProcessorFacade.Repository.TaskRuntimeInfo.RequestCancel(taskId, DateTime.UtcNow);

            this.TaskProcessorFacade.CancelTask(taskId);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CancelFailedTask()
        {
            this.Configuration.PredefineResult(null, c => c.GetPollingQueueKey(typeof(FakeTask)));

            Guid taskId = this.TaskProcessorFacade.SubmitTask(new FakeTask());

            this.TaskProcessorFacade.Repository.TaskRuntimeInfo.Fail(taskId, DateTime.UtcNow, new TypeNotFoundInRedisException());

            this.TaskProcessorFacade.CancelTask(taskId);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CancelCompletedTask()
        {
            this.Configuration.PredefineResult(null, c => c.GetPollingQueueKey(typeof(FakeTask)));

            Guid taskId = this.TaskProcessorFacade.SubmitTask(new FakeTask());

            this.TaskProcessorFacade.Repository.TaskRuntimeInfo.Complete(taskId, DateTime.UtcNow);

            this.TaskProcessorFacade.CancelTask(taskId);
        }

        #endregion CancelTask

        #region MakeTaskProcessorMaster

        [TestMethod]
        public void MakeTaskProcessorMaster1()
        {
            Guid taskProcessorId = Guid.NewGuid();

            this.DateTimeProvider.UtcNow = Helpers.GetRandomDateTime();

            this.TaskProcessorFacade.MakeTaskProcessorMaster(taskProcessorId);

            Assert.AreEqual(taskProcessorId, this.Repository.TaskProcessorRuntimeInfo.GetMasterId());

            this.MessageBus.TaskProcessors.AssertMethodCallOnceWithArguments(mb => mb.NotifyMasterModeChangeRequest(taskProcessorId, true));
        }

        [TestMethod]
        public void MakeTaskProcessorMaster2()
        {
            this.Repository.TaskProcessorRuntimeInfo.SetMaster(Guid.NewGuid());

            Guid taskProcessorId = Guid.NewGuid();

            this.DateTimeProvider.UtcNow = Helpers.GetRandomDateTime();

            this.TaskProcessorFacade.MakeTaskProcessorMaster(taskProcessorId);

            Assert.AreEqual(taskProcessorId, this.Repository.TaskProcessorRuntimeInfo.GetMasterId());

            this.MessageBus.TaskProcessors.AssertMethodCallOnceWithArguments(mb => mb.NotifyMasterModeChangeRequest(taskProcessorId, true));
        }

        #endregion MakeTaskProcessorMaster

        private void SubmitTask(bool isPollingQueueTask, bool hasSummary, TaskPriority expectedPriority, Func<ITask, ITaskSummary, Guid> submitCallback)
        {
            this.DateTimeProvider.UtcNow = DateTime.UtcNow.AddMinutes(-1);

            FakeTask task1 = new FakeTask()
            {
                StringValue = "Hello Submit Task",
                NumberValue = 1234567
            };

            StringTaskSummary summary1;

            if (hasSummary)
            {
                summary1 = new StringTaskSummary("Hello World");
            }

            this.Configuration.PredefineResult(isPollingQueueTask ? "Test" : null, c => c.GetPollingQueueKey(task1.GetType()));

            Guid taskId = submitCallback(task1, summary1);

            Assert.IsNotNull(this.TaskProcessorFacade.GetTask(taskId));

            ITaskRuntimeInfo taskInfo = this.TaskProcessorFacade.GetTaskRuntimeInfo(taskId);

            Assert.IsNotNull(taskInfo);

            Assert.AreEqual(taskId, taskInfo.TaskId);
            Assert.AreEqual(expectedPriority, taskInfo.Priority);
            Assert.AreEqual(TaskStatus.Pending, taskInfo.Status);
            Assert.AreEqual(this.DateTimeProvider.UtcNow, taskInfo.SubmittedUtc);

            FakeTask task2 = (FakeTask)this.Repository.Tasks.GetById(taskId);

            UnitTestHelpers.AssertEqualByPublicScalarProperties(task1, task2);

            if (hasSummary)
            {
                StringTaskSummary summary2 = (StringTaskSummary)this.Repository.TaskSummary.GetById(taskId);

                Assert.AreEqual(summary1.Summary, summary2.Summary);
            }

            TaskSubmittedMasterCommand message = this.TaskProcessorFacade.MessageBus.MasterCommands
                    .OfType<TaskSubmittedMasterCommand>()
                    .SingleOrDefault(m => m.TaskId == taskId);

            if (isPollingQueueTask)
            {
                Assert.IsNull(message);

                Assert.IsTrue(this.Repository.TaskRuntimeInfo.GetPending(true).Any(p => p.TaskId == taskId));
                Assert.IsTrue(this.Repository.TaskRuntimeInfo.ReservePollingQueueTasks("Test", int.MaxValue).Any(p => p.TaskId == taskId));
            }
            else
            {
                Assert.IsNotNull(message);

                Assert.AreNotEqual(Guid.Empty, message.MessageUniqueId);
                Assert.AreEqual(taskId, message.TaskId);

                Assert.IsTrue(this.Repository.TaskRuntimeInfo.GetPending(false).Any(p => p.TaskId == taskId));
            }
        }
    }
}