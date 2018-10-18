using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Serialization;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public sealed class RedisScheduledTaskRepositoryBinarySerializerTests : RedisScheduledTaskRepositoryTests
    {
        protected override IEntityBinarySerializer CreateSerializer()
        {
            return new EntityBinarySerializer();
        }
    }
}