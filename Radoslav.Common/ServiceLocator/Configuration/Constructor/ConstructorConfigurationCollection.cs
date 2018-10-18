using System;
using System.Configuration;
using System.Reflection;
using System.Xml;

namespace Radoslav.ServiceLocator.Configuration
{
    [ConfigurationCollection(typeof(ConstructorConfigurationElementBase))]
    internal sealed class ConstructorConfigurationCollection : ConfigurationElementCollection
    {
        internal ConstructorInfo ResolvedConstructorInfo { get; set; }

        internal new ConstructorConfigurationElementBase this[string parameterName]
        {
            get
            {
                return (ConstructorConfigurationElementBase)this.BaseGet(parameterName);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            throw new InvalidOperationException();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ConstructorConfigurationElementBase)element).ParameterName;
        }

        protected override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
        {
            ConstructorConfigurationElementBase element;

            switch (elementName)
            {
                case "parameter":
                    element = new ConstructorValueConfigurationElement();
                    break;

                case "dependency":
                    element = new ConstructorResolveConfigurationElement();
                    break;

                default:
                    return false;
            }

            element.DeserializeElement(reader);

            this.BaseAdd(element);

            return true;
        }
    }
}