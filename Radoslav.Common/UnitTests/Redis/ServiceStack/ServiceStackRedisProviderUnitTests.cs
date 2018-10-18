using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Redis;
using Radoslav.Redis.ServiceStack;

namespace Radoslav.Common.UnitTests
{
    [TestClass]
    public sealed class ServiceStackRedisProviderTests : RedisProviderUnitTests
    {
        [TestMethod]
        public void ConstructorNullHost1()
        {
            new ServiceStackRedisProvider(null);
        }

        [TestMethod]
        public void ConstructorNullHost2()
        {
            new ServiceStackRedisProvider(null, 0, "Password", 0);
        }

        [TestMethod]
        public void ConstructorEmptyHost1()
        {
            new ServiceStackRedisProvider(string.Empty);
        }

        [TestMethod]
        public void ConstructorEmptyHost2()
        {
            new ServiceStackRedisProvider(string.Empty, 0, "Password", 0);
        }

        [TestMethod]
        public void ConstructorNullPassword()
        {
            new ServiceStackRedisProvider("localhost", 0, null, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void AcquireLockZeroTimeout()
        {
            using (this.RedisProvider.AcquireLock("Key", TimeSpan.Zero))
            {
            }
        }

        protected override IRedisProvider CreateRedisProvider()
        {
            return new ServiceStackRedisProvider();
        }
    }
}