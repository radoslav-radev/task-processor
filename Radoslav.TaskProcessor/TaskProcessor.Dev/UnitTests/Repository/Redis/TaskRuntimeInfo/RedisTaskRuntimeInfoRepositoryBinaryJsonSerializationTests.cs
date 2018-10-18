using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Serialization;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public sealed class RedisTaskRuntimeInfoRepositoryBinaryJsonSerializationTests : RedisTaskRuntimeInfoRepositoryTestsBase
    {
        protected override IEntityBinarySerializer CreateSerializer()
        {
            return new EntityBinaryJsonSerializer();
        }

        protected override void AssertEqual(DateTime value1, DateTime value2)
        {
            UnitTestHelpers.AssertEqualToMillisecond(value1, value2);
        }
    }
}