using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Serialization;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public sealed class SerializeDictionaryTaskSummaryBinaryTests : SerializeDictionaryTaskSummaryTests<byte[]>
    {
        protected override IEntitySerializer<DictionaryTaskSummary, byte[]> CreateSerializer()
        {
            return new EntityBinarySerializer<DictionaryTaskSummary>();
        }
    }
}