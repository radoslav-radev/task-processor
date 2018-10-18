using System;
using ServiceStack.Redis;

namespace Radoslav.Redis.ServiceStack
{
    internal sealed class ServiceStackPipeline : ServiceStackQueueableOperation, IRedisPipeline
    {
        private readonly global::ServiceStack.Redis.Pipeline.IRedisPipeline pipeline;

        internal ServiceStackPipeline(RedisClient client, global::ServiceStack.Redis.Pipeline.IRedisPipeline pipeline)
            : base(client, pipeline)
        {
            this.pipeline = pipeline;
        }

        #region IRedisPipeline Members

        public void Flush()
        {
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            if (!this.IsEmpty)
            {
                this.pipeline.Flush();

                this.IsEmpty = true;
            }
        }

        #endregion IRedisPipeline Members

        protected override void Dispose(bool disposing)
        {
            this.pipeline.Dispose();

            base.Dispose(disposing);
        }
    }
}