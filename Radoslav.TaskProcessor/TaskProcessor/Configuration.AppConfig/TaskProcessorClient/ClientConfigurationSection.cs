using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.Configuration.AppConfig
{
    /// <summary>
    /// An implementation of <see cref="ITaskProcessorClientConfiguration"/> that reads from the App.config file.
    /// </summary>
    public sealed class ClientConfigurationSection : ConfigurationSection, ITaskProcessorClientConfiguration
    {
        /// <summary>
        /// The section name in App.config file.
        /// </summary>
        public const string SectionName = "Radoslav.TaskProcessor.Client";

        private readonly Dictionary<Type, string> taskTypePollingQueueKeys = new Dictionary<Type, string>();

        /// <summary>
        /// Gets a collection of task configurations.
        /// </summary>
        /// <value>A collection of task configurations.</value>
        [ConfigurationProperty("tasks")]
        internal ClientTasksConfiguration Tasks
        {
            get
            {
                return (ClientTasksConfiguration)this["tasks"];
            }
        }

        /// <summary>
        /// Gets a collection of polling queue configurations.
        /// </summary>
        /// <value>A collection of polling queue configurations.</value>
        [ConfigurationProperty("pollingQueues")]
        internal ClientPollingQueuesConfiguration PollingQueues
        {
            get
            {
                return (ClientPollingQueuesConfiguration)this["pollingQueues"];
            }
        }

        #region ITaskProcessorClientConfiguration Members

        /// <inheritdoc />
        public string GetPollingQueueKey(Type taskType)
        {
            if (taskType == null)
            {
                throw new ArgumentNullException("taskType");
            }

            if (!typeof(ITask).IsAssignableFrom(taskType))
            {
                throw new ArgumentException("Type '{0}' does not implement '{1}'.".FormatInvariant(taskType, typeof(ITask)), "taskType");
            }

            string result;

            if (!this.taskTypePollingQueueKeys.TryGetValue(taskType, out result))
            {
                throw new ArgumentException("Task type '{0}' is not defined in task processor client configuration.".FormatInvariant(taskType), "taskType");
            }

            return result;
        }

        #endregion ITaskProcessorClientConfiguration Members

        /// <inheritdoc />
        protected override void DeserializeSection(XmlReader reader)
        {
            base.DeserializeSection(reader);

            foreach (ClientTaskConfiguration taskConfig in this.Tasks)
            {
                this.taskTypePollingQueueKeys.Add(taskConfig.TaskType, null);
            }

            foreach (ClientPollingQueueConfiguration pollingQueueConfig in this.PollingQueues)
            {
                foreach (ClientTaskConfiguration taskConfig in pollingQueueConfig.TaskTypes)
                {
                    if (this.taskTypePollingQueueKeys.ContainsKey(taskConfig.TaskType))
                    {
                        throw new ConfigurationErrorsException(
                            "Task type '{0}' is defined more than once in task processor client configuration.".FormatInvariant(taskConfig.TaskType),
                            reader);
                    }

                    this.taskTypePollingQueueKeys.Add(taskConfig.TaskType, pollingQueueConfig.Key);
                }
            }
        }
    }
}