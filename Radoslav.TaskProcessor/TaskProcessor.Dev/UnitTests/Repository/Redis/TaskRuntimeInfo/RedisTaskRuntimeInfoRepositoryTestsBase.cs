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
    public abstract class RedisTaskRuntimeInfoRepositoryTestsBase : TaskRuntimeInfoRepositoryUnitTestsBase
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
            ((RedisTaskRuntimeInfoRepository)this.Repository).Provider.Dispose();

            AssemblyUnitTests.ExitRedisLock();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullProvider()
        {
            new RedisTaskRuntimeInfoRepository(null, new FakeBinarySerializer());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullSerializer()
        {
            using (IRedisProvider provider = new FakeRedisProvider())
            {
                new RedisTaskRuntimeInfoRepository(provider, null);
            }
        }

        protected abstract IEntityBinarySerializer CreateSerializer();

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = SuppressMessages.DisposedOnTestCleanup)]
        protected override ITaskRuntimeInfoRepository CreateRepository()
        {
            return new RedisTaskRuntimeInfoRepository(new ServiceStackRedisProvider(), this.CreateSerializer());
        }
    }
}