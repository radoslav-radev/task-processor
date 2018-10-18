using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.TaskProcessor.Repository;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public abstract class TaskRepositoryUnitTestsBase
    {
        public TestContext TestContext { get; set; }

        protected ITaskRepository Repository
        {
            get
            {
                return (ITaskRepository)this.TestContext.Properties["Repository"];
            }
        }

        [TestInitialize]
        public virtual void TestInitialize()
        {
            this.TestContext.Properties.Add("Repository", this.CreateRepository());
        }

        [TestMethod]
        public void AddTask()
        {
            Guid taskId = Guid.NewGuid();

            FakeTask task1 = new FakeTask()
            {
                StringValue = "Hello World",
                NumberValue = DateTime.UtcNow.Millisecond
            };

            this.Repository.Add(taskId, task1);

            FakeTask task2 = (FakeTask)this.Repository.GetById(taskId);

            UnitTestHelpers.AssertEqualByPublicScalarProperties(task1, task2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddNullTask()
        {
            this.Repository.Add(Guid.NewGuid(), null);
        }

        [TestMethod]
        public void TaskNotFound()
        {
            Assert.IsNull(this.Repository.GetById(Guid.NewGuid()));
        }

        [TestMethod]
        public void Delete()
        {
            Guid taskId = Guid.NewGuid();

            FakeTask task = new FakeTask()
            {
                StringValue = "Hello World",
                NumberValue = DateTime.UtcNow.Millisecond
            };

            this.Repository.Add(taskId, task);

            Assert.IsNotNull(this.Repository.GetById(taskId));

            this.Repository.Delete(taskId);

            Assert.IsNull(this.Repository.GetById(taskId));
        }

        protected abstract ITaskRepository CreateRepository();
    }
}