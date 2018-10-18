using System;

namespace Radoslav.Redis
{
    /// <summary>
    /// A basic functionality of a Redis pipeline.
    /// </summary>
    public interface IRedisPipeline : IRedisQueueableOperation, IDisposable
    {
        /// <summary>
        /// Flushes all commands queued to the pipeline.
        /// </summary>
        void Flush();
    }
}