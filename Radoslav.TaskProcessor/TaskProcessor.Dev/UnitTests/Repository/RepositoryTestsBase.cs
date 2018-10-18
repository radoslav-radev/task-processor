using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public abstract class RepositoryTestsBase<TRepository>
    {
        protected static readonly object LockObject = new object();

        public TestContext TestContext { get; set; }

        protected TRepository Repository
        {
            get
            {
                return (TRepository)this.TestContext.Properties["Repository"];
            }
        }

        [TestInitialize]
        public virtual void TestInitialize()
        {
            this.TestContext.Properties.Add("Repository", this.CreateRepository());
        }

        protected abstract TRepository CreateRepository();
    }
}