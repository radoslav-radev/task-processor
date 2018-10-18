using System;
using System.Diagnostics;
using Radoslav.Redis;
using Radoslav.Serialization;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.Repository.Redis
{
    /// <summary>
    /// An implementation of <see cref="ITaskRepository"/> that uses Redis for storage.
    /// </summary>
    public class RedisTaskRepository : ITaskRepository
    {
        private const string TasksHashKey = "Tasks";

        private readonly IRedisProvider provider;
        private readonly IEntityBinarySerializer serializer;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisTaskRepository"/> class.
        /// </summary>
        /// <param name="provider">The Redis provider to use.</param>
        /// <param name="serializer">The serializer to use.</param>
        public RedisTaskRepository(IRedisProvider provider, IEntityBinarySerializer serializer)
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

        #region Properties

        /// <summary>
        /// Gets the Redis provider used by the repository.
        /// </summary>
        /// <value>The Redis provider used by the repository.</value>
        public IRedisProvider Provider
        {
            get { return this.provider; }
        }

        /// <summary>
        /// Gets the Redis serializer used by the repository.
        /// </summary>
        /// <value>The Redis serializer used by the repository.</value>
        public IEntityBinarySerializer Serializer
        {
            get { return this.serializer; }
        }

        #endregion Properties

        #region ITaskRepository Members

        /// <inheritdoc />
        public void Add(Guid taskId, ITask task)
        {
            Trace.WriteLine("ENTER: Adding task '{0}' with ID '{1}' ...".FormatInvariant(task, taskId));

            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            string taskIdAsString = RedisConverter.ToString(taskId);

            if (this.serializer.CanDetermineEntityTypeFromContent)
            {
                this.provider.SetHashValue(RedisTaskRepository.TasksHashKey, taskIdAsString, this.serializer.Serialize(task));
            }
            else
            {
                using (IRedisTransaction transaction = this.provider.CreateTransaction())
                {
                    transaction.SetHashValue(RedisTaskRepository.TasksHashKey, taskIdAsString, this.serializer.Serialize(task));
                    transaction.SetHashValue(RedisTaskRepository.TasksHashKey, taskIdAsString + "$Type", RedisConverter.ToString(task.GetType(), true));

                    transaction.Commit();
                }
            }

            Trace.WriteLine("EXIT: Task '{0}' with ID '{1}' added.".FormatInvariant(task, taskId));
        }

        /// <inheritdoc />
        public ITask GetById(Guid taskId)
        {
            Trace.WriteLine("ENTER: Getting task with ID '{0}' ...".FormatInvariant(taskId));

            string taskIdAsString = RedisConverter.ToString(taskId);

            ITask result;

            if (this.serializer.CanDetermineEntityTypeFromContent)
            {
                byte[] content = this.provider.GetHashBinaryValue(RedisTaskRepository.TasksHashKey, taskIdAsString);

                if ((content == null) || (content.Length == 0))
                {
                    return null;
                }

                result = (ITask)this.serializer.Deserialize(content);
            }
            else
            {
                byte[] content = null;
                string taskTypeAsString = null;

                using (IRedisPipeline pipeline = this.provider.CreatePipeline())
                {
                    pipeline.GetHashBinaryValue(RedisTaskRepository.TasksHashKey, taskIdAsString, value => content = value);
                    pipeline.GetHashTextValue(RedisTaskRepository.TasksHashKey, taskIdAsString + "$Type", value => taskTypeAsString = value);

                    pipeline.Flush();
                }

                if ((content == null) || (content.Length == 0))
                {
                    return null;
                }

                if (string.IsNullOrEmpty(taskTypeAsString))
                {
#if DEBUG
                    throw new TypeNotFoundInRedisException("Task type for task '{0}' was not found in Redis.".FormatInvariant(taskId));
#else
                    Trace.TraceWarning("EXIT: Task type for task '{0}' was not found in Redis.".FormatInvariant(taskId));

                    return null;
#endif
                }

                Type taskType;

#if DEBUG
                taskType = Type.GetType(taskTypeAsString, true);
#else
                taskType = Type.GetType(taskTypeAsString, false);

                if (taskType == null)
                {
                    Trace.TraceWarning("EXIT: Task type '{0}' cannot be resolved.", taskTypeAsString);

                    return null;
                }
#endif

                result = (ITask)this.serializer.Deserialize(content, taskType);
            }

            Trace.WriteLine("EXIT: Task '{0}' with ID '{1}' returned.".FormatInvariant(result, taskId));

            return result;
        }

        /// <inheritdoc />
        public void Delete(Guid taskId)
        {
            Trace.WriteLine("ENTER: Deleting task '{0}' ...".FormatInvariant(taskId));

            string taskIdAsString = RedisConverter.ToString(taskId);

            this.provider.RemoveFromHash(RedisTaskRepository.TasksHashKey, taskIdAsString, taskIdAsString + "$Type");

            Trace.WriteLine("EXIT: Task '{0}' deleted.".FormatInvariant(taskId));
        }

        #endregion ITaskRepository Members
    }
}