using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Timers;

namespace Radoslav.Common.UnitTests
{
    public class TimerTestsBase<TTimer>
        where TTimer : ITimer, new()
    {
        #region Properties

        public TestContext TestContext { get; set; }

        protected TTimer Timer
        {
            get
            {
                return (TTimer)this.TestContext.Properties[typeof(TTimer).Name];
            }

            private set
            {
                this.TestContext.Properties.Add(typeof(TTimer).Name, value);
            }
        }

        #endregion Properties

        #region Initializes & Cleanup

        [TestInitialize]
        public void InitializeTest()
        {
            this.Timer = new TTimer();
        }

        [TestCleanup]
        public void CleanupTest()
        {
            if (this.Timer is IDisposable)
            {
                ((IDisposable)this.Timer).Dispose();
            }
        }

        #endregion Initializes & Cleanup

        [TestMethod]
        public void InitialTimeInterval()
        {
            Assert.AreEqual(TimeSpan.Zero, this.Timer.Interval);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void NegativeTimeInterval()
        {
            this.Timer.Interval = TimeSpan.FromMinutes(-1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ZeroTimeInterval()
        {
            this.Timer.Interval = TimeSpan.Zero;
        }

        [TestMethod]
        public void SetTimeInterval()
        {
            this.Timer.Interval = TimeSpan.FromMinutes(11);

            Assert.AreEqual(TimeSpan.FromMinutes(11), this.Timer.Interval);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void StartWithoutTimeInterval()
        {
            this.Timer.Start();
        }

        [TestMethod]
        public void Start()
        {
            this.Timer.Interval = TimeSpan.FromSeconds(1);

            this.Timer.Start();

            Assert.IsTrue(this.Timer.IsActive);
        }

        [TestMethod]
        public void StartTwice()
        {
            this.Timer.Interval = TimeSpan.FromSeconds(1);

            this.Timer.Start();
            this.Timer.Start();
        }

        [TestMethod]
        public void IsTickEventRaised()
        {
            this.Timer.Interval = TimeSpan.FromSeconds(1);

            this.Timer.Start();

            Helpers.WaitForEvent(TimeSpan.FromSeconds(1.1), handler => this.Timer.Tick += handler);

            Assert.IsTrue(this.WaitForTick(TimeSpan.FromSeconds(1.1)));
            Assert.IsTrue(this.WaitForTick(TimeSpan.FromSeconds(1.1)));
        }

        [TestMethod]
        public void StopBeforeStart()
        {
            this.Timer.Stop();
        }

        [TestMethod]
        public void Stop()
        {
            this.Timer.Interval = TimeSpan.FromSeconds(1);

            this.Timer.Start();
            this.Timer.Stop();

            Assert.IsFalse(this.Timer.IsActive);

            Assert.IsFalse(this.WaitForTick(TimeSpan.FromSeconds(2)));
        }

        [TestMethod]
        public void StopTwice()
        {
            this.Timer.Interval = TimeSpan.FromSeconds(1);

            this.Timer.Start();

            this.Timer.Stop();
            this.Timer.Stop();
        }

        [TestMethod]
        public void IncreaseInterval()
        {
            this.Timer.Interval = TimeSpan.FromSeconds(1);

            this.Timer.Start();

            this.Timer.Interval = TimeSpan.FromSeconds(2);

            Assert.IsFalse(this.WaitForTick(TimeSpan.FromSeconds(1.2)));
            Assert.IsTrue(this.WaitForTick(TimeSpan.FromSeconds(1)));
        }

        [TestMethod]
        public void DecreaseInterval()
        {
            this.Timer.Interval = TimeSpan.FromSeconds(2);

            this.Timer.Start();

            this.Timer.Interval = TimeSpan.FromSeconds(1);

            Assert.IsTrue(this.WaitForTick(TimeSpan.FromSeconds(1.2)));
        }

        private bool WaitForTick(TimeSpan timeout)
        {
            try
            {
                Helpers.WaitForEvent(timeout, handler => this.Timer.Tick += handler);

                return true;
            }
            catch (TimeoutException)
            {
                return false;
            }
        }
    }
}