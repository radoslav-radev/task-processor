using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.TaskProcessor.Model;
using Radoslav.TaskProcessor.Repository;
using Radoslav.TaskProcessor.Repository.Redis;
using Radoslav.TaskProcessor.TaskWorker;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public sealed class TaskWorkerBootstrapUnitTests
    {
        #region Properties & Initialize

        public TestContext TestContext { get; set; }

        private TaskWorkerBootstrap TaskWorkerBootstrap
        {
            get
            {
                return (TaskWorkerBootstrap)this.TestContext.Properties["TaskWorker"];
            }
        }

        private FakeTaskProcessorRepository Repository
        {
            get
            {
                return (FakeTaskProcessorRepository)this.TestContext.Properties["Repository"];
            }
        }

        private FakeMessageBus MessageBus
        {
            get
            {
                return (FakeMessageBus)this.TestContext.Properties["MessageBus"];
            }
        }

        private FakeDateTimeProvider DateTimeProvider
        {
            get
            {
                return (FakeDateTimeProvider)this.TestContext.Properties["DateTimeProvider"];
            }
        }

        private FakeTaskWorkerFactory TaskWorkerFactory
        {
            get
            {
                return (FakeTaskWorkerFactory)this.TestContext.Properties["TaskWorkerFactory"];
            }
        }

        private FakeTaskWorkersConfiguration Configuration
        {
            get
            {
                return (FakeTaskWorkersConfiguration)this.TestContext.Properties["Configuration"];
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            this.TestContext.Properties.Add("Repository", new FakeTaskProcessorRepository());
            this.TestContext.Properties.Add("MessageBus", new FakeMessageBus());
            this.TestContext.Properties.Add("DateTimeProvider", new FakeDateTimeProvider());
            this.TestContext.Properties.Add("TaskWorkerFactory", new FakeTaskWorkerFactory());
            this.TestContext.Properties.Add("Configuration", new FakeTaskWorkersConfiguration());

            FakeConfigurationProvider configProvider = new FakeConfigurationProvider();

            configProvider.PredefineResult(this.Configuration, p => p.GetTaskWorkerConfiguration());

            this.Configuration.Add<FakeTask>(new FakeTaskWorkerConfiguration());

            this.TestContext.Properties.Add("TaskWorker", new TaskWorkerBootstrap(configProvider, this.Repository, this.MessageBus, this.TaskWorkerFactory, this.DateTimeProvider));
        }

        #endregion Properties & Initialize

        #region Constructor

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullConfigurationProvider()
        {
            new TaskWorkerBootstrap(null, new FakeTaskProcessorRepository(), new FakeMessageBus(), new FakeTaskWorkerFactory(), new FakeDateTimeProvider());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructorNullConfiguration()
        {
            FakeConfigurationProvider configProvider = new FakeConfigurationProvider();

            new TaskWorkerBootstrap(configProvider, new FakeTaskProcessorRepository(), new FakeMessageBus(), new FakeTaskWorkerFactory(), new FakeDateTimeProvider());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullRepository()
        {
            new TaskWorkerBootstrap(new FakeConfigurationProvider(), null, new FakeMessageBus(), new FakeTaskWorkerFactory(), new FakeDateTimeProvider());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullMessageBus()
        {
            new TaskWorkerBootstrap(new FakeConfigurationProvider(), new FakeTaskProcessorRepository(), null, new FakeTaskWorkerFactory(), new FakeDateTimeProvider());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullTaskWorkerFactory()
        {
            new TaskWorkerBootstrap(new FakeConfigurationProvider(), new FakeTaskProcessorRepository(), new FakeMessageBus(), null, new FakeDateTimeProvider());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullDateTimeProvider()
        {
            new TaskWorkerBootstrap(new FakeConfigurationProvider(), new FakeTaskProcessorRepository(), new FakeMessageBus(), new FakeTaskWorkerFactory(), null);
        }

        #endregion Constructor

        #region Start Task

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void StartTaskRuntimeInfoNotFound()
        {
            Guid taskId = Guid.NewGuid();

            ITask task = new FakeTask();

            this.Repository.Tasks.Add(taskId, task);

            using (FakeTaskWorker worker = new FakeTaskWorker())
            {
                this.TaskWorkerFactory.PredefineResult(worker, f => f.CreateTaskWorker(task));
            }

            this.TaskWorkerBootstrap.StartTask(taskId);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void StartTaskNotFound()
        {
            Guid taskId = Guid.NewGuid();

            this.Repository.TaskRuntimeInfo.Add(new FakeTaskRuntimeInfo()
            {
                TaskId = taskId
            });

            this.TaskWorkerBootstrap.StartTask(taskId);
        }

        [TestMethod]
        [ExpectedException(typeof(OperationCanceledException))]
        public void StartTaskCanceled()
        {
            ITaskRuntimeInfo taskInfo = this.Repository.TaskRuntimeInfo.Add();

            ITask task = new FakeTask();

            this.Repository.Tasks.Add(taskInfo.TaskId, task);

            using (FakeTaskWorker worker = new FakeTaskWorker())
            {
                this.TaskWorkerFactory.PredefineResult(worker, f => f.CreateTaskWorker(task));

                this.Repository.TaskRuntimeInfo.RequestCancel(taskInfo.TaskId, DateTime.UtcNow);

                this.TaskWorkerBootstrap.StartTask(taskInfo.TaskId);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void StartTaskFailed()
        {
            this.StartTaskCompleted(taskId => this.Repository.TaskRuntimeInfo.Fail(taskId, DateTime.UtcNow, new TypeNotFoundInRedisException()));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void StartTaskSuccess()
        {
            this.StartTaskCompleted(taskId => this.Repository.TaskRuntimeInfo.Complete(taskId, DateTime.UtcNow));
        }

        [TestMethod]
        public void StartTaskNoTaskJobSettings()
        {
            ITaskRuntimeInfo taskInfo = this.Repository.TaskRuntimeInfo.Create();

            FakeTask task = new FakeTask();

            this.Repository.Tasks.Add(taskInfo.TaskId, task);

            this.Repository.TaskRuntimeInfo.Add(taskInfo);

            this.DateTimeProvider.UtcNow = DateTime.UtcNow;

            using (FakeTaskWorker taskWorker = new FakeTaskWorker())
            {
                this.TaskWorkerFactory.PredefineResult(taskWorker, f => f.CreateTaskWorker(task));

                this.TaskWorkerBootstrap.StartTask(taskInfo.TaskId);

                this.TaskWorkerFactory.AssertMethodCallOnceWithArguments(f => f.CreateTaskWorker(task));

                taskWorker.AssertMethodCallOnce(w => w.StartTask(task, null));
            }
        }

        [TestMethod]
        public void StartTaskWithTaskJobSettings()
        {
            this.Configuration.Get<FakeTask>().HasTaskJobSettings = true;

            ITaskJobSettings settings = new FakeTaskJobSettings();

            this.Repository.TaskJobSettings.Set<FakeTask>(settings);

            ITaskRuntimeInfo taskInfo = this.Repository.TaskRuntimeInfo.Create();

            FakeTask task = new FakeTask();

            this.Repository.Tasks.Add(taskInfo.TaskId, task);

            this.Repository.TaskRuntimeInfo.Add(taskInfo);

            using (FakeTaskWorker taskWorker = new FakeTaskWorker())
            {
                this.TaskWorkerFactory.PredefineResult(taskWorker, f => f.CreateTaskWorker(task));

                this.DateTimeProvider.UtcNow = DateTime.UtcNow;

                this.TaskWorkerBootstrap.StartTask(taskInfo.TaskId);

                this.TaskWorkerFactory.AssertMethodCallOnceWithArguments(f => f.CreateTaskWorker(task));

                taskWorker.AssertMethodCallOnce(w => w.StartTask(task, settings));
            }
        }

        #endregion Start Task

        #region Report Progress / Fail / Complete Task

        [TestMethod]
        public void ReportProgress()
        {
            ITaskRuntimeInfo taskInfo = this.Repository.TaskRuntimeInfo.Create();

            this.Repository.TaskRuntimeInfo.Add(taskInfo);

            FakeTask task = new FakeTask();

            this.Repository.Tasks.Add(taskInfo.TaskId, task);

            double percent = DateTime.Now.Second;

            using (FakeTaskWorker worker = new FakeTaskWorker())
            {
                this.TaskWorkerFactory.PredefineResult(worker, f => f.CreateTaskWorker(task));

                this.TaskWorkerBootstrap.StartTask(taskInfo.TaskId);

                worker.RaiseReportProgress(percent);
            }

            this.MessageBus.Tasks.Receiver.AssertMethodCallOnceWithArguments(mb => mb.NotifyTaskProgress(taskInfo.TaskId, percent));

            this.Repository.TaskRuntimeInfo.AssertMethodCallOnceWithArguments(r => r.Progress(taskInfo.TaskId, percent));
        }

        [TestMethod]
        public void Fail()
        {
            ITaskRuntimeInfo taskInfo = this.Repository.TaskRuntimeInfo.Add();

            ITask task = new FakeTask();

            this.Repository.Tasks.Add(taskInfo.TaskId, task);

            this.DateTimeProvider.UtcNow = DateTime.UtcNow;

            Exception error = new TypeNotFoundInRedisException("Dummy Error");

            using (FakeTaskWorker worker = new FakeTaskWorker())
            {
                worker.PredefineMethodCall(w => w.StartTask(task, null), () => { throw error; });

                this.TaskWorkerFactory.PredefineResult(worker, f => f.CreateTaskWorker(task));

                try
                {
                    this.TaskWorkerBootstrap.StartTask(taskInfo.TaskId);
                }
                catch (TypeNotFoundInRedisException)
                {
                }

                worker.AssertMethodCallOnce(w => w.Dispose());
            }

            this.MessageBus.Tasks.Receiver.AssertMethodCallOnce(mb => mb.UnsubscribeFromAllChannels());

            this.Repository.TaskRuntimeInfo.AssertMethodCallOnceWithArguments(r => r.Fail(taskInfo.TaskId, this.DateTimeProvider.UtcNow, error));
        }

        [TestMethod]
        public void Complete()
        {
            ITaskRuntimeInfo taskInfo = this.Repository.TaskRuntimeInfo.Add();

            ITask task = new FakeTask();

            this.Repository.Tasks.Add(taskInfo.TaskId, task);

            this.DateTimeProvider.UtcNow = DateTime.UtcNow;

            using (FakeTaskWorker taskWorker = new FakeTaskWorker())
            {
                taskWorker.PredefineMethodCall(w => w.StartTask(task, null), () => { });

                this.TaskWorkerFactory.PredefineResult(taskWorker, f => f.CreateTaskWorker(task));

                this.TaskWorkerBootstrap.StartTask(taskInfo.TaskId);

                taskWorker.AssertMethodCallOnce(w => w.Dispose());
            }

            this.MessageBus.Tasks.Receiver.AssertMethodCallOnce(mb => mb.UnsubscribeFromAllChannels());
        }

        #endregion Report Progress / Fail / Complete Task

        #region Cancel Task

        [TestMethod]
        public void CancelTask()
        {
            ITaskRuntimeInfo taskInfo = this.Repository.TaskRuntimeInfo.Create();

            this.Repository.TaskRuntimeInfo.Add(taskInfo);

            ITask task = new FakeTask();

            this.Repository.Tasks.Add(taskInfo.TaskId, task);

            using (FakeTaskWorker taskWorker = new FakeTaskWorker())
            {
                this.TaskWorkerFactory.PredefineResult(taskWorker, f => f.CreateTaskWorker(task));

                taskWorker.PredefineMethodCall(w => w.StartTask(task, null), () => Thread.Sleep(TimeSpan.FromSeconds(1)));

                ThreadPool.QueueUserWorkItem(_ => this.TaskWorkerBootstrap.StartTask(taskInfo.TaskId));

                Thread.Sleep(TimeSpan.FromSeconds(0.5));

                this.DateTimeProvider.UtcNow = DateTime.UtcNow;

                this.MessageBus.Tasks.NotifyTaskCancelRequest(taskInfo.TaskId, this.DateTimeProvider.UtcNow);

                Thread.Sleep(TimeSpan.FromSeconds(1.5));

                taskWorker.AssertMethodCallOnce(w => w.CancelTask());
                taskWorker.AssertMethodCallOnce(w => w.Dispose());
            }

            this.MessageBus.Tasks.Receiver.AssertMethodCallOnce(mb => mb.UnsubscribeFromAllChannels());
        }

        [TestMethod]
        public void CancelAnotherTask()
        {
            ITaskRuntimeInfo taskInfo = this.Repository.TaskRuntimeInfo.Create();

            this.Repository.TaskRuntimeInfo.Add(taskInfo);

            ITask task = new FakeTask();

            this.Repository.Tasks.Add(taskInfo.TaskId, task);

            using (FakeTaskWorker taskWorker = new FakeTaskWorker())
            {
                this.TaskWorkerFactory.PredefineResult(taskWorker, f => f.CreateTaskWorker(task));

                this.TaskWorkerBootstrap.StartTask(taskInfo.TaskId);

                this.MessageBus.Tasks.NotifyTaskCancelRequest(Guid.NewGuid(), DateTime.UtcNow);

                Thread.Sleep(TimeSpan.FromSeconds(0.5));

                taskWorker.AssertNoMethodCall(w => w.CancelTask());
            }
        }

        [TestMethod]
        public void CancelTaskMultipleTimes()
        {
            ITaskRuntimeInfo taskInfo = this.Repository.TaskRuntimeInfo.Add();

            ITask task = new FakeTask();

            this.Repository.Tasks.Add(taskInfo.TaskId, task);

            using (FakeTaskWorker taskWorker = new FakeTaskWorker())
            {
                taskWorker.PredefineMethodCall(w => w.StartTask(task, null), () => Thread.Sleep(TimeSpan.FromSeconds(1)));

                this.TaskWorkerFactory.PredefineResult(taskWorker, f => f.CreateTaskWorker(task));

                for (int i = 0; i < 3; i++)
                {
                    ThreadPool.QueueUserWorkItem(_ =>
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(0.3));

                        this.MessageBus.Tasks.NotifyTaskCancelRequest(taskInfo.TaskId, DateTime.UtcNow);
                    });
                }

                this.TaskWorkerBootstrap.StartTask(taskInfo.TaskId);

                Thread.Sleep(TimeSpan.FromSeconds(0.5));

                taskWorker.AssertMethodCallOnceWithArguments(w => w.CancelTask());
            }

            this.MessageBus.Tasks.Receiver.AssertMethodCallOnce(mb => mb.UnsubscribeFromAllChannels());
        }

        #endregion Cancel Task

        private void StartTaskCompleted(Action<Guid> completeTask)
        {
            ITaskRuntimeInfo taskInfo = this.Repository.TaskRuntimeInfo.Create();

            this.Repository.TaskRuntimeInfo.Add(taskInfo);

            ITask task = new FakeTask();

            this.Repository.Tasks.Add(taskInfo.TaskId, task);

            using (FakeTaskWorker worker = new FakeTaskWorker())
            {
                this.TaskWorkerFactory.PredefineResult(worker, f => f.CreateTaskWorker(task));
            }

            completeTask(taskInfo.TaskId);

            this.TaskWorkerBootstrap.StartTask(taskInfo.TaskId);
        }
    }
}