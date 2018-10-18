using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Redis.ServiceStack;
using Radoslav.Serialization;
using Radoslav.TaskProcessor.Repository;
using Radoslav.TaskProcessor.Repository.Redis;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public abstract class RedisTaskJobSettingsRepositoryTestsBase : TaskJobSettingsRepositoryTestsBase
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
            AssemblyUnitTests.ExitRedisLock();
        }

        protected abstract IEntityBinarySerializer CreateSerializer();

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = SuppressMessages.DisposedOnTestCleanup)]
        protected override ITaskJobSettingsRepository CreateRepository()
        {
            return new RedisTaskJobSettingsRepository(new ServiceStackRedisProvider(), this.CreateSerializer());
        }
    }
}