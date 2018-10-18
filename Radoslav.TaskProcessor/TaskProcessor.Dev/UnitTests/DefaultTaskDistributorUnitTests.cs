using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.TaskProcessor.Model;
using Radoslav.TaskProcessor.TaskDistributor;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public sealed class DefaultTaskDistributorUnitTests
    {
        #region Properties & Initialize

        public TestContext TestContext { get; set; }

        private DefaultTaskDistributor TaskDistributor
        {
            get
            {
                return (DefaultTaskDistributor)this.TestContext.Properties["TaskDistributor"];
            }
        }

        private FakeTaskProcessorRepository Repository
        {
            get
            {
                return (FakeTaskProcessorRepository)this.TaskDistributor.Repository;
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            this.TestContext.Properties.Add("TaskDistributor", new DefaultTaskDistributor(new FakeTaskProcessorRepository()));
        }

        #endregion Properties & Initialize

        #region ChooseProcessorForTask

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ChooseProcessorForTaskNullPendingTask()
        {
            this.TaskDistributor.ChooseProcessorForTask(null);
        }

        [TestMethod]
        public void ChooseProcessorForTaskNoConfiguration()
        {
            FakeTaskProcessorRuntimeInfo taskProcessorInfo = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid(),
                Configuration = new FakeTaskProcessorConfiguration()
            };

            this.Repository.TaskProcessorRuntimeInfo.Add(taskProcessorInfo);

            var result = this.TaskDistributor.ChooseProcessorForTask(new FakeTaskRuntimeInfo() { TaskType = typeof(FakeTask) });

            Assert.AreEqual(taskProcessorInfo, result.Single());
        }

        [TestMethod]
        public void ChooseProcessorForTaskEmptyConfiguration()
        {
            FakeTaskProcessorRuntimeInfo taskProcessorInfo = new FakeTaskProcessorRuntimeInfo()
            {
                Configuration = new FakeTaskProcessorConfiguration()
            };

            this.Repository.TaskProcessorRuntimeInfo.Add(taskProcessorInfo);

            var result = this.TaskDistributor.ChooseProcessorForTask(new FakeTaskRuntimeInfo()
            {
                TaskType = typeof(FakeTask)
            });

            Assert.AreEqual(taskProcessorInfo, result.Single());
        }

        [TestMethod]
        public void ChooseProcessorForTaskNoProcessors()
        {
            Assert.IsFalse(this.TaskDistributor.ChooseProcessorForTask(new FakeTaskRuntimeInfo()).Any());
        }

        [TestMethod]
        public void ChooseProcessorForTaskExceedProcessorMaxWorkers()
        {
            ITaskProcessorRuntimeInfo processorInfo = new FakeTaskProcessorRuntimeInfo()
            {
                Configuration = new FakeTaskProcessorConfiguration()
            };

            processorInfo.Configuration.Tasks.MaxWorkers = 0;

            this.Repository.TaskProcessorRuntimeInfo.Add(processorInfo);

            Assert.IsFalse(this.TaskDistributor.ChooseProcessorForTask(new FakeTaskRuntimeInfo()).Any());
        }

        [TestMethod]
        public void ChooseProcessorForTaskExceedProcessorTaskJobMaxWorkers()
        {
            ITaskProcessorRuntimeInfo processorInfo = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid(),
                Configuration = new FakeTaskProcessorConfiguration()
            };

            processorInfo.Configuration.Tasks.Add(typeof(FakeTask)).MaxWorkers = 0;

            this.Repository.TaskProcessorRuntimeInfo.Add(processorInfo);

            Assert.IsFalse(this.TaskDistributor.ChooseProcessorForTask(new FakeTaskRuntimeInfo() { TaskType = typeof(FakeTask) }).Any());
        }

        [TestMethod]
        public void ChooseProcessorForTask()
        {
            ITaskProcessorRuntimeInfo processorInfo = new FakeTaskProcessorRuntimeInfo()
            {
                Configuration = new FakeTaskProcessorConfiguration()
            };

            processorInfo.Configuration.Tasks.MaxWorkers = 1;
            processorInfo.Configuration.Tasks.Add(typeof(FakeTask)).MaxWorkers = 1;

            this.Repository.TaskProcessorRuntimeInfo.Add(processorInfo);

            Assert.AreEqual(processorInfo, this.TaskDistributor.ChooseProcessorForTask(new FakeTaskRuntimeInfo() { TaskType = typeof(FakeTask) }).Single());
        }

        [TestMethod]
        public void ChooseProcessorForTaskOrder()
        {
            ITaskProcessorRuntimeInfo processorInfo1 = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid(),
                Configuration = new FakeTaskProcessorConfiguration()
            };

            this.Repository.TaskProcessorRuntimeInfo.Add(processorInfo1);

            ITaskProcessorRuntimeInfo processorInfo2 = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid(),
                Configuration = new FakeTaskProcessorConfiguration()
            };

            this.Repository.TaskProcessorRuntimeInfo.Add(processorInfo2);

            ITaskRuntimeInfo activeTaskInfo = this.Repository.TaskRuntimeInfo.Create();

            this.Repository.TaskRuntimeInfo.Add(activeTaskInfo);
            this.Repository.TaskRuntimeInfo.Start(activeTaskInfo.TaskId, processorInfo1.TaskProcessorId, DateTime.UtcNow);

            var result = this.TaskDistributor.ChooseProcessorForTask(new FakeTaskRuntimeInfo()
            {
                TaskType = typeof(FakeTask)
            });

            Assert.AreEqual(2, result.Count());

            Assert.AreEqual(processorInfo2, result.First());
            Assert.AreEqual(processorInfo1, result.Last());
        }

        #endregion ChooseProcessorForTask

        #region ChooseNextTasksForProcessor

        [TestMethod]
        public void ChooseNextTaskForProcessorNoProcessor()
        {
            ITaskRuntimeInfo pendingTaskInfo = this.Repository.TaskRuntimeInfo.Create();

            this.Repository.TaskRuntimeInfo.Add(pendingTaskInfo);

            Assert.IsFalse(this.TaskDistributor.ChooseNextTasksForProcessor(Guid.NewGuid()).Any());
        }

        [TestMethod]
        public void ChooseNextTaskForProcessorExceedProcessorMaxWorkers()
        {
            FakeTaskProcessorRuntimeInfo taskProcessorInfo = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid(),
                Configuration = new FakeTaskProcessorConfiguration()
            };

            taskProcessorInfo.Configuration.Tasks.MaxWorkers = 1;

            this.Repository.TaskProcessorRuntimeInfo.Add(taskProcessorInfo);

            ITaskRuntimeInfo pendingTaskInfo = this.Repository.TaskRuntimeInfo.Create();

            this.Repository.TaskRuntimeInfo.Add(pendingTaskInfo);

            ITaskRuntimeInfo activeTaskInfo = this.Repository.TaskRuntimeInfo.Create();

            this.Repository.TaskRuntimeInfo.Add(activeTaskInfo);

            this.Repository.TaskRuntimeInfo.Start(activeTaskInfo.TaskId, taskProcessorInfo.TaskProcessorId, DateTime.UtcNow);

            Assert.IsFalse(this.TaskDistributor.ChooseNextTasksForProcessor(taskProcessorInfo.TaskProcessorId).Any());
        }

        [TestMethod]
        public void ChooseNextTaskForProcessorExceedProcessorTaskJobMaxWorkers()
        {
            FakeTaskProcessorRuntimeInfo taskProcessorInfo = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid(),
                Configuration = new FakeTaskProcessorConfiguration()
            };

            taskProcessorInfo.Configuration.Tasks.Add(typeof(FakeTask)).MaxWorkers = 1;

            this.Repository.TaskProcessorRuntimeInfo.Add(taskProcessorInfo);

            ITaskRuntimeInfo pendingTaskInfo = this.Repository.TaskRuntimeInfo.Create();

            this.Repository.TaskRuntimeInfo.Add(pendingTaskInfo);

            ITaskRuntimeInfo activeTaskInfo = this.Repository.TaskRuntimeInfo.Create();

            this.Repository.TaskRuntimeInfo.Add(activeTaskInfo);

            this.Repository.TaskRuntimeInfo.Start(activeTaskInfo.TaskId, taskProcessorInfo.TaskProcessorId, DateTime.UtcNow);

            Assert.IsFalse(this.TaskDistributor.ChooseNextTasksForProcessor(taskProcessorInfo.TaskProcessorId).Any());
        }

        [TestMethod]
        public void ChooseNextTaskForProcessorOrderNormalBeforeLow()
        {
            this.ChooseNextTaskForProcessorOrder(TaskPriority.Normal, TaskPriority.Low);
        }

        [TestMethod]
        public void ChooseNextTaskForProcessorOrderHighBeforeLow()
        {
            this.ChooseNextTaskForProcessorOrder(TaskPriority.High, TaskPriority.Low);
        }

        [TestMethod]
        public void ChooseNextTaskForProcessorOrderVeryHighBeforeLow()
        {
            this.ChooseNextTaskForProcessorOrder(TaskPriority.VeryHigh, TaskPriority.Low);
        }

        [TestMethod]
        public void ChooseNextTaskForProcessorOrderHighBeforeNormal()
        {
            this.ChooseNextTaskForProcessorOrder(TaskPriority.High, TaskPriority.Normal);
        }

        [TestMethod]
        public void ChooseNextTaskForProcessorOrderVeryHighBeforeNormal()
        {
            this.ChooseNextTaskForProcessorOrder(TaskPriority.VeryHigh, TaskPriority.Normal);
        }

        [TestMethod]
        public void ChooseNextTaskForProcessorOrderVeryHighBeforeHigh()
        {
            this.ChooseNextTaskForProcessorOrder(TaskPriority.VeryHigh, TaskPriority.High);
        }

        [TestMethod]
        public void ChooseNextTaskForProcessorSkipProcessorMaxWorkers()
        {
            FakeTaskProcessorRuntimeInfo taskProcessorInfo = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid(),
                Configuration = new FakeTaskProcessorConfiguration()
            };

            taskProcessorInfo.Configuration.Tasks.MaxWorkers = 1;

            this.Repository.TaskProcessorRuntimeInfo.Add(taskProcessorInfo);

            FakeTaskRuntimeInfo pendingTaskToSkip = this.Repository.TaskRuntimeInfo.Create<FakeTaskRuntimeInfo>();

            pendingTaskToSkip.Priority = TaskPriority.Low;

            this.Repository.TaskRuntimeInfo.Add(pendingTaskToSkip);

            FakeTaskRuntimeInfo pendingTask = this.Repository.TaskRuntimeInfo.Create<FakeTaskRuntimeInfo>();

            pendingTask.Priority = TaskPriority.Normal;

            this.Repository.TaskRuntimeInfo.Add(pendingTask);

            IEnumerable<ITaskRuntimeInfo> result = this.TaskDistributor.ChooseNextTasksForProcessor(taskProcessorInfo.TaskProcessorId);

            Assert.AreEqual(pendingTask, result.Single());
        }

        [TestMethod]
        public void ChooseNextTaskForProcessorSkipProcessorTaskJob()
        {
            FakeTaskProcessorRuntimeInfo taskProcessorInfo = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid(),
                Configuration = new FakeTaskProcessorConfiguration()
            };

            taskProcessorInfo.Configuration.Tasks.Add(typeof(FakeTask)).MaxWorkers = 1;

            this.Repository.TaskProcessorRuntimeInfo.Add(taskProcessorInfo);

            FakeTaskRuntimeInfo pendingTaskToSkip = this.Repository.TaskRuntimeInfo.Create<FakeTaskRuntimeInfo>();

            pendingTaskToSkip.Priority = TaskPriority.Low;

            this.Repository.TaskRuntimeInfo.Add(pendingTaskToSkip);

            FakeTaskRuntimeInfo pendingTask = this.Repository.TaskRuntimeInfo.Create<FakeTaskRuntimeInfo>();

            pendingTask.Priority = TaskPriority.High;

            this.Repository.TaskRuntimeInfo.Add(pendingTask);

            IEnumerable<ITaskRuntimeInfo> result = this.TaskDistributor.ChooseNextTasksForProcessor(taskProcessorInfo.TaskProcessorId);

            Assert.AreEqual(pendingTask, result.Single());
        }

        [TestMethod]
        public void ChooseNextTaskForProcessor()
        {
            ITaskProcessorRuntimeInfo processorInfo1 = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid(),
                Configuration = new FakeTaskProcessorConfiguration()
            };

            processorInfo1.Configuration.Tasks.MaxWorkers = 4;
            processorInfo1.Configuration.Tasks.Add(typeof(IFakeTask)).MaxWorkers = 2;

            this.Repository.TaskProcessorRuntimeInfo.Add(processorInfo1);

            ITaskProcessorRuntimeInfo processorInfo2 = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid(),
                Configuration = new FakeTaskProcessorConfiguration()
            };

            processorInfo2.Configuration.Tasks.MaxWorkers = 1;

            this.Repository.TaskProcessorRuntimeInfo.Add(processorInfo2);

            ITaskRuntimeInfo pendingTaskInfo1 = this.Repository.TaskRuntimeInfo.Add(TaskPriority.High);
            ITaskRuntimeInfo pendingTaskInfo2 = this.Repository.TaskRuntimeInfo.Add(TaskPriority.VeryHigh);

            ITaskRuntimeInfo activeTaskInfo1 = this.Repository.TaskRuntimeInfo.Add();
            ITaskRuntimeInfo activeTaskInfo2 = this.Repository.TaskRuntimeInfo.Add();
            ITaskRuntimeInfo activeTaskInfo3 = this.Repository.TaskRuntimeInfo.Add();

            this.Repository.TaskRuntimeInfo.Start(activeTaskInfo1.TaskId, Guid.NewGuid(), DateTime.UtcNow);
            this.Repository.TaskRuntimeInfo.Start(activeTaskInfo2.TaskId, processorInfo1.TaskProcessorId, DateTime.UtcNow);
            this.Repository.TaskRuntimeInfo.Start(activeTaskInfo3.TaskId, processorInfo1.TaskProcessorId, DateTime.UtcNow);

            IEnumerable<ITaskRuntimeInfo> result = this.TaskDistributor.ChooseNextTasksForProcessor(processorInfo1.TaskProcessorId);

            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(pendingTaskInfo2, result.First());
            Assert.AreEqual(pendingTaskInfo1, result.Last());
        }

        #endregion ChooseNextTasksForProcessor

        private void ChooseNextTaskForProcessorOrder(TaskPriority higher, TaskPriority smaller)
        {
            FakeTaskProcessorRuntimeInfo taskProcessorInfo = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid(),
                Configuration = new FakeTaskProcessorConfiguration()
            };

            this.Repository.TaskProcessorRuntimeInfo.Add(taskProcessorInfo);

            FakeTaskRuntimeInfo pendingTaskInfo1 = this.Repository.TaskRuntimeInfo.Create<FakeTaskRuntimeInfo>();

            pendingTaskInfo1.Priority = smaller;

            this.Repository.TaskRuntimeInfo.Add(pendingTaskInfo1);

            FakeTaskRuntimeInfo pendingTaskInfo2 = this.Repository.TaskRuntimeInfo.Create<FakeTaskRuntimeInfo>();

            pendingTaskInfo1.Priority = higher;

            this.Repository.TaskRuntimeInfo.Add(pendingTaskInfo2);

            var result = this.TaskDistributor.ChooseNextTasksForProcessor(taskProcessorInfo.TaskProcessorId);

            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(pendingTaskInfo2, result.First());
            Assert.AreEqual(pendingTaskInfo1, result.Last());
        }
    }
}