using System;
using System.Diagnostics;
using Radoslav.Redis;
using Radoslav.Serialization;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.Repository.Redis
{
    /// <summary>
    /// An implementation of the <see cref="ITaskSummaryRepository"/> interface that uses Redis for storage.
    /// </summary>
    public sealed class RedisTaskSummaryRepository : ITaskSummaryRepository
    {
        private const string RedisTaskSummaryHashKey = "TaskSummary";

        private readonly IRedisProvider provider;
        private readonly IEntityBinarySerializer serializer;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisTaskSummaryRepository"/> class.
        /// </summary>
        /// <param name="provider">The Redis provider to use.</param>
        /// <param name="serializer">The serializer to use.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="provider"/> or <paramref name="serializer"/> is null.</exception>
        public RedisTaskSummaryRepository(IRedisProvider provider, IEntityBinarySerializer serializer)
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

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the Redis provider used by the repository.
        /// </summary>
        /// <value>The Redis provider used by the repository.</value>
        public IRedisProvider Provider
        {
            get { return this.provider; }
        }

        #endregion Properties

        #region ITaskSummaryRepository Members

        /// <inheritdoc />
        public void Add(Guid taskId, ITaskSummary summary)
        {
            Trace.WriteLine("ENTER: Adding summary '{0}' for task '{1}' ...".FormatInvariant(summary, taskId));

            if (summary == null)
            {
                throw new ArgumentNullException(nameof(summary));
            }

            string taskIdAsString = RedisConverter.ToString(taskId);

            byte[] content = this.serializer.Serialize(summary);

            if (this.serializer.CanDetermineEntityTypeFromContent)
            {
                this.provider.SetHashValue(RedisTaskSummaryRepository.RedisTaskSummaryHashKey, taskIdAsString, content);
            }
            else
            {
                using (IRedisTransaction transaction = this.provider.CreateTransaction())
                {
                    transaction.SetHashValue(RedisTaskSummaryRepository.RedisTaskSummaryHashKey, taskIdAsString, content);
                    transaction.SetHashValue(RedisTaskSummaryRepository.RedisTaskSummaryHashKey, taskIdAsString + "$Type", RedisConverter.ToString(summary.GetType(), false));

                    transaction.Commit();
                }
            }

            Trace.WriteLine("EXIT: Summary '{0}' added for task '{1}'.".FormatInvariant(summary, taskId));
        }

        /// <inheritdoc />
        public ITaskSummary GetById(Guid taskId)
        {
            Trace.WriteLine("ENTER: Getting summary for task '{0}' ...".FormatInvariant(taskId));

            string taskIdAsString = RedisConverter.ToString(taskId);

            byte[] content = null;

            ITaskSummary result;

            if (this.serializer.CanDetermineEntityTypeFromContent)
            {
                content = this.provider.GetHashBinaryValue(RedisTaskSummaryRepository.RedisTaskSummaryHashKey, taskIdAsString);

                result = (ITaskSummary)this.serializer.Deserialize(content);
            }
            else
            {
                string summaryTypeAsString = null;

                using (IRedisPipeline pipeline = this.provider.CreatePipeline())
                {
                    pipeline.GetHashBinaryValue(RedisTaskSummaryRepository.RedisTaskSummaryHashKey, taskIdAsString, value => content = value);
                    pipeline.GetHashTextValue(RedisTaskSummaryRepository.RedisTaskSummaryHashKey, taskIdAsString + "$Type", value => summaryTypeAsString = value);

                    pipeline.Flush();
                }

                if ((content == null) || (content.Length == 0))
                {
                    return null;
                }

                if (string.IsNullOrEmpty(summaryTypeAsString))
                {
#if DEBUG
                    throw new TypeNotFoundInRedisException("Task summary type for task '{0}' was not found in Redis.".FormatInvariant(taskId));
#else
                    Trace.TraceWarning("EXIT: Task summary type for task '{0}' was not found in Redis.".FormatInvariant(taskId));

                    return null;
#endif
                }

                Type summaryType;

#if DEBUG
                summaryType = Type.GetType(summaryTypeAsString, true);
#else
                summaryType = Type.GetType(summaryTypeAsString, false);

                if (summaryType == null)
                {
                    Trace.TraceWarning("EXIT: Task summary type '{0}' cannot be resolved.", summaryTypeAsString);

                    return null;
                }
#endif

                result = (ITaskSummary)this.serializer.Deserialize(content, summaryType);
            }

            Trace.WriteLine("EXIT: Return task summary '{0}' for task '{1}'.".FormatInvariant(result, taskId));

            return result;
        }

        #endregion ITaskSummaryRepository Members
    }
}