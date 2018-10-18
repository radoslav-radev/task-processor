using Radoslav.Redis;
using Radoslav.Serialization;

namespace Radoslav.TaskProcessor.Repository.Redis
{
    /// <summary>
    /// An implementation of the <see cref="ITaskProcessorRepository"/> that uses Redis for storage.
    /// </summary>
    public sealed class RedisTaskProcessorRepository : TaskProcessorRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RedisTaskProcessorRepository"/> class.
        /// </summary>
        /// <param name="provider">The Redis provider to use.</param>
        /// <param name="serializer">The serializer to use.</param>
        public RedisTaskProcessorRepository(IRedisProvider provider, IEntityBinarySerializer serializer)
            : base(new RedisTaskRepository(provider, serializer),
                  new RedisTaskRuntimeInfoRepository(provider, serializer),
                  new RedisTaskProcessorRuntimeInfoRepository(provider),
                  new RedisTaskSummaryRepository(provider, serializer),
                  new RedisTaskJobSettingsRepository(provider, serializer),
                  new RedisScheduledTaskRepository(provider, serializer))
        {
        }
    }
}