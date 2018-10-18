using System;
using System.ComponentModel;
using System.Configuration;
using System.Xml;

namespace Radoslav.ServiceLocator.Configuration
{
    internal sealed class RemoveServiceConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("key")]
        internal string Key
        {
            get { return (string)base["key"]; }
        }

        [ConfigurationProperty("contract", IsRequired = true)]
        [TypeConverter(typeof(TypeNameConverter))]
        internal Type ContractType
        {
            get { return (Type)base["contract"]; }
        }

        internal void DeserializeElement(XmlReader reader)
        {
            this.DeserializeElement(reader, false);
        }
    }
}