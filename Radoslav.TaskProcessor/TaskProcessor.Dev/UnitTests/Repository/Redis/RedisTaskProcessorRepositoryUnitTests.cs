using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Redis;
using Radoslav.TaskProcessor.Repository.Redis;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public sealed class RedisTaskProcessorRepositoryUnitTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullProvider()
        {
            new RedisTaskProcessorRepository(null, new FakeBinarySerializer());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullSerialize()
        {
            using (IRedisProvider provider = new FakeRedisProvider())
            {
                new RedisTaskProcessorRepository(provider, null);
            }
        }
    }
}