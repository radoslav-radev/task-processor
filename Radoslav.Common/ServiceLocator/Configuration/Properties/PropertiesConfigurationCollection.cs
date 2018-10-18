using System;
using System.Configuration;
using System.Xml;

namespace Radoslav.ServiceLocator.Configuration
{
    [ConfigurationCollection(typeof(PropertyConfigurationElementBase))]
    internal sealed class PropertiesConfigurationCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            throw new NotSupportedException();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PropertyConfigurationElementBase)element).PropertyName;
        }

        protected override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
        {
            PropertyConfigurationElementBase element;

            switch (elementName)
            {
                case "set":
                    element = new ValuePropertyConfigurationElement();

                    break;

                case "dependency":
                    element = new ResolvePropertyConfigurationElement();

                    break;

                case "collection":
                    element = new CollectionPropertyConfigurationElement();

                    break;

                default:
                    return false;
            }

            element.DeserializeElementInternal(reader);

            this.BaseAdd(element);

            return true;
        }
    }
}