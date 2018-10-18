using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Redis.ServiceStack;
using Radoslav.TaskProcessor.Repository;
using Radoslav.TaskProcessor.Repository.Redis;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public sealed class RedisTaskProcessorRuntimeInfoRepositoryUnitTests : TaskProcessorRuntimeInfoRepositoryUnitTests
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
            ((RedisTaskProcessorRuntimeInfoRepository)this.Repository).Provider.Dispose();

            AssemblyUnitTests.ExitRedisLock();
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = SuppressMessages.DisposedOnTestCleanup)]
        protected override ITaskProcessorRuntimeInfoRepository CreateRepository()
        {
            return new RedisTaskProcessorRuntimeInfoRepository(new ServiceStackRedisProvider());
        }
    }
}