using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Serialization;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public sealed class RedisTaskRepositoryBinarySerializerUnitTests : RedisTaskRepositoryUnitTestsBase
    {
        protected override IEntityBinarySerializer CreateSerializer()
        {
            return new EntityBinarySerializer();
        }
    }
}