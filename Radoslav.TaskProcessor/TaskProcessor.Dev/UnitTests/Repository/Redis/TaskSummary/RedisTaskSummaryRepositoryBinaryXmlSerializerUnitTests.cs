using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Serialization;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public sealed class RedisTaskSummaryRepositoryBinaryXmlSerializerUnitTests : RedisTaskSummaryRepositoryUnitTestsBase
    {
        protected override IEntityBinarySerializer CreateSerializer()
        {
            return new EntityBinaryXmlSerializer();
        }
    }
}