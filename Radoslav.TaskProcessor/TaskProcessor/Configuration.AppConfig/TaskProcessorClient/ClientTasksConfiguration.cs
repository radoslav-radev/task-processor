using System.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace Radoslav.TaskProcessor.Configuration.AppConfig
{
    /// <summary>
    /// A collection of client task configurations in the App.config file.
    /// </summary>
    [ConfigurationCollection(typeof(ClientTaskConfiguration))]
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = SuppressMessages.ConfigurationProperty)]
    internal sealed class ClientTasksConfiguration : ConfigurationElementCollection
    {
        /// <inheritdoc />
        protected override ConfigurationElement CreateNewElement()
        {
            return new ClientTaskConfiguration();
        }

        /// <inheritdoc />
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ClientTaskConfiguration)element).TaskType;
        }
    }
}