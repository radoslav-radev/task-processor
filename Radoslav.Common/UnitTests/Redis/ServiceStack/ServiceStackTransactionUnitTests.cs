using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Redis;
using Radoslav.Redis.ServiceStack;

namespace Radoslav.Common.UnitTests
{
    [TestClass]
    public sealed class ServiceStackTransactionUnitTests : RedisTransactionUnitTests
    {
        protected override IRedisProvider CreateRedisProvider()
        {
            return new ServiceStackRedisProvider();
        }
    }
}