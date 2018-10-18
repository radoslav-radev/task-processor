using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Serialization;

namespace Radoslav.Common.UnitTests
{
    [TestClass]
    public sealed class BinarySerializerUnitTests
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
            this.TestContext.Properties.Add("Serializer", new EntityBinarySerializer());
        }

        [TestMethod]
        public void CanDetermineEntityTypeFromContent()
        {
            Assert.IsTrue(this.Serializer.CanDetermineEntityTypeFromContent);
        }

        [TestMethod]
        public void SerializeAndDeserializeNull()
        {
            byte[] content = this.Serializer.Serialize(null);

            Assert.AreEqual(0, content.Length);

            Assert.IsNull(this.Serializer.Deserialize(null));
            Assert.IsNull(this.Serializer.Deserialize(new byte[0]));
        }

        [TestMethod]
        public void SerializeAndDeserializeWithoutType()
        {
            this.SerializeAndDeserialize(content => this.Serializer.Deserialize(content));
        }

        [TestMethod]
        public void SerializeAndDeserializeWithType()
        {
            this.SerializeAndDeserialize(content => this.Serializer.Deserialize(content, typeof(FakeEntity)));
        }

        private void SerializeAndDeserialize(Func<byte[], object> deserializeCallback)
        {
            FakeEntity entity1 = new FakeEntity()
            {
                StringValue = "A",
                IntValue = 19
            };

            byte[] content = this.Serializer.Serialize(entity1);

            FakeEntity entity2 = (FakeEntity)this.Serializer.Deserialize(content);

            UnitTestHelpers.AssertEqualByPublicScalarProperties(entity1, entity2);
        }
    }
}