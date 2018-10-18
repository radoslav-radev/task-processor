using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.TaskProcessor.Model;
using Radoslav.TaskProcessor.TaskDistributor;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public sealed class SimpleTaskDistributorUnitTests
    {
        #region Properties & Initialize

        public TestContext TestContext { get; set; }

        private SimpleTaskDistributor TaskDistributor
        {
            get
            {
                return (SimpleTaskDistributor)this.TestContext.Properties["TaskDistributor"];
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
            this.TestContext.Properties.Add("TaskDistributor", new SimpleTaskDistributor(new FakeTaskProcessorRepository()));
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
        public void ChooseProcessorForTaskNoProcessors()
        {
            Assert.IsFalse(this.TaskDistributor.ChooseProcessorForTask(new FakeTaskRuntimeInfo()).Any());
        }

        [TestMethod]
        public void ChooseProcessorForTask()
        {
            ITaskProcessorRuntimeInfo processorInfo1 = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid()
            };

            this.Repository.TaskProcessorRuntimeInfo.Add(processorInfo1);

            ITaskProcessorRuntimeInfo processorInfo2 = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid()
            };

            this.Repository.TaskProcessorRuntimeInfo.Add(processorInfo2);

            ITaskRuntimeInfo activeTaskInfo = this.Repository.TaskRuntimeInfo.Create();

            this.Repository.TaskRuntimeInfo.Add(activeTaskInfo);

            this.Repository.TaskRuntimeInfo.Start(activeTaskInfo.TaskId, processorInfo1.TaskProcessorId, DateTime.UtcNow);

            var result = this.TaskDistributor.ChooseProcessorForTask(new FakeTaskRuntimeInfo());

            Assert.AreEqual(processorInfo2, result.First());
            Assert.AreEqual(processorInfo1, result.Last());
        }

        #endregion ChooseProcessorForTask

        #region ChooseNextTasksForProcessor

        [TestMethod]
        public void ChooseNextTaskForProcessorVeryHighHigh()
        {
            this.ChooseNextTaskForProcessorOrder(TaskPriority.VeryHigh, TaskPriority.High);
        }

        [TestMethod]
        public void ChooseNextTaskForProcessorVeryHighNormal()
        {
            this.ChooseNextTaskForProcessorOrder(TaskPriority.VeryHigh, TaskPriority.Normal);
        }

        [TestMethod]
        public void ChooseNextTaskForProcessorVeryHighLow()
        {
            this.ChooseNextTaskForProcessorOrder(TaskPriority.VeryHigh, TaskPriority.Low);
        }

        [TestMethod]
        public void ChooseNextTaskForProcessorHighNormal()
        {
            this.ChooseNextTaskForProcessorOrder(TaskPriority.High, TaskPriority.Normal);
        }

        [TestMethod]
        public void ChooseNextTaskForProcessorHighLow()
        {
            this.ChooseNextTaskForProcessorOrder(TaskPriority.High, TaskPriority.Low);
        }

        [TestMethod]
        public void ChooseNextTaskForProcessorNormalLow()
        {
            this.ChooseNextTaskForProcessorOrder(TaskPriority.Normal, TaskPriority.Low);
        }

        #endregion ChooseNextTasksForProcessor

        private void ChooseNextTaskForProcessorOrder(TaskPriority higher, TaskPriority smaller)
        {
            ITaskRuntimeInfo taskInfo1 = this.Repository.TaskRuntimeInfo.Add(smaller);
            ITaskRuntimeInfo taskInfo2 = this.Repository.TaskRuntimeInfo.Add(higher);

            IEnumerable<ITaskRuntimeInfo> result = this.TaskDistributor.ChooseNextTasksForProcessor(Guid.NewGuid());

            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(taskInfo2, result.First());
            Assert.AreEqual(taskInfo1, result.Last());
        }
    }
}