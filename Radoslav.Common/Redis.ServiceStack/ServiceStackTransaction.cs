using System;
using ServiceStack.Redis;

namespace Radoslav.Redis.ServiceStack
{
    internal sealed class ServiceStackTransaction : ServiceStackQueueableOperation, IRedisTransaction
    {
        private readonly global::ServiceStack.Redis.IRedisTransaction transaction;

        internal ServiceStackTransaction(RedisClient client, global::ServiceStack.Redis.IRedisTransaction transaction)
            : base(client, transaction)
        {
            this.transaction = transaction;
        }

        #region IRedisTransaction Members

        public void Commit()
        {
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            if (!this.IsEmpty)
            {
                this.transaction.Commit();

                this.IsEmpty = true;
            }
        }

        public void Rollback()
        {
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            if (!this.IsEmpty)
            {
                this.transaction.Rollback();

                this.IsEmpty = true;
            }
        }

        #endregion IRedisTransaction Members

        protected override void Dispose(bool disposing)
        {
            this.transaction.Dispose();

            base.Dispose(disposing);
        }
    }
}