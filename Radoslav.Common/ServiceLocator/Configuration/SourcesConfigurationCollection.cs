using System.Configuration;

namespace Radoslav.ServiceLocator.Configuration
{
    [ConfigurationCollection(typeof(SourceConfigurationElement))]
    internal sealed class SourcesConfigurationCollection : ConfigurationElementCollection
    {
        /// <inheritdoc />
        protected override ConfigurationElement CreateNewElement()
        {
            return new SourceConfigurationElement();
        }

        /// <inheritdoc />
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SourceConfigurationElement)element).ConfigFilePath;
        }
    }
}