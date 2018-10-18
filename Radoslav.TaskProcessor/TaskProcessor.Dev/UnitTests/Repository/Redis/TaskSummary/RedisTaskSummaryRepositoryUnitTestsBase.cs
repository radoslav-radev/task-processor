using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Redis;
using Radoslav.Redis.ServiceStack;
using Radoslav.Serialization;
using Radoslav.TaskProcessor.Repository;
using Radoslav.TaskProcessor.Repository.Redis;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public abstract class RedisTaskSummaryRepositoryUnitTestsBase : TaskSummaryRepositoryUnitTestsBase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();

            AssemblyUnitTests.EnterRedisLock();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            ((RedisTaskSummaryRepository)this.Repository).Provider.Dispose();

            AssemblyUnitTests.ExitRedisLock();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullProvider()
        {
            new RedisTaskSummaryRepository(null, new FakeBinarySerializer());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullSerializer()
        {
            using (IRedisProvider provider = new FakeRedisProvider())
            {
                new RedisTaskSummaryRepository(provider, null);
            }
        }

        protected abstract IEntityBinarySerializer CreateSerializer();

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = SuppressMessages.DisposedOnTestCleanup)]
        protected override ITaskSummaryRepository CreateRepository()
        {
            return new RedisTaskSummaryRepository(new ServiceStackRedisProvider(), this.CreateSerializer());
        }
    }
}