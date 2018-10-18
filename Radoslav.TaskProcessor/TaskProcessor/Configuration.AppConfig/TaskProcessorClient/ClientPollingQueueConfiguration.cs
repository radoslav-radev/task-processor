using System.Configuration;

namespace Radoslav.TaskProcessor.Configuration.AppConfig
{
    /// <summary>
    /// A client configuration for a single polling queue.
    /// </summary>
    internal sealed class ClientPollingQueueConfiguration : ConfigurationElement
    {
        /// <summary>
        /// Gets the unique key of the polling queue.
        /// </summary>
        /// <value>The unique key of the polling queue.</value>
        [ConfigurationProperty("key", IsRequired = true, IsKey = true)]
        internal string Key
        {
            get
            {
                return (string)this["key"];
            }
        }

        /// <summary>
        /// Gets a collection with the types of the tasks which belong to this polling queue.
        /// </summary>
        /// <value>The types of the tasks which belong to this polling queue.</value>
        [ConfigurationProperty("tasks", IsRequired = true)]
        internal ClientTasksConfiguration TaskTypes
        {
            get
            {
                return (ClientTasksConfiguration)this["tasks"];
            }
        }
    }
}