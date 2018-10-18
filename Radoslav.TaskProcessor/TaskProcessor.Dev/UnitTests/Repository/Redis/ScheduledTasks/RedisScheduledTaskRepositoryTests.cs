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
    public abstract class RedisScheduledTaskRepositoryTests : ScheduledTaskRepositoryTests
    {
        protected new RedisScheduledTaskRepository Repository
        {
            get
            {
                return (RedisScheduledTaskRepository)base.Repository;
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
            this.Repository.Dispose();
            this.Repository.Provider.Dispose();

            AssemblyUnitTests.ExitRedisLock();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullProvider()
        {
            new RedisScheduledTaskRepository(null, new FakeBinarySerializer());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullSerializer()
        {
            using (IRedisProvider provider = new FakeRedisProvider())
            {
                new RedisScheduledTaskRepository(provider, null);
            }
        }

        protected abstract IEntityBinarySerializer CreateSerializer();

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = SuppressMessages.DisposedOnTestCleanup)]
        protected override IScheduledTaskRepository CreateRepository()
        {
            return new RedisScheduledTaskRepository(new ServiceStackRedisProvider(), this.CreateSerializer());
        }
    }
}