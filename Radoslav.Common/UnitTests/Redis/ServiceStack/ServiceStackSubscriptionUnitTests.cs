using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Redis;

namespace Radoslav.Common.UnitTests
{
    [TestClass]
    public sealed class ServiceStackSubscriptionUnitTests : RedisSubscriptionUnitTests
    {
        protected override IRedisMessageSubscription CreateSubscription()
        {
            return this.RedisProvider.CreateSubscription();
        }

        protected override void PublishMessage(string channel, string message)
        {
            this.RedisProvider.PublishMessage(channel, message);
        }
    }
}