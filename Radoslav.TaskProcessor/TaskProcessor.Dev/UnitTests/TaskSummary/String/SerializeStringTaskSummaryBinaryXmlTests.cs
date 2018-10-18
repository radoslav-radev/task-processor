using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Serialization;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public sealed class SerializeStringTaskSummaryBinaryXmlTests : SerializeStringTaskSummaryTests<byte[]>
    {
        protected override IEntitySerializer<StringTaskSummary, byte[]> CreateSerializer()
        {
            return new EntityBinaryXmlSerializer<StringTaskSummary>();
        }
    }
}