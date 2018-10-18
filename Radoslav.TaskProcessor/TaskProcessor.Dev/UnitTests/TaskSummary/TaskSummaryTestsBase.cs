using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Serialization;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public abstract class TaskSummaryTestsBase<TTaskSummary, TSerializationData>
        where TTaskSummary : ITaskSummary
    {
        [TestMethod]
        public void Serialization()
        {
            TTaskSummary summary = this.CreateTaskSummary(false);

            this.AssertSerialization(summary, false);
        }

        [TestMethod]
        public void SerializationEmpty()
        {
            TTaskSummary summary = this.CreateTaskSummary(true);

            this.AssertSerialization(summary, true);
        }

        protected abstract TTaskSummary CreateTaskSummary(bool empty);

        protected abstract void AssertIsEmpty(TTaskSummary summary);

        protected abstract void AssertAreEqual(TTaskSummary first, TTaskSummary second);

        protected abstract IEntitySerializer<TTaskSummary, TSerializationData> CreateSerializer();

        protected void AssertSerialization(TTaskSummary summary, bool empty)
        {
            var serializer = this.CreateSerializer();

            TSerializationData content = serializer.Serialize(summary);

            TTaskSummary summary2 = serializer.Deserialize(content, typeof(TTaskSummary));

            if (empty)
            {
                this.AssertIsEmpty(summary);
            }
            else
            {
                this.AssertAreEqual(summary, summary2);
            }
        }
    }
}