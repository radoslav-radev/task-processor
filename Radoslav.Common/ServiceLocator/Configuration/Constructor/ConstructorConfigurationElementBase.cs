using System.Configuration;
using System.Xml;

namespace Radoslav.ServiceLocator.Configuration
{
    internal abstract class ConstructorConfigurationElementBase : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        internal string ParameterName
        {
            get { return (string)base["name"]; }
        }

        internal void DeserializeElement(XmlReader reader)
        {
            base.DeserializeElement(reader, false);
        }
    }
}