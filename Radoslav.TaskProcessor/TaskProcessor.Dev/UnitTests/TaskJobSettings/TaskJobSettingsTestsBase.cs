using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Serialization;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public abstract class TaskJobSettingsTestsBase<TTaskJobSettings, TSerializationData>
        where TTaskJobSettings : ITaskJobSettings
    {
        [TestMethod]
        public void Serialization()
        {
            TTaskJobSettings settings = this.CreateTaskJobSettings(false);

            this.AssertSerialization(settings, false);
        }

        [TestMethod]
        public void SerializationEmpty()
        {
            TTaskJobSettings settings = this.CreateTaskJobSettings(true);

            this.AssertSerialization(settings, true);
        }

        protected abstract TTaskJobSettings CreateTaskJobSettings(bool empty);

        protected abstract void AssertIsEmpty(TTaskJobSettings settings);

        protected abstract void AssertAreEqual(TTaskJobSettings first, TTaskJobSettings second);

        protected abstract IEntitySerializer<TTaskJobSettings, TSerializationData> CreateSerializer();

        protected void AssertSerialization(TTaskJobSettings settings, bool empty)
        {
            var serializer = this.CreateSerializer();

            TSerializationData content = serializer.Serialize(settings);

            TTaskJobSettings settings2 = serializer.Deserialize(content, typeof(TTaskJobSettings));

            if (empty)
            {
                this.AssertIsEmpty(settings);
            }
            else
            {
                this.AssertAreEqual(settings, settings2);
            }
        }
    }
}