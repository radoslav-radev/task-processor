using System;
using System.Configuration;
using System.Diagnostics;

namespace Radoslav.Configuration
{
    /// <summary>
    /// Class for helper methods for System.Configuration namespace.
    /// </summary>
    public static class ConfigurationHelpers
    {
        /// <summary>
        /// Loads a configuration section from App.config file.
        /// </summary>
        /// <typeparam name="TConfigurationSection">Type configuration section type.</typeparam>
        /// <param name="appSectionName">The section name in the App.config configuration sections.</param>
        /// <returns>The configuration section retrieved from the App.config file.</returns>
        public static TConfigurationSection Load<TConfigurationSection>(string appSectionName)
        {
            Trace.WriteLine("ENTER: Loading {0} ...".FormatInvariant(typeof(TConfigurationSection).Name));

            if (string.IsNullOrWhiteSpace(appSectionName))
            {
                throw new ArgumentNullException("appSectionName");
            }

            TConfigurationSection result = (TConfigurationSection)ConfigurationManager.GetSection(appSectionName);

            Trace.WriteLine("EXIT: {0} loaded.".FormatInvariant(typeof(TConfigurationSection).Name));

            return result;
        }

        /// <summary>
        /// Loads a configuration section from a specified configuration file.
        /// </summary>
        /// <typeparam name="TConfigurationSection">Type configuration section type.</typeparam>
        /// <param name="appSectionName">The section name in the configuration file configuration sections.</param>
        /// <param name="configurationFilePath">The path to the configuration file.</param>
        /// <returns>The configuration section retrieved from the specified configuration file.</returns>
        public static TConfigurationSection Load<TConfigurationSection>(string appSectionName, string configurationFilePath)
            where TConfigurationSection : ConfigurationSection
        {
            Trace.WriteLine("ENTER: Loading {0} from {1} ...".FormatInvariant(typeof(TConfigurationSection).Name, configurationFilePath));

            if (string.IsNullOrWhiteSpace(appSectionName))
            {
                throw new ArgumentNullException("appSectionName");
            }

            if (string.IsNullOrWhiteSpace(configurationFilePath))
            {
                throw new ArgumentNullException("configurationFilePath");
            }

            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();

            fileMap.ExeConfigFilename = configurationFilePath;

            var configuraiton = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

            TConfigurationSection result = (TConfigurationSection)configuraiton.GetSection(appSectionName);

            Trace.WriteLine("EXIT: {0} loaded from {1}.".FormatInvariant(typeof(TConfigurationSection).Name, configurationFilePath));

            return result;
        }
    }
}