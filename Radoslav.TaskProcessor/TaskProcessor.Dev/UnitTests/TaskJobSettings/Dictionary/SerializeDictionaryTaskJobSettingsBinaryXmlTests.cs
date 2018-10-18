using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Serialization;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public sealed class SerializeDictionaryTaskJobSettingsBinaryXmlTests : SerializeDictionaryTaskJobSettingsTests<byte[]>
    {
        protected override IEntitySerializer<DictionaryTaskJobSettings, byte[]> CreateSerializer()
        {
            return new EntityBinaryXmlSerializer<DictionaryTaskJobSettings>();
        }
    }
}