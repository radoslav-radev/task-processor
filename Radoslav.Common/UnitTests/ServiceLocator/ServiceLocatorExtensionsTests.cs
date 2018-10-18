using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.ServiceLocator;
using Radoslav.Timers;

namespace Radoslav.Common.UnitTests
{
    [TestClass]
    public sealed class ServiceLocatorExtensionsTests
    {
        public TestContext TestContext { get; set; }

        #region Properties & Initialize

        private FakeServiceLocator ServiceLocator
        {
            get
            {
                return (FakeServiceLocator)this.TestContext.Properties["ServiceLocator"];
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            this.TestContext.Properties.Add("ServiceLocator", new FakeServiceLocator());
        }

        #endregion Properties & Initialize

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void CanResolveNullLocator()
        {
            IRadoslavServiceLocator locator = null;

            locator.CanResolve<ITimer>();
        }

        [TestMethod]
        public void CanResolveTrue()
        {
            this.ServiceLocator.PredefineResult(true, sl => sl.CanResolve(typeof(ITimer)));

            Assert.IsTrue(this.ServiceLocator.CanResolve<ITimer>());

            this.ServiceLocator.AssertMethodCallOnceWithArguments(sl => sl.CanResolve(typeof(ITimer)));
        }

        [TestMethod]
        public void CanResolveFalse()
        {
            this.ServiceLocator.PredefineResult(false, sl => sl.CanResolve(typeof(ITimer)));

            Assert.IsFalse(this.ServiceLocator.CanResolve<ITimer>());

            this.ServiceLocator.AssertMethodCallOnceWithArguments(sl => sl.CanResolve(typeof(ITimer)));
        }

        [TestMethod]
        public void ResolveSingle()
        {
            ITimer timer = new FakeTimer();

            this.ServiceLocator.PredefineResult(timer, sl => sl.ResolveSingle(typeof(ITimer)));

            Assert.AreSame(timer, this.ServiceLocator.ResolveSingle<ITimer>());

            this.ServiceLocator.AssertMethodCallOnceWithArguments(sl => sl.ResolveSingle(typeof(ITimer)));
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void ResolveSingleNullLocator()
        {
            IRadoslavServiceLocator locator = null;

            locator.ResolveSingle<ITimer>();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void ResolveSingleInvalidCast()
        {
            this.ServiceLocator.PredefineResult(new FakeDelayStrategy(), sl => sl.ResolveSingle(typeof(ITimer)));

            this.ServiceLocator.ResolveSingle<ITimer>();
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void ResolveSingleContractNullLocator()
        {
            IRadoslavServiceLocator locator = null;

            locator.ResolveSingle<ITimer>(typeof(ITimer));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ResolveSingleContractNullContractType()
        {
            this.ServiceLocator.ResolveSingle<ITimer>(default(Type));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ResolveSingleContractInvalidContractType()
        {
            this.ServiceLocator.ResolveSingle<ITimer>(typeof(FakeDelayStrategy));
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void ResolveMultipleNullLocator()
        {
            IRadoslavServiceLocator locator = null;

            locator.ResolveMultiple<ITimer>();
        }

        [TestMethod]
        public void ResolveMultiple()
        {
            IEnumerable<ITimer> result1 = new ITimer[] { new FakeTimer(), new TimersTimer() };

            this.ServiceLocator.PredefineResult(result1, sl => sl.ResolveMultiple(typeof(ITimer)));

            IEnumerable<ITimer> result2 = this.ServiceLocator.ResolveMultiple<ITimer>();

            Assert.AreEqual(result1.Count(), result2.Count());
            Assert.AreSame(result1.First(), result2.First());
            Assert.AreSame(result1.Last(), result2.Last());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void ResolveMultipleInvalidCast()
        {
            this.ServiceLocator.PredefineResult(new[] { new FakeDelayStrategy() }, sl => sl.ResolveMultiple(typeof(ITimer)));

            this.ServiceLocator.ResolveMultiple<ITimer>().ToArray();
        }
    }
}