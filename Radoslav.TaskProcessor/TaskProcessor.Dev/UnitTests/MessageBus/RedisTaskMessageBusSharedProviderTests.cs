using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Redis;
using Radoslav.Redis.ServiceStack;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public sealed class RedisTaskMessageBusSharedProviderTests : RedisTaskMessageBusTestsBase
    {
        private ServiceStackRedisProvider Provider
        {
            get
            {
                return (ServiceStackRedisProvider)this.TestContext.Properties["Provider"];
            }
        }

        [TestInitialize]
        public override void TestInitialize()
        {
            this.TestContext.Properties.Add("Provider", new ServiceStackRedisProvider());

            base.TestInitialize();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this.Provider.Dispose();
        }

        protected override IRedisProvider GetProvider()
        {
            return this.Provider;
        }
    }
}