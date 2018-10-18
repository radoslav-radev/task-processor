using System;
using System.Collections.Generic;
using System.Linq;
using Radoslav.Redis;
using Radoslav.TaskProcessor.Configuration;

namespace Radoslav.TaskProcessor.Repository.Redis
{
    /// <summary>
    /// Class for serialization and deserialization of configuration classes to and from Redis.
    /// </summary>
    internal static class RedisTaskProcessorConfigurationRepository
    {
        /// <summary>
        /// Serializes a task processor configuration to a dictionary to be stored in Redis.
        /// </summary>
        /// <param name="configuration">The configuration to serialize.</param>
        /// <param name="result">The dictionary where to add the serialized key/value pairs.</param>
        /// <param name="prefix">The prefix to add before each dictionary key.</param>
        internal static void Serialize(ITaskProcessorConfiguration configuration, IDictionary<string, string> result, string prefix)
        {
            if (!string.IsNullOrEmpty(prefix) && !prefix.EndsWith(".", StringComparison.Ordinal))
            {
                prefix = prefix + ".";
            }

            RedisTaskProcessorConfigurationRepository.Serialize(configuration.Tasks, result, prefix + "Tasks");
            RedisTaskProcessorConfigurationRepository.Serialize(configuration.PollingQueues, result, prefix + "PollingQueues");
            RedisTaskProcessorConfigurationRepository.Serialize(configuration.PollingJobs, result, prefix + "PollingJobs");
        }

        /// <summary>
        /// Deserializes a task processor configuration from a Redis dictionary.
        /// </summary>
        /// <param name="values">The dictionary stored in Redis.</param>
        /// <param name="result">The instance where to deserialize the data.</param>
        /// <param name="prefix">The prefix that has been added before each dictionary key during serialization.</param>
        internal static void Deserialize(IReadOnlyDictionary<string, string> values, ITaskProcessorConfiguration result, string prefix)
        {
            if (!string.IsNullOrEmpty(prefix) && !prefix.EndsWith(".", StringComparison.Ordinal))
            {
                prefix = prefix + ".";
            }

            RedisTaskProcessorConfigurationRepository.Deserialize(values, result.Tasks, prefix + "Tasks");
            RedisTaskProcessorConfigurationRepository.Deserialize(values, result.PollingQueues, prefix + "PollingQueues");
            RedisTaskProcessorConfigurationRepository.Deserialize(values, result.PollingJobs, prefix + "PollingJobs");
        }

        private static void Serialize(ITaskJobsConfiguration configuration, IDictionary<string, string> result, string prefix)
        {
            if (!string.IsNullOrEmpty(prefix) && !prefix.EndsWith(".", StringComparison.Ordinal))
            {
                prefix = prefix + ".";
            }

            string key = string.Concat(prefix, "MaxTasks");

            result.Add(key, RedisConverter.ToString(configuration.MaxWorkers));

            foreach (ITaskJobConfiguration jobConfig in configuration)
            {
                key = string.Concat(prefix, jobConfig.TaskType.Name, ".TaskType");

                result.Add(key, RedisConverter.ToString(jobConfig.TaskType, false));

                key = string.Concat(prefix, jobConfig.TaskType.Name, ".MaxWorkers");

                result.Add(key, RedisConverter.ToString(jobConfig.MaxWorkers));
            }
        }

        private static void Deserialize(IReadOnlyDictionary<string, string> values, ITaskJobsConfiguration result, string prefix)
        {
            if (!string.IsNullOrEmpty(prefix) && !prefix.EndsWith(".", StringComparison.Ordinal))
            {
                prefix = prefix + ".";
            }

            result.MaxWorkers = RedisConverter.ParseIntegerOrNull(values[prefix + "MaxTasks"]);

            foreach (KeyValuePair<string, string> pair in values.Where(v => v.Key.StartsWith(prefix, StringComparison.Ordinal) && v.Key.EndsWith(".TaskType", StringComparison.Ordinal)))
            {
                Type taskType = RedisConverter.ParseType(pair.Value);

                ITaskJobConfiguration jobConfig = result.Add(taskType);

                string key = string.Concat(prefix, taskType.Name, ".MaxWorkers");

                jobConfig.MaxWorkers = RedisConverter.ParseIntegerOrNull(values[key]);
            }
        }

