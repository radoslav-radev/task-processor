using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Retryable.DelayStrategy;

namespace Radoslav.Common.UnitTests
{
    [TestClass]
    public class DelayStrategyTests
    {
        [TestMethod]
        public void None()
        {
            IDelayStrategy strategy = new NoDelayStrategy();

            Assert.AreEqual(TimeSpan.Zero, strategy.NextDelay());
            Assert.AreEqual(TimeSpan.Zero, strategy.NextDelay());
            Assert.AreEqual(TimeSpan.Zero, strategy.NextDelay());
            Assert.AreEqual(TimeSpan.Zero, strategy.NextDelay());
        }

        [TestMethod]
        public void Constant()
        {
            IDelayStrategy strategy = new ConstantDelayStrategy(TimeSpan.FromSeconds(1));

            Assert.AreEqual(TimeSpan.FromSeconds(1), strategy.NextDelay());
            Assert.AreEqual(TimeSpan.FromSeconds(1), strategy.NextDelay());
            Assert.AreEqual(TimeSpan.FromSeconds(1), strategy.NextDelay());
            Assert.AreEqual(TimeSpan.FromSeconds(1), strategy.NextDelay());
        }

        [TestMethod]
        public void Exponential()
        {
            IDelayStrategy strategy = new ExponentialDelayStrategy(TimeSpan.FromMilliseconds(100));

            Assert.AreEqual(TimeSpan.FromMilliseconds(100), strategy.NextDelay());
            Assert.AreEqual(TimeSpan.FromMilliseconds(200), strategy.NextDelay());
            Assert.AreEqual(TimeSpan.FromMilliseconds(400), strategy.NextDelay());
            Assert.AreEqual(TimeSpan.FromMilliseconds(800), strategy.NextDelay());
        }
    }
}