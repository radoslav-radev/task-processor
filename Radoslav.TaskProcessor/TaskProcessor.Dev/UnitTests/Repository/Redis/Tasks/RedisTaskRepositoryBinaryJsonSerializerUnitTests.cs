using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Serialization;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public sealed class RedisTaskRepositoryJsonSerializerUnitTests : RedisTaskRepositoryUnitTestsBase
    {
        protected override IEntityBinarySerializer CreateSerializer()
        {
            return new EntityBinaryJsonSerializer();
        }
    }
}