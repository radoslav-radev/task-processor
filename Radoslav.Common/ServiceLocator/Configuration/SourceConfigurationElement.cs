using System.Configuration;
using System.IO;
using Radoslav.Configuration;

namespace Radoslav.ServiceLocator.Configuration
{
    internal sealed class SourceConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("source", IsRequired = true, IsKey = true)]
        internal string ConfigFilePath
        {
            get
            {
                return (string)this["source"];
            }
        }

        internal ServiceLocatorConfiguration LoadConfigSource()
        {
            string configFilePath;

            if (Path.IsPathRooted(this.ConfigFilePath))
            {
                configFilePath = this.ConfigFilePath;
            }
            else
            {
                string configFileFolder = Path.GetDirectoryName(this.ElementInformation.Source);

                configFilePath = Path.Combine(configFileFolder, this.ConfigFilePath);
            }

            ServiceLocatorConfiguration result = ConfigurationHelpers.Load<ServiceLocatorConfiguration>(ServiceLocatorConfiguration.AppSectionName, configFilePath);

            if (result == null)
            {
                throw new ConfigurationErrorsException(
                    "Configuration source file '{0}' was not found.".FormatInvariant(this.ConfigFilePath),
                    this.ElementInformation.Source,
                    this.ElementInformation.LineNumber);
            }

            return result;
        }
    }
}