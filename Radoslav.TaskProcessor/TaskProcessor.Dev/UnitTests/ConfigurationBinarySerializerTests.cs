using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.ServiceLocator;
using Radoslav.TaskProcessor.Serialization;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public sealed class ConfigurationBinarySerializerTests
    {
        #region Properties & Initialize

        public TestContext TestContext { get; set; }

        private ConfigurationBinarySerializer Serializer
        {
            get
            {
                return (ConfigurationBinarySerializer)this.TestContext.Properties["Serializer"];
            }
        }

        private FakeSerializationConfiguration Configuration
        {
            get
            {
                return (FakeSerializationConfiguration)this.TestContext.Properties["Configuration"];
            }
        }

        private FakeServiceLocator ServiceLocator
        {
            get
            {
                return (FakeServiceLocator)this.Serializer.ServiceLocator;
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            this.TestContext.Properties.Add("Configuration", new FakeSerializationConfiguration());

            FakeConfigurationProvider configProvider = new FakeConfigurationProvider();

            configProvider.PredefineResult(this.Configuration, p => p.GetSerializationConfiguration());

            this.TestContext.Properties.Add("Serializer", new ConfigurationBinarySerializer(configProvider, new FakeServiceLocator()));
        }

        #endregion Properties & Initialize

        #region Constructors

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullConfigurationProvider()
        {
            new ConfigurationBinarySerializer(null, new FakeServiceLocator());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullServiceLocator()
        {
            new ConfigurationBinarySerializer(new FakeConfigurationProvider(), default(IRadoslavServiceLocator));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructorNullConfiguration()
        {
            new ConfigurationBinarySerializer(new FakeConfigurationProvider(), new FakeServiceLocator());
        }

        #endregion Constructors

        [TestMethod]
        public void CanDetermineEntityTypeFromContent()
        {
            Assert.IsFalse(this.Serializer.CanDetermineEntityTypeFromContent);
        }

        [TestMethod]
        public void SerializeAndDeserializeNull()
        {
            byte[] content = this.Serializer.Serialize(null);

            Assert.IsNull(this.Serializer.Deserialize(content));
            Assert.IsNull(this.Serializer.Deserialize(content, typeof(FakeTask)));
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void DeserializeWithoutEntityType()
        {
            this.Serializer.Deserialize(new byte[1]);
        }

        [TestMethod]
        public void CustomSerializerNoServiceLocator()
        {
            this.CustomSerializerInternal();
        }

        [TestMethod]
        public void CustomSerializerWithServiceLocator()
        {
            this.TestServiceLocator(this.CustomSerializerInternal);
        }

        private void CustomSerializerInternal()
        {
            this.Configuration.PredefineResult(typeof(FakeBinarySerializer), c => c.GetSerializerType(typeof(FakeTask)));

            byte[] content = this.Serializer.Serialize(new FakeTask());

            object task2 = this.Serializer.Deserialize(content, typeof(FakeTask));

            Assert.IsNotNull(task2);

            Assert.IsInstanceOfType(task2, typeof(FakeTask));
        }

        private void TestServiceLocator(Action callback)
        {
            this.ServiceLocator.PredefineResult(true, l => l.CanResolve(typeof(FakeBinarySerializer)));

            this.ServiceLocator.PredefineResult(new FakeBinarySerializer(), l => l.ResolveSingle(typeof(FakeBinarySerializer)));

            callback();

            this.ServiceLocator.AssertMethodCallOnceWithArguments(l => l.ResolveSingle(typeof(FakeBinarySerializer)));
        }
    }
}