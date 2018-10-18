using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Serialization;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public sealed class RedisTaskJobSettingsRepositoryBinaryJsonSerializerTests : RedisTaskJobSettingsRepositoryTestsBase
    {
        protected override IEntityBinarySerializer CreateSerializer()
        {
            return new EntityBinaryJsonSerializer();
        }
    }
}