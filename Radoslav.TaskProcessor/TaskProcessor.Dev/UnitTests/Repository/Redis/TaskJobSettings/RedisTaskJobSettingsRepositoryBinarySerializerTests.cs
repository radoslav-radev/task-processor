using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Serialization;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public sealed class RedisTaskJobSettingsRepositoryBinarySerializerTests : RedisTaskJobSettingsRepositoryTestsBase
    {
        protected override IEntityBinarySerializer CreateSerializer()
        {
            return new EntityBinarySerializer();
        }
    }
}