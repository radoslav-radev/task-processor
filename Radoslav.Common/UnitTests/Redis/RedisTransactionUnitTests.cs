using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Redis;

namespace Radoslav.Common.UnitTests
{
    [TestClass]
    public abstract class RedisTransactionUnitTests : RedisQueueableOperationUnitTests
    {
        protected new IRedisTransaction QueueableOperation
        {
            get
            {
                return (IRedisTransaction)base.QueueableOperation;
            }
        }

        [TestMethod]
        public void Commit()
        {
            string key1 = RedisProviderUnitTests.GetTempRedisKey();
            string key2 = RedisProviderUnitTests.GetTempRedisKey();

            this.QueueableOperation.SetValue(key1, "A");
            this.QueueableOperation.SetValue(key2, "B");

            this.QueueableOperation.Commit();

            Assert.AreEqual("A", this.Provider.GetTextValue(key1));
            Assert.AreEqual("B", this.Provider.GetTextValue(key2));
        }

        [TestMethod]
        public void CommitNothing()
        {
            this.QueueableOperation.Commit();
        }

        [TestMethod]
        public void Rollback()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.QueueableOperation.SetValue(key, "A");

            this.QueueableOperation.Rollback();

            Assert.IsFalse(this.Provider.ContainsKey(key));
        }

        [TestMethod]
        public void RollbackNothing()
        {
            this.QueueableOperation.Rollback();
        }

        [TestMethod]
        public void RollbackOnDispose()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.QueueableOperation.SetValue(key, "A");

            this.QueueableOperation.Dispose();

            Assert.IsFalse(this.Provider.ContainsKey(key));
        }

        protected override void CompleteOperation()
        {
            this.QueueableOperation.Commit();
        }

        protected override IRedisQueueableOperation CreateQueueableOperation()
        {
            return this.Provider.CreateTransaction();
        }
    }
}