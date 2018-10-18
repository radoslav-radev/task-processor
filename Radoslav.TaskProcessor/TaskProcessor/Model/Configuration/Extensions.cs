using System;
using System.Linq;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.Configuration
{
    /// <summary>
    /// Class for configuration extensions and helpers.
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Checks if tasks of a specified type are defined in configuration.
        /// </summary>
        /// <typeparam name="TTask">The type of the tasks.</typeparam>
        /// <param name="configuration">The client configuration.</param>
        /// <returns>True if tasks of the specified tasks are defined in configuration; otherwise false.</returns>
        public static bool IsSupported<TTask>(this ITaskProcessorClientConfiguration configuration)
            where TTask : ITask
        {
            try
            {
                configuration.GetPollingQueueKey(typeof(TTask));
            }
            catch (ArgumentException)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Merges a <see cref="ITaskProcessorConfiguration"/> instance with another.
        /// </summary>
        /// <param name="source">The <see cref="ITaskProcessorConfiguration"/> instance to update.</param>
        /// <param name="other">The <see cref="ITaskProcessorConfiguration"/> to merge with.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="source"/> or <paramref name="other"/> is null.</exception>
        public static void MergeWith(this ITaskProcessorConfiguration source, ITaskProcessorConfiguration other)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            source.Tasks.MergeWith(other.Tasks);
            source.PollingJobs.MergeWith(other.PollingJobs);
            source.PollingQueues.MergeWith(other.PollingQueues);
        }

        private static void MergeWith(this ITaskJobsConfiguration source, ITaskJobsConfiguration other)
        {
            foreach (ITaskJobConfiguration jobConfig in source.ToArray())
            {
                if (other[jobConfig.TaskType] == null)
                {
                    source.Remove(jobConfig.TaskType);
                }
            }

            foreach (ITaskJobConfiguration otherTask in other)
            {
                if (source[otherTask.TaskType] == null)
                {
                    source.AddCopy(otherTask);
                }
            }
        }

        private static void MergeWith(this IPollingJobsConfiguration source, IPollingJobsConfiguration other)
        {
            foreach (IPollingJobConfiguration queueConfig in source.ToArray())
            {
                if (other[queueConfig.ImplementationType] == null)
                {
                    source.Remove(queueConfig.ImplementationType);
                }
            }

            foreach (IPollingJobConfiguration queueConfig in other)
            {
                if (source[queueConfig.ImplementationType] == null)
                {
                    source.AddCopy(queueConfig);
                }
            }
        }

        private static void MergeWith(this ITaskProcessorPollingQueuesConfiguration source, ITaskProcessorPollingQueuesConfiguration other)
        {
            foreach (ITaskProcessorPollingQueueConfiguration queueConfig in source.ToArray())
            {
                if (other[queueConfig.Key] == null)
                {
                    source.Remove(queueConfig.Key);
                }
            }

            foreach (ITaskProcessorPollingQueueConfiguration queueConfig in other)
            {
                if (source[queueConfig.Key] == null)
                {
                    source.AddCopy(queueConfig);
                }
            }
        }
    }
}