using System.Configuration;

namespace Radoslav.ServiceLocator.Configuration
{
    [ConfigurationCollection(typeof(CollectionPropertyValueConfigurationElement))]
    internal sealed class CollectionPropertyValuesConfigurationCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new CollectionPropertyValueConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CollectionPropertyValueConfigurationElement)element).ValueAsString;
        }
    }
}