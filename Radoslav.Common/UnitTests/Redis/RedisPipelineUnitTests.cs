using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Redis;

namespace Radoslav.Common.UnitTests
{
    [TestClass]
    public abstract class RedisPipelineUnitTests : RedisQueueableOperationUnitTests
    {
        protected new IRedisPipeline QueueableOperation
        {
            get
            {
                return (IRedisPipeline)base.QueueableOperation;
            }
        }

        protected override void CompleteOperation()
        {
            this.QueueableOperation.Flush();
        }

        protected override IRedisQueueableOperation CreateQueueableOperation()
        {
            return this.Provider.CreatePipeline();
        }
    }
}