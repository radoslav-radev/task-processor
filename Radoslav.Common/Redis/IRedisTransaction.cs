using System;

namespace Radoslav.Redis
{
    /// <summary>
    /// A basic functionality of a Redis transaction.
    /// </summary>
    public interface IRedisTransaction : IRedisQueueableOperation, IDisposable
    {
        /// <summary>
        /// Commits all operations queued to the transaction.
        /// </summary>
        void Commit();

        /// <summary>
        /// Rollbacks all operations queued to the transaction.
        /// </summary>
        void Rollback();
    }
}