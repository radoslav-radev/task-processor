using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Serialization;

namespace Radoslav.Common.UnitTests
{
    [TestClass]
    public sealed class BinaryXmlSerializerUnitTests
    {
        public TestContext TestContext { get; set; }

        private IEntityBinarySerializer Serializer
        {
            get
            {
                return (IEntityBinarySerializer)this.TestContext.Properties["Serializer"];
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            this.TestContext.Properties.Add("Serializer", new EntityBinaryXmlSerializer());
        }

        [TestMethod]
        public void CanDetermineEntityTypeFromContent()
        {
            Assert.IsFalse(this.Serializer.CanDetermineEntityTypeFromContent);
        }

        [TestMethod]
        public void SerializeAndDeserializeNull()
        {
            byte[] content = this.Serializer.Serialize(null);

            Assert.AreEqual(0, content.Length);

            Assert.IsNull(this.Serializer.Deserialize(null, typeof(FakeEntity)));
            Assert.IsNull(this.Serializer.Deserialize(new byte[0], typeof(FakeEntity)));
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void SerializeAndDeserializeWithoutType()
        {
            this.Serializer.Deserialize(new byte[0]);
        }

        [TestMethod]
        public void SerializeAndDeserializeWithType()
        {
            FakeEntity entity1 = new FakeEntity()
            {
                StringValue = "A",
                IntValue = 19
            };

            byte[] content = this.Serializer.Serialize(entity1);

            FakeEntity entity2 = (FakeEntity)this.Serializer.Deserialize(content, entity1.GetType());

            UnitTestHelpers.AssertEqualByPublicScalarProperties(entity1, entity2);
        }
    }
}