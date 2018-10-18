using System.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace Radoslav.TaskProcessor.Configuration.AppConfig
{
    /// <summary>
    /// A collection of client polling queues configurations that reads from the App.config file.
    /// </summary>
    [ConfigurationCollection(typeof(ClientPollingQueueConfiguration))]
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = SuppressMessages.ConfigurationProperty)]
    internal sealed class ClientPollingQueuesConfiguration : ConfigurationElementCollection
    {
        /// <inheritdoc />
        protected override ConfigurationElement CreateNewElement()
        {
            return new ClientPollingQueueConfiguration();
        }

        /// <inheritdoc />
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ClientPollingQueueConfiguration)element).Key;
        }
    }
}