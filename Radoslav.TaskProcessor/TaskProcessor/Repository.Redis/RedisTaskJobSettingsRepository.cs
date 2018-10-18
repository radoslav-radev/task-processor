using System;
using System.Diagnostics;
using Radoslav.Redis;
using Radoslav.Serialization;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.Repository.Redis
{
    /// <summary>
    /// An implementation of the <see cref="ITaskJobSettingsRepository"/> that uses Redis for storage.
    /// </summary>
    public sealed class RedisTaskJobSettingsRepository : ITaskJobSettingsRepository
    {
        private const string RedisTaskJobSettingsHashKey = "TaskJobSettings";

        private readonly IRedisProvider provider;
        private readonly IEntityBinarySerializer serializer;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisTaskJobSettingsRepository"/> class.
        /// </summary>
        /// <param name="provider">The Redis provider to use.</param>
        /// <param name="serializer">The serializer to use.</param>
        public RedisTaskJobSettingsRepository(IRedisProvider provider, IEntityBinarySerializer serializer)
        {
            Trace.WriteLine("ENTER: Constructing '{0}' ...".FormatInvariant(this.GetType().Name));

            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            if (serializer == null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            this.provider = provider;
            this.serializer = serializer;

            Trace.WriteLine("EXIT: '{0}' constructed.".FormatInvariant(this.GetType().Name));
        }

        #endregion Constructor

        #region ITaskJobSettingsRepository Members

        /// <inheritdoc />
        public ITaskJobSettings Get(Type taskType)
        {
            Trace.WriteLine("ENTER: Getting task job settings for task '{0}' ...".FormatInvariant(taskType));

            if (taskType == null)
            {
                throw new ArgumentNullException(nameof(taskType));
            }

            if (!typeof(ITask).IsAssignableFrom(taskType))
            {
                throw new ArgumentException("Type '{0}' does not implement '{1}'.".FormatInvariant(taskType, typeof(ITask), nameof(taskType)));
            }

            ITaskJobSettings result;

            if (this.serializer.CanDetermineEntityTypeFromContent)
            {
                byte[] content = this.provider.GetHashBinaryValue(RedisTaskJobSettingsRepository.RedisTaskJobSettingsHashKey, taskType.Name);

                result = (ITaskJobSettings)this.serializer.Deserialize(content);
            }
            else
            {
                byte[] content = null;

                string settingsTypeAsString = null;

                using (IRedisPipeline pipeline = this.provider.CreatePipeline())
                {
                    pipeline.GetHashBinaryValue(RedisTaskJobSettingsRepository.RedisTaskJobSettingsHashKey, taskType.Name, value => content = value);
                    pipeline.GetHashTextValue(RedisTaskJobSettingsRepository.RedisTaskJobSettingsHashKey, taskType.Name + "$Type", value => settingsTypeAsString = value);

                    pipeline.Flush();
                }

                if ((content == null) || (content.Length == 0))
                {
                    return null;
                }

                if (string.IsNullOrEmpty(settingsTypeAsString))
                {
#if DEBUG
                    throw new TypeNotFoundInRedisException("Task job settings type for task '{0}' was not found in Redis.".FormatInvariant(taskType));
#else
                    Trace.TraceWarning("EXIT: Task job settings type for task '{0}' was not found in Redis.".FormatInvariant(taskType));

                    return null;
#endif
                }

                Type settingsType;

#if DEBUG
                settingsType = Type.GetType(settingsTypeAsString, true);
#else
                settingsType = Type.GetType(settingsTypeAsString, false);

                if (settingsType == null)
                {
                    Trace.TraceWarning("EXIT: Task job settings type '{0}' cannot be resolved.", settingsTypeAsString);

                    return null;
                }
#endif

                result = (ITaskJobSettings)this.serializer.Deserialize(content, settingsType);
            }

            Trace.WriteLine("EXIT: Return task job settings '{0}' for task '{1}'.".FormatInvariant(result, taskType));

            return result;
        }

        /// <inheritdoc />
        public void Set(Type taskType, ITaskJobSettings settings)
        {
            Trace.WriteLine("ENTER: Setting task job settings '{0}' for task '{1}' ...".FormatInvariant(settings, taskType));

            if (taskType == null)
            {
                throw new ArgumentNullException(nameof(taskType));
            }

            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (!typeof(ITask).IsAssignableFrom(taskType))
            {
                throw new ArgumentException("Type '{0}' does not implement '{1}'.".FormatInvariant(taskType, typeof(ITask), nameof(taskType)));
            }

            byte[] content = this.serializer.Serialize(settings);

            if (this.serializer.CanDetermineEntityTypeFromContent)
            {
                this.provider.SetHashValue(RedisTaskJobSettingsRepository.RedisTaskJobSettingsHashKey, taskType.Name, content);
            }
            else
            {
                using (IRedisPipeline pipeline = this.provider.CreatePipeline())
                {
                    pipeline.SetHashValue(RedisTaskJobSettingsRepository.RedisTaskJobSettingsHashKey, taskType.Name, content);
                    pipeline.SetHashValue(RedisTaskJobSettingsRepository.RedisTaskJobSettingsHashKey, taskType.Name + "$Type", RedisConverter.ToString(settings.GetType(), false));

                    pipeline.Flush();
                }
            }

            Trace.WriteLine("EXIT: Task job settings '{0}' for task '{1}' set.".FormatInvariant(settings, taskType));
        }

        /// <inheritdoc />
        public void Clear(Type taskType)
        {
            Trace.WriteLine("ENTER: Clearing task job settings for task '{0}' ...".FormatInvariant(taskType));

            if (taskType == null)
            {
                throw new ArgumentNullException(nameof(taskType));
            }

            if (!typeof(ITask).IsAssignableFrom(taskType))
            {
                throw new ArgumentException("Type '{0}' does not implement '{1}'.".FormatInvariant(taskType, typeof(ITask), nameof(taskType)));
            }

            if (this.serializer.CanDetermineEntityTypeFromContent)
            {
                this.provider.RemoveFromHash(RedisTaskJobSettingsRepository.RedisTaskJobSettingsHashKey, taskType.Name);
            }
            else
            {
                this.provider.RemoveFromHash(RedisTaskJobSettingsRepository.RedisTaskJobSettingsHashKey, taskType.Name, taskType.Name + "$Type");
            }

            Trace.WriteLine("EXIT: Task job settings for task '{0}' cleared.".FormatInvariant(taskType));
        }

        #endregion ITaskJobSettingsRepository Members
    }
}