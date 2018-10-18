using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public abstract class SerializeStringTaskSummaryTests<TSerializationData> : TaskSummaryTestsBase<StringTaskSummary, TSerializationData>
    {
        [TestMethod]
        public void SerializationEmptyString()
        {
            this.AssertSerialization(new StringTaskSummary(string.Empty), true);
        }

        protected override StringTaskSummary CreateTaskSummary(bool empty)
        {
            return empty ? new StringTaskSummary() : new StringTaskSummary("Hello World");
        }

        protected override void AssertIsEmpty(StringTaskSummary summary)
        {
            Assert.IsTrue(string.IsNullOrEmpty(summary.Summary));
        }

        protected override void AssertAreEqual(StringTaskSummary first, StringTaskSummary second)
        {
            Assert.AreEqual(first.Summary, second.Summary);
        }
    }
}