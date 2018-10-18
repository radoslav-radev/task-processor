using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Redis;
using Radoslav.Redis.ServiceStack;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public sealed class RedisTaskMessageBusTests : RedisTaskMessageBusTestsBase
    {
        private ICollection<IRedisProvider> Providers
        {
            get
            {
                return (ICollection<IRedisProvider>)this.TestContext.Properties["Providers"];
            }
        }

        [TestInitialize]
        public override void TestInitialize()
        {
            this.TestContext.Properties.Add("Providers", new List<IRedisProvider>());

            base.TestInitialize();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            foreach (IRedisProvider provider in this.Providers)
            {
                provider.Dispose();
            }
        }

        protected override IRedisProvider GetProvider()
        {
            IRedisProvider result = new ServiceStackRedisProvider();

            this.Providers.Add(result);

            return result;
        }
    }
}