        private static void Serialize(IPollingConfiguration queueConfig, IDictionary<string, string> result, string prefix)
        {
            result.Add(
                string.Concat(prefix, ".Interval"),
                RedisConverter.ToString(queueConfig.PollInterval));

            result.Add(
                string.Concat(prefix, ".Master"),
                RedisConverter.ToString(queueConfig.IsMaster));

            result.Add(
               string.Concat(prefix, ".Active"),
               RedisConverter.ToString(queueConfig.IsActive));

            result.Add(
              string.Concat(prefix, ".Concurrent"),
              RedisConverter.ToString(queueConfig.IsConcurrent));
        }

        private static void Serialize(IPollingJobsConfiguration configuration, IDictionary<string, string> result, string prefix)
        {
            if (!string.IsNullOrEmpty(prefix) && !prefix.EndsWith(".", StringComparison.Ordinal))
            {
                prefix = prefix + ".";
            }

            foreach (IPollingJobConfiguration config in configuration)
            {
                result.Add(
                    string.Concat(prefix, config.ImplementationType.Name, ".Type"),
                    RedisConverter.ToString(config.ImplementationType, false));

                RedisTaskProcessorConfigurationRepository.Serialize(config, result, prefix + config.ImplementationType.Name);
            }
        }

        private static void Deserialize(IReadOnlyDictionary<string, string> values, IPollingConfiguration result, string prefix)
        {
            string key = string.Concat(prefix, ".Interval");

            result.PollInterval = RedisConverter.ParseTimeSpan(values[key]);

            key = string.Concat(prefix, ".Master");

            result.IsMaster = RedisConverter.ParseBoolean(values[key]);

            key = string.Concat(prefix, ".Active");

            result.IsActive = RedisConverter.ParseBoolean(values[key]);

            key = string.Concat(prefix, ".Concurrent");

            result.IsConcurrent = RedisConverter.ParseBoolean(values[key]);
        }

        private static void Deserialize(IReadOnlyDictionary<string, string> values, IPollingJobsConfiguration result, string prefix)
        {
            if (!string.IsNullOrEmpty(prefix) && !prefix.EndsWith(".", StringComparison.Ordinal))
            {
                prefix = prefix + ".";
            }

            foreach (KeyValuePair<string, string> pair in values.Where(v => v.Key.StartsWith(prefix, StringComparison.Ordinal) && v.Key.EndsWith(".Type", StringComparison.Ordinal)))
            {
                Type implementationType = RedisConverter.ParseType(pair.Value);

                IPollingJobConfiguration config = result.Add(implementationType);

                RedisTaskProcessorConfigurationRepository.Deserialize(values, config, prefix + implementationType.Name);
            }
        }

        private static void Serialize(ITaskProcessorPollingQueuesConfiguration configuration, IDictionary<string, string> result, string prefix)
        {
            if (!string.IsNullOrEmpty(prefix) && !prefix.EndsWith(".", StringComparison.Ordinal))
            {
                prefix = prefix + ".";
            }

            foreach (ITaskProcessorPollingQueueConfiguration config in configuration)
            {
                result.Add(
                    string.Concat(prefix, config.Key, ".Type"),
                    RedisConverter.ToString(config.Key));

                result.Add(
                    string.Concat(prefix, config.Key, ".MaxWorkers"),
                    RedisConverter.ToString(config.MaxWorkers));

                RedisTaskProcessorConfigurationRepository.Serialize(config, result, prefix + config.Key);
            }
        }

        private static void Deserialize(IReadOnlyDictionary<string, string> values, ITaskProcessorPollingQueuesConfiguration result, string prefix)
        {
            if (!string.IsNullOrEmpty(prefix) && !prefix.EndsWith(".", StringComparison.Ordinal))
            {
                prefix = prefix + ".";
            }

            foreach (KeyValuePair<string, string> pair in values.Where(v => v.Key.StartsWith(prefix, StringComparison.Ordinal) && v.Key.EndsWith(".Type", StringComparison.Ordinal)))
            {
                string key = pair.Value;

                ITaskProcessorPollingQueueConfiguration config = result.Add(key);

                config.MaxWorkers = RedisConverter.ParseInteger(values[prefix + key + ".MaxWorkers"]);

                RedisTaskProcessorConfigurationRepository.Deserialize(values, config, prefix + key);
            }
        }
    }
}