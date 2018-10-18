using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.TaskProcessor.Facade;
using Radoslav.TaskProcessor.Model;
using Radoslav.TaskProcessor.Repository;
using Radoslav.TaskProcessor.Repository.Redis;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public abstract class TaskRuntimeInfoRepositoryUnitTestsBase : RepositoryTestsBase<ITaskRuntimeInfoRepository>
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateTaskTypeNull()
        {
            this.Repository.Create(Guid.NewGuid(), null, DateTime.UtcNow, TaskPriority.Normal, "Polling Queue");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateTaskTypeNotTask()
        {
            this.Repository.Create(Guid.NewGuid(), typeof(ITaskProcessorFacade), DateTime.UtcNow, TaskPriority.Normal, null);
        }

        [TestMethod]
        public void Create()
        {
            Guid taskId = Guid.NewGuid();

            DateTime timestampUtc = DateTime.UtcNow.AddMinutes(-1);

            ITaskRuntimeInfo taskInfo = this.Repository.Create(taskId, typeof(FakeTask), timestampUtc, TaskPriority.VeryHigh, "Polling Queue");

            Assert.AreEqual(taskId, taskInfo.TaskId);
            Assert.AreEqual(typeof(FakeTask), taskInfo.TaskType);
            Assert.AreEqual(TaskPriority.VeryHigh, taskInfo.Priority);
            Assert.AreEqual(TaskStatus.Pending, taskInfo.Status);
            Assert.AreEqual(timestampUtc, taskInfo.SubmittedUtc);
            Assert.AreEqual("Polling Queue", taskInfo.PollingQueue);
            Assert.IsNull(taskInfo.Error);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddNull()
        {
            this.Repository.Add(null);
        }

        [TestMethod]
        public void Add()
        {
            ITaskRuntimeInfo taskInfo1 = this.Repository.Add(TaskPriority.Low);
            ITaskRuntimeInfo taskInfo2 = this.Repository.GetById(taskInfo1.TaskId);

            UnitTestHelpers.AssertEqualByPublicScalarProperties(taskInfo1, taskInfo2);

            Assert.IsTrue(this.Repository.CheckIsPendingOrActive(taskInfo1.TaskId));
            Assert.IsTrue(this.Repository.GetPending(false).Any(t => t.TaskId == taskInfo1.TaskId));
            Assert.IsTrue(this.Repository.GetPending(true).Any(t => t.TaskId == taskInfo1.TaskId));
            Assert.IsTrue(this.Repository.GetPendingAndActive()[TaskStatus.Pending].Any(t => t.TaskId == taskInfo1.TaskId));
            Assert.IsFalse(this.Repository.GetPendingAndActive()[TaskStatus.InProgress].Any(t => t.TaskId == taskInfo1.TaskId));
            Assert.IsFalse(this.Repository.GetActive().Any(t => t.TaskId == taskInfo1.TaskId));
            Assert.IsFalse(this.Repository.GetFailed().Any(t => t.TaskId == taskInfo1.TaskId));
            Assert.IsFalse(this.Repository.GetArchive().Any(t => t.TaskId == taskInfo1.TaskId));
            Assert.IsFalse(this.Repository.ReservePollingQueueTasks("A", 10).Any(t => t.TaskId == taskInfo1.TaskId));
        }

        [TestMethod]
        public void AddPollingQueueTask()
        {
            ITaskRuntimeInfo taskInfo1 = this.Repository.Add("Test");
            ITaskRuntimeInfo taskInfo2 = this.Repository.GetById(taskInfo1.TaskId);

            UnitTestHelpers.AssertEqualByPublicScalarProperties(taskInfo1, taskInfo2);

            Assert.IsTrue(this.Repository.CheckIsPendingOrActive(taskInfo1.TaskId));
            Assert.IsFalse(this.Repository.GetPending(false).Any(t => t.TaskId == taskInfo1.TaskId));
            Assert.IsTrue(this.Repository.GetPending(true).Any(t => t.TaskId == taskInfo1.TaskId));
            Assert.IsFalse(this.Repository.GetPendingAndActive()[TaskStatus.Pending].Any(t => t.TaskId == taskInfo1.TaskId));
            Assert.IsFalse(this.Repository.GetPendingAndActive()[TaskStatus.InProgress].Any(t => t.TaskId == taskInfo1.TaskId));
            Assert.IsFalse(this.Repository.GetActive().Any(t => t.TaskId == taskInfo1.TaskId));
            Assert.IsFalse(this.Repository.GetFailed().Any(t => t.TaskId == taskInfo1.TaskId));
            Assert.IsFalse(this.Repository.GetArchive().Any(t => t.TaskId == taskInfo1.TaskId));
            Assert.IsTrue(this.Repository.ReservePollingQueueTasks("Test", 1).Any(t => t.TaskId == taskInfo1.TaskId));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AddStatusInProgress()
        {
            this.Repository.Add(new FakeTaskRuntimeInfo()
            {
                Status = TaskStatus.InProgress
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AddStatusCanceled()
        {
            this.Repository.Add(new FakeTaskRuntimeInfo()
            {
                Status = TaskStatus.Canceled
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AddStatusFailed()
        {
            this.Repository.Add(new FakeTaskRuntimeInfo()
            {
                Status = TaskStatus.Failed
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AddStatusSuccess()
        {
            this.Repository.Add(new FakeTaskRuntimeInfo()
            {
                Status = TaskStatus.Success
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddTaskProcessorNotNull()
        {
            this.Repository.Add(new FakeTaskRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid()
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddStartUtcIsNotNull()
        {
            this.Repository.Add(new FakeTaskRuntimeInfo()
            {
                StartedUtc = DateTime.UtcNow
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddCanceledUtcIsNotNull()
        {
            this.Repository.Add(new FakeTaskRuntimeInfo()
            {
                CanceledUtc = DateTime.UtcNow
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddCompletedUtcIsNotNull()
        {
            this.Repository.Add(new FakeTaskRuntimeInfo()
            {
                CompletedUtc = DateTime.UtcNow
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddErrorNotNull()
        {
            this.Repository.Add(new FakeTaskRuntimeInfo()
            {
                Error = "Dummy"
            });
        }

        [TestMethod]
        public void Assign()
        {
            ITaskRuntimeInfo taskInfo1 = this.Repository.Add();

            Guid taskProcessorId = Guid.NewGuid();

            this.Repository.Assign(taskInfo1.TaskId, taskProcessorId);

            ITaskRuntimeInfo taskInfo2 = this.Repository.GetById(taskInfo1.TaskId);

            Assert.AreEqual(TaskStatus.Pending, taskInfo2.Status);
            Assert.AreEqual(taskProcessorId, taskInfo2.TaskProcessorId);

            Assert.IsTrue(this.Repository.CheckIsPendingOrActive(taskInfo1.TaskId));
            Assert.IsTrue(this.Repository.GetPending(false).Any(t => t.TaskId == taskInfo1.TaskId));
            Assert.IsTrue(this.Repository.GetPending(true).Any(t => t.TaskId == taskInfo1.TaskId));
            Assert.IsTrue(this.Repository.GetPendingAndActive()[TaskStatus.Pending].Any(t => t.TaskId == taskInfo1.TaskId));
            Assert.IsFalse(this.Repository.GetPendingAndActive()[TaskStatus.InProgress].Any(t => t.TaskId == taskInfo1.TaskId));
            Assert.IsFalse(this.Repository.GetActive().Any(t => t.TaskId == taskInfo1.TaskId));
            Assert.IsFalse(this.Repository.GetFailed().Any(t => t.TaskId == taskInfo1.TaskId));
            Assert.IsFalse(this.Repository.GetArchive().Any(t => t.TaskId == taskInfo1.TaskId));
        }

        [TestMethod]
        public void Start()
        {
            ITaskRuntimeInfo taskInfo1 = this.Repository.Add();

            Guid taskProcessorId = Guid.NewGuid();

            DateTime timestampUtc = DateTime.UtcNow;

            this.Repository.Start(taskInfo1.TaskId, taskProcessorId, timestampUtc);

            ITaskRuntimeInfo taskInfo2 = this.Repository.GetById(taskInfo1.TaskId);

            Assert.AreEqual(TaskStatus.InProgress, taskInfo2.Status);
            Assert.AreEqual(taskProcessorId, taskInfo2.TaskProcessorId);
            Assert.AreEqual(timestampUtc, taskInfo2.StartedUtc);

            Assert.IsTrue(this.Repository.CheckIsPendingOrActive(taskInfo1.TaskId));
            Assert.IsFalse(this.Repository.GetPending(false).Any(t => t.TaskId == taskInfo1.TaskId));
            Assert.IsFalse(this.Repository.GetPending(true).Any(t => t.TaskId == taskInfo1.TaskId));
            Assert.IsFalse(this.Repository.GetPendingAndActive()[TaskStatus.Pending].Any(t => t.TaskId == taskInfo1.TaskId));
            Assert.IsTrue(this.Repository.GetPendingAndActive()[TaskStatus.InProgress].Any(t => t.TaskId == taskInfo1.TaskId));
            Assert.IsTrue(this.Repository.GetActive().Any(t => t.TaskId == taskInfo1.TaskId));
            Assert.IsFalse(this.Repository.GetFailed().Any(t => t.TaskId == taskInfo1.TaskId));
            Assert.IsFalse(this.Repository.GetArchive().Any(t => t.TaskId == taskInfo1.TaskId));
        }

        [TestMethod]
        public void StartPollingQueueTask()
        {
            ITaskRuntimeInfo taskInfo1 = this.Repository.Add("Test");

            Guid taskProcessorId = Guid.NewGuid();

            DateTime timestampUtc = DateTime.UtcNow;

            this.Repository.Start(taskInfo1.TaskId, taskProcessorId, timestampUtc);

            ITaskRuntimeInfo taskInfo2 = this.Repository.GetById(taskInfo1.TaskId);

            Assert.AreEqual(TaskStatus.InProgress, taskInfo2.Status);
            Assert.AreEqual(taskProcessorId, taskInfo2.TaskProcessorId);
            Assert.AreEqual(timestampUtc, taskInfo2.StartedUtc);

            Assert.IsTrue(this.Repository.CheckIsPendingOrActive(taskInfo1.TaskId));
            Assert.IsFalse(this.Repository.GetPending(false).Any(t => t.TaskId == taskInfo1.TaskId));
            Assert.IsFalse(this.Repository.GetPending(true).Any(t => t.TaskId == taskInfo1.TaskId));
            Assert.IsFalse(this.Repository.GetPendingAndActive()[TaskStatus.Pending].Any(t => t.TaskId == taskInfo1.TaskId));
            Assert.IsTrue(this.Repository.GetPendingAndActive()[TaskStatus.InProgress].Any(t => t.TaskId == taskInfo1.TaskId));
            Assert.IsTrue(this.Repository.GetActive().Any(t => t.TaskId == taskInfo1.TaskId));
            Assert.IsFalse(this.Repository.GetFailed().Any(t => t.TaskId == taskInfo1.TaskId));
            Assert.IsFalse(this.Repository.GetArchive().Any(t => t.TaskId == taskInfo1.TaskId));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void UpdateProgressNegativePercent()
        {
            this.Repository.Progress(Guid.Empty, -1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void UpdateProgressPercent101()
        {
            this.Repository.Progress(Guid.Empty, 101);
        }

        [TestMethod]
        public void UpdateProgress()
        {
            ITaskRuntimeInfo taskInfo = this.Repository.Add();

            this.Repository.Progress(taskInfo.TaskId, 100);

            taskInfo = this.Repository.GetById(taskInfo.TaskId);

            Assert.AreEqual(100, taskInfo.Percentage);

            this.Repository.Progress(taskInfo.TaskId, 0);

            taskInfo = this.Repository.GetById(taskInfo.TaskId);

            Assert.AreEqual(0, taskInfo.Percentage);
        }

        [TestMethod]
        public void RequestCancelBeforeStart()
        {
            this.RequestCancel(false);
        }

        [TestMethod]
        public void RequestCancelAfterStart()
        {
            this.RequestCancel(true);
        }

        [TestMethod]
        public void CompleteCancel()
        {
            ITaskRuntimeInfo taskInfo = this.Repository.Add();

            this.Repository.Start(taskInfo.TaskId, Guid.NewGuid(), DateTime.UtcNow.AddMinutes(-1));

            DateTime timestampUtc = DateTime.UtcNow;

            this.Repository.RequestCancel(taskInfo.TaskId, timestampUtc.AddSeconds(-1));
            this.Repository.CompleteCancel(taskInfo.TaskId, timestampUtc.AddSeconds(1));

            ITaskRuntimeInfo taskInfo2 = this.Repository.GetById(taskInfo.TaskId);

            Assert.AreEqual(TaskStatus.Canceled, taskInfo2.Status);

            this.AssertEqual(timestampUtc.AddSeconds(-1), taskInfo2.CanceledUtc.Value);
            this.AssertEqual(timestampUtc.AddSeconds(1), taskInfo2.CompletedUtc.Value);

            Assert.IsFalse(this.Repository.CheckIsPendingOrActive(taskInfo.TaskId));
            Assert.IsFalse(this.Repository.GetPending(false).Any(t => t.TaskId == taskInfo.TaskId));
            Assert.IsFalse(this.Repository.GetPending(true).Any(t => t.TaskId == taskInfo.TaskId));
            Assert.IsFalse(this.Repository.GetPendingAndActive()[TaskStatus.Pending].Any(t => t.TaskId == taskInfo.TaskId));
            Assert.IsFalse(this.Repository.GetPendingAndActive()[TaskStatus.InProgress].Any(t => t.TaskId == taskInfo.TaskId));
            Assert.IsFalse(this.Repository.GetActive().Any(t => t.TaskId == taskInfo.TaskId));
            Assert.IsFalse(this.Repository.GetFailed().Any(t => t.TaskId == taskInfo.TaskId));
            Assert.IsTrue(this.Repository.GetArchive().Any(t => t.TaskId == taskInfo.TaskId));
        }

        [TestMethod]
        public void Fail()
        {
            ITaskRuntimeInfo taskInfo = this.Repository.Add();

            Guid taskProcessorId = Guid.NewGuid();

            DateTime timestampUtc = DateTime.UtcNow;

            Exception error = new TypeNotFoundInRedisException("Hello Error");

            this.Repository.Start(taskInfo.TaskId, taskProcessorId, DateTime.UtcNow.AddMinutes(-1));
            this.Repository.Fail(taskInfo.TaskId, timestampUtc, error);

            taskInfo = this.Repository.GetById(taskInfo.TaskId);

            Assert.AreEqual(TaskStatus.Failed, taskInfo.Status);

            this.AssertEqual(timestampUtc, taskInfo.CompletedUtc.Value);

            Assert.AreEqual(error.ToString(), taskInfo.Error);

            Assert.IsFalse(this.Repository.CheckIsPendingOrActive(taskInfo.TaskId));
            Assert.IsFalse(this.Repository.GetPending(false).Any(t => t.TaskId == taskInfo.TaskId));
            Assert.IsFalse(this.Repository.GetPending(true).Any(t => t.TaskId == taskInfo.TaskId));
            Assert.IsFalse(this.Repository.GetPendingAndActive()[TaskStatus.Pending].Any(t => t.TaskId == taskInfo.TaskId));
            Assert.IsFalse(this.Repository.GetPendingAndActive()[TaskStatus.InProgress].Any(t => t.TaskId == taskInfo.TaskId));
            Assert.IsFalse(this.Repository.GetActive().Any(t => t.TaskId == taskInfo.TaskId));
            Assert.IsTrue(this.Repository.GetFailed().Any(t => t.TaskId == taskInfo.TaskId));
            Assert.IsTrue(this.Repository.GetArchive().Any(t => t.TaskId == taskInfo.TaskId));
        }

        [TestMethod]
        public void Complete()
        {
            ITaskRuntimeInfo taskInfo1 = this.Repository.Add();

            Guid taskProcessorId = Guid.NewGuid();

            DateTime timestampUtc = DateTime.UtcNow;

            this.Repository.Start(taskInfo1.TaskId, taskProcessorId, DateTime.UtcNow.AddMinutes(-1));
            this.Repository.Complete(taskInfo1.TaskId, timestampUtc);

            ITaskRuntimeInfo taskInfo2 = this.Repository.GetById(taskInfo1.TaskId);

            Assert.AreEqual(100, taskInfo2.Percentage);

            Assert.AreEqual(TaskStatus.Success, taskInfo2.Status);

            this.AssertEqual(timestampUtc, taskInfo2.CompletedUtc.Value);

            Assert.IsFalse(this.Repository.CheckIsPendingOrActive(taskInfo1.TaskId));
            Assert.IsFalse(this.Repository.GetPending(false).Any(t => t.TaskId == taskInfo1.TaskId));
            Assert.IsFalse(this.Repository.GetPending(true).Any(t => t.TaskId == taskInfo1.TaskId));
            Assert.IsFalse(this.Repository.GetPendingAndActive()[TaskStatus.Pending].Any(t => t.TaskId == taskInfo1.TaskId));
            Assert.IsFalse(this.Repository.GetPendingAndActive()[TaskStatus.InProgress].Any(t => t.TaskId == taskInfo1.TaskId));
            Assert.IsFalse(this.Repository.GetActive().Any(t => t.TaskId == taskInfo1.TaskId));
            Assert.IsFalse(this.Repository.GetFailed().Any(t => t.TaskId == taskInfo1.TaskId));
            Assert.IsTrue(this.Repository.GetArchive().Any(t => t.TaskId == taskInfo1.TaskId));
        }

        [TestMethod]
        public void ReservePollingQueueTasks()
        {
            string pollingQueueKey = Guid.NewGuid().ToString();

            ITaskRuntimeInfo taskInfo1 = this.Repository.Add(pollingQueueKey);
            ITaskRuntimeInfo taskInfo2 = this.Repository.Add(pollingQueueKey);

            var result = this.Repository.ReservePollingQueueTasks(pollingQueueKey, 1);

            Assert.AreEqual(1, result.Count());
            Assert.IsTrue(result.Any(t => t.TaskId == taskInfo1.TaskId));
            Assert.IsFalse(result.Any(t => t.TaskId == taskInfo2.TaskId));

            result = this.Repository.ReservePollingQueueTasks(pollingQueueKey, 1);

            Assert.AreEqual(1, result.Count());
            Assert.IsFalse(result.Any(t => t.TaskId == taskInfo1.TaskId));
            Assert.IsTrue(result.Any(t => t.TaskId == taskInfo2.TaskId));

            Assert.IsFalse(this.Repository.ReservePollingQueueTasks(pollingQueueKey, 1).Any());
        }

        [TestMethod]
        public void GetTaskType()
        {
            Guid taskId = this.Repository.Add().TaskId;

            Assert.AreEqual(typeof(FakeTask), this.Repository.GetTaskType(taskId));
        }

        [TestMethod]
        public void GetArchiveTaskType()
        {
            Guid taskId = this.Repository.Add().TaskId;

            this.Repository.Complete(taskId, DateTime.UtcNow);

            Assert.AreEqual(typeof(FakeTask), this.Repository.GetTaskType(taskId));
        }

        [TestMethod]
        public void GetTaskTypeNull()
        {
            Assert.IsNull(this.Repository.GetTaskType(Guid.NewGuid()));
        }

        protected virtual void AssertEqual(DateTime value1, DateTime value2)
        {
            Assert.AreEqual(value1, value1);
        }

        private void RequestCancel(bool startTask)
        {
            ITaskRuntimeInfo taskInfo = this.Repository.Add();

            if (startTask)
            {
                this.Repository.Start(taskInfo.TaskId, Guid.NewGuid(), DateTime.UtcNow.AddMinutes(-1));
            }

            DateTime timestampUtc = DateTime.UtcNow;

            this.Repository.RequestCancel(taskInfo.TaskId, timestampUtc);

            ITaskRuntimeInfo taskInfo2 = this.Repository.GetById(taskInfo.TaskId);

            Assert.AreEqual(TaskStatus.Canceled, taskInfo2.Status);

            this.AssertEqual(timestampUtc, taskInfo2.CanceledUtc.Value);

            Assert.IsFalse(this.Repository.CheckIsPendingOrActive(taskInfo.TaskId));
            Assert.IsFalse(this.Repository.GetPending(false).Any(t => t.TaskId == taskInfo.TaskId));
            Assert.IsFalse(this.Repository.GetPending(true).Any(t => t.TaskId == taskInfo.TaskId));
            Assert.IsFalse(this.Repository.GetPendingAndActive()[TaskStatus.Pending].Any(t => t.TaskId == taskInfo.TaskId));
            Assert.IsFalse(this.Repository.GetPendingAndActive()[TaskStatus.InProgress].Any(t => t.TaskId == taskInfo.TaskId));
            Assert.IsFalse(this.Repository.GetActive().Any(t => t.TaskId == taskInfo.TaskId));
            Assert.IsFalse(this.Repository.GetFailed().Any(t => t.TaskId == taskInfo.TaskId));
            Assert.IsTrue(this.Repository.GetArchive().Any(t => t.TaskId == taskInfo.TaskId));
        }
    }
}