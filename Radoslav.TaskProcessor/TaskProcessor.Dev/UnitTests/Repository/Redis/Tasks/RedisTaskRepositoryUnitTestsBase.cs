using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Redis.ServiceStack;
using Radoslav.Serialization;
using Radoslav.TaskProcessor.Repository;
using Radoslav.TaskProcessor.Repository.Redis;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public abstract class RedisTaskRepositoryUnitTestsBase : TaskRepositoryUnitTestsBase
    {
        protected new RedisTaskRepository Repository
        {
            get
            {
                return (RedisTaskRepository)base.Repository;
            }
        }

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();

            AssemblyUnitTests.EnterRedisLock();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this.Repository.Provider.Dispose();

            AssemblyUnitTests.ExitRedisLock();
        }

        protected abstract IEntityBinarySerializer CreateSerializer();

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = SuppressMessages.DisposedOnTestCleanup)]
        protected override ITaskRepository CreateRepository()
        {
            return new RedisTaskRepository(new ServiceStackRedisProvider(), this.CreateSerializer());
        }
    }
}