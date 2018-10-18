using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Redis;
using Radoslav.Redis.ServiceStack;

namespace Radoslav.Common.UnitTests
{
    [TestClass]
    public abstract class RedisSubscriptionUnitTests
    {
        private static int nextTestChannel;

        private readonly TimeSpan subscribeTimeout = TimeSpan.FromSeconds(2);

        public TestContext TestContext { get; set; }

        protected IRedisProvider RedisProvider
        {
            get
            {
                return (IRedisProvider)this.TestContext.Properties["RedisProvider"];
            }
        }

        private IRedisMessageSubscription Subscription
        {
            get
            {
                return (IRedisMessageSubscription)this.TestContext.Properties["Subscription"];
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            this.TestContext.Properties.Add("RedisProvider", new ServiceStackRedisProvider());
            this.TestContext.Properties.Add("Subscription", this.CreateSubscription());
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this.Subscription.Dispose();
            this.RedisProvider.Dispose();
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void SubscribeToChannelsAfterDispose()
        {
            this.Subscription.Dispose();
            this.Subscription.SubscribeToChannels(TimeSpan.FromSeconds(1), "Test");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SubscribeToChannelsZeroTimeout()
        {
            this.Subscription.SubscribeToChannels(TimeSpan.Zero, "Test");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SubscribeToChannelsNegativeTimeout()
        {
            this.Subscription.SubscribeToChannels(TimeSpan.FromSeconds(-1), "Test");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SubscribeToChannelsNull()
        {
            this.Subscription.SubscribeToChannels(TimeSpan.FromSeconds(1), null);
        }

        [TestMethod]
        public void SubscribeToZeroChannels()
        {
            this.Subscription.SubscribeToChannels(TimeSpan.FromTicks(1), Enumerable.Empty<string>());
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void SubscribeToChannelsTimeout()
        {
            string channel = RedisSubscriptionUnitTests.GetUniqueTestChannel();

            this.Subscription.SubscribeToChannels(TimeSpan.FromTicks(1), channel);
        }

        [TestMethod]
        public void SubscribeToOneChannel()
        {
            string channel = RedisSubscriptionUnitTests.GetUniqueTestChannel();

            this.Subscription.SubscribeToChannels(this.subscribeTimeout, channel);

            Assert.AreEqual(1, this.Subscription.ActiveChannels.Count());
            Assert.IsTrue(this.Subscription.ActiveChannels.Contains(channel));
        }

        [TestMethod]
        public void SubscribeToTwoChannels()
        {
            string channel1 = RedisSubscriptionUnitTests.GetUniqueTestChannel();
            string channel2 = RedisSubscriptionUnitTests.GetUniqueTestChannel();

            this.Subscription.SubscribeToChannels(this.subscribeTimeout, channel1, channel2);

            Assert.AreEqual(2, this.Subscription.ActiveChannels.Count());
            Assert.IsTrue(this.Subscription.ActiveChannels.Contains(channel1));
            Assert.IsTrue(this.Subscription.ActiveChannels.Contains(channel2));
        }

        [TestMethod]
        public void ReceiveMessageForSubscribedChannel()
        {
            string channel1 = RedisSubscriptionUnitTests.GetUniqueTestChannel();

            this.Subscription.SubscribeToChannels(this.subscribeTimeout, channel1);

            Assert.AreEqual(1, this.Subscription.ActiveChannels.Count());

            Assert.IsTrue(this.Subscription.ActiveChannels.Contains(channel1));

            RedisMessageEventArgs args = Helpers.WaitForEvent<RedisMessageEventArgs>(
                TimeSpan.FromSeconds(1),
                handler => this.Subscription.MessageReceived += handler,
                () => this.PublishMessage(channel1, "Hello"));

            Assert.AreEqual(channel1, args.Channel);
            Assert.AreEqual("Hello", args.Message);
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotReceiveMessageIfNotSubscribedForChannel()
        {
            string channel1 = RedisSubscriptionUnitTests.GetUniqueTestChannel();
            string channel2 = RedisSubscriptionUnitTests.GetUniqueTestChannel();

            this.Subscription.SubscribeToChannels(TimeSpan.FromSeconds(1), channel1);

            Helpers.WaitForEvent<RedisMessageEventArgs>(
                TimeSpan.FromSeconds(1),
                handler => this.Subscription.MessageReceived += handler,
                () => this.PublishMessage(channel2, "Hello"));
        }

        [TestMethod]
        public void UnsubscribeFromOneChannel()
        {
            string channel = RedisSubscriptionUnitTests.GetUniqueTestChannel();

            this.Subscription.SubscribeToChannels(this.subscribeTimeout, channel);

            Assert.IsTrue(this.Subscription.UnsubscribeFromChannels(this.subscribeTimeout, channel));

            Assert.IsFalse(this.Subscription.ActiveChannels.Contains(channel));
        }

        [TestMethod]
        public void UnsubscribeFromTwoChannels()
        {
            string channel1 = RedisSubscriptionUnitTests.GetUniqueTestChannel();
            string channel2 = RedisSubscriptionUnitTests.GetUniqueTestChannel();

            this.Subscription.SubscribeToChannels(this.subscribeTimeout, channel1, channel2);

            Assert.IsTrue(this.Subscription.UnsubscribeFromChannels(this.subscribeTimeout, channel1, channel2));

            Assert.AreEqual(0, this.Subscription.ActiveChannels.Count());
        }

        [TestMethod]
        public void UnsubscribeFromOneChannelOutOfTwo()
        {
            string channel1 = RedisSubscriptionUnitTests.GetUniqueTestChannel();
            string channel2 = RedisSubscriptionUnitTests.GetUniqueTestChannel();

            this.Subscription.SubscribeToChannels(this.subscribeTimeout, channel1, channel2);

            Assert.IsTrue(this.Subscription.UnsubscribeFromChannels(this.subscribeTimeout, channel1));

            Assert.AreEqual(1, this.Subscription.ActiveChannels.Count());
            Assert.IsFalse(this.Subscription.ActiveChannels.Contains(channel1));
            Assert.IsTrue(this.Subscription.ActiveChannels.Contains(channel2));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UnsubscribeFromChannelsNull()
        {
            this.Subscription.UnsubscribeFromChannels(TimeSpan.FromSeconds(1), null);
        }

        [TestMethod]
        public void UnsubscribeFromChannelsNoChannelsSpecified()
        {
            this.Subscription.UnsubscribeFromChannels(TimeSpan.FromSeconds(1), Enumerable.Empty<string>());
        }

        [TestMethod]
        public void UnsubscribeFromChannelWithoutSubscribeFirst()
        {
            Assert.IsTrue(this.Subscription.UnsubscribeFromChannels(TimeSpan.FromSeconds(2), "Test3"));

            Assert.AreEqual(0, this.Subscription.ActiveChannels.Count());
            Assert.IsFalse(this.Subscription.ActiveChannels.Contains("Test3"));
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void UnsubscribeFromChannelsAfterDispose()
        {
            this.Subscription.Dispose();
            this.Subscription.UnsubscribeFromChannels(TimeSpan.FromSeconds(1), "Test");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void UnsubscribeFromChannelsZeroTimeout()
        {
            this.Subscription.UnsubscribeFromChannels(TimeSpan.Zero, "Test");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void UnsubscribeFromChannelsNegativeTimeout()
        {
            this.Subscription.UnsubscribeFromChannels(TimeSpan.FromSeconds(-1), "Test");
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotReceiveMessageAfterUnsubscribeFromChannel()
        {
            string channel = RedisSubscriptionUnitTests.GetUniqueTestChannel();

            this.Subscription.SubscribeToChannels(this.subscribeTimeout, channel);

            Assert.IsTrue(this.Subscription.UnsubscribeFromChannels(this.subscribeTimeout, channel));

            Helpers.WaitForEvent<RedisMessageEventArgs>(
                TimeSpan.FromSeconds(1),
                handler => this.Subscription.MessageReceived += handler,
                () => this.PublishMessage(channel, "Hello"));
        }

        protected abstract IRedisMessageSubscription CreateSubscription();

        protected abstract void PublishMessage(string channel, string message);

        private static string GetUniqueTestChannel()
        {
            return "TestChannel" + nextTestChannel++;
        }
    }
}