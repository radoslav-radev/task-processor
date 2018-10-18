using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Serialization;

namespace Radoslav.Common.UnitTests
{
    [TestClass]
    public sealed class XmlSerializerUnitTests
    {
        public TestContext TestContext { get; set; }

        private IEntityStringSerializer Serializer
        {
            get
            {
                return (IEntityStringSerializer)this.TestContext.Properties["Serializer"];
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            this.TestContext.Properties.Add("Serializer", new EntityXmlSerializer());
        }

        [TestMethod]
        public void CanDetermineEntityTypeFromContent()
        {
            Assert.IsFalse(this.Serializer.CanDetermineEntityTypeFromContent);
        }

        [TestMethod]
        public void SerializeAndDeserializeNull()
        {
            string content = this.Serializer.Serialize(null);

            Assert.AreEqual(string.Empty, content);

            Assert.IsNull(this.Serializer.Deserialize(null, typeof(FakeEntity)));
            Assert.IsNull(this.Serializer.Deserialize(string.Empty, typeof(FakeEntity)));
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void SerializeAndDeserializeWithoutType()
        {
            this.Serializer.Deserialize("Hello");
        }

        [TestMethod]
        public void SerializeAndDeserializeWithType()
        {
            FakeEntity entity1 = new FakeEntity()
            {
                StringValue = "A",
                IntValue = 19
            };

            string content = this.Serializer.Serialize(entity1);

            FakeEntity entity2 = (FakeEntity)this.Serializer.Deserialize(content, entity1.GetType());

            UnitTestHelpers.AssertEqualByPublicScalarProperties(entity1, entity2);
        }
    }
}