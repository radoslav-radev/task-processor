using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.TaskProcessor.Model;
using Radoslav.TaskProcessor.Repository;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public abstract class TaskSummaryRepositoryUnitTestsBase : RepositoryTestsBase<ITaskSummaryRepository>
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddNullSummary()
        {
            this.Repository.Add(Guid.NewGuid(), null);
        }

        [TestMethod]
        public void Add()
        {
            Guid taskId = Guid.NewGuid();

            StringTaskSummary summary1 = new StringTaskSummary("Hello World");

            this.Repository.Add(taskId, summary1);

            StringTaskSummary summary2 = (StringTaskSummary)this.Repository.GetById(taskId);

            Assert.AreEqual(summary1.Summary, summary2.Summary);
        }
    }
}