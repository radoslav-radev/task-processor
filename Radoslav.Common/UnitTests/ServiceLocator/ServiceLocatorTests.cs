using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Retryable.DelayStrategy;
using Radoslav.ServiceLocator;
using Radoslav.Timers;

namespace Radoslav.UnitTests
{
    [TestClass]
    public sealed class ServiceLocatorTests
    {
        private const string ConfigFilePropertyName = "ConfigFile";

        #region Properties & Initialize

        public TestContext TestContext { get; set; }

        private string ConfigFilePath
        {
            get
            {
                return (string)this.TestContext.Properties[ServiceLocatorTests.ConfigFilePropertyName];
            }
        }

        private RadoslavServiceLocator ServiceLocator
        {
            get
            {
                return (RadoslavServiceLocator)this.TestContext.Properties[typeof(RadoslavServiceLocator).Name];
            }

            set
            {
                this.TestContext.Properties.Add(typeof(RadoslavServiceLocator).Name, value);
            }
        }

        [TestInitialize]
        public void InitializeTest()
        {
            this.ServiceLocator = new RadoslavServiceLocator(Path.Combine("ServiceLocator", "Config", this.ConfigFilePath));
        }

        #endregion Properties & Initialize

        #region Resolve Single

        [TestMethod]
        [TestProperty(ServiceLocatorTests.ConfigFilePropertyName, "Empty.config")]
        [ExpectedException(typeof(ArgumentException))]
        public void ResolveSingleWithNoDefinitions()
        {
            this.ServiceLocator.ResolveSingle<ITimer>();
        }

        [TestMethod]
        [TestProperty(ServiceLocatorTests.ConfigFilePropertyName, "SingleTimer.config")]
        public void ResolveSingleWithOneDefinition()
        {
            ITimer timer = this.ServiceLocator.ResolveSingle<ITimer>();

            Assert.IsNotNull(timer);
            Assert.IsInstanceOfType(timer, typeof(TimersTimer));
        }

        [TestMethod]
        [TestProperty(ServiceLocatorTests.ConfigFilePropertyName, "MultipleTimersWithDifferentKeys.config")]
        [ExpectedException(typeof(ArgumentException))]
        public void ResolveSingleWithTwoDefinitions()
        {
            this.ServiceLocator.ResolveSingle<ITimer>();
        }

        [TestMethod]
        [TestProperty(ServiceLocatorTests.ConfigFilePropertyName, "MultipleTimersOneWithKey.config")]
        public void ResolveSingleWithoutKey()
        {
            this.ServiceLocator.ResolveSingle<ITimer>();
        }

        #endregion Resolve Single

        #region Resolve Multiple

        [TestMethod]
        [TestProperty(ServiceLocatorTests.ConfigFilePropertyName, "Empty.config")]
        public void ResolveMultipleWithNoDefinitions()
        {
            Assert.AreEqual(0, this.ServiceLocator.ResolveMultiple<ITimer>().Count());
        }

        [TestMethod]
        [TestProperty(ServiceLocatorTests.ConfigFilePropertyName, "SingleTimer.config")]
        public void ResolveMultipleWithOneDefinition()
        {
            IEnumerable<ITimer> timers = this.ServiceLocator.ResolveMultiple<ITimer>();

            Assert.AreEqual(1, timers.Count());

            Assert.IsNotNull(timers.First());
            Assert.IsInstanceOfType(timers.First(), typeof(TimersTimer));
        }

        [TestMethod]
        [TestProperty(ServiceLocatorTests.ConfigFilePropertyName, "MultipleTimersOneWithKey.config")]
        public void ResolveMultipleWithTwoDefinitions()
        {
            IEnumerable<ITimer> timers = this.ServiceLocator.ResolveMultiple<ITimer>();

            Assert.AreEqual(2, timers.Count());

            Assert.IsInstanceOfType(timers.First(), typeof(TimersTimer));
            Assert.IsInstanceOfType(timers.Last(), typeof(ThreadingTimer));
        }

        #endregion Resolve Multiple

        #region Resolve Shared

        [TestMethod]
        [TestProperty(ServiceLocatorTests.ConfigFilePropertyName, "SingleTimer.config")]
        public void ResolveSingleNotShared()
        {
            ITimer first = this.ServiceLocator.ResolveSingle<ITimer>();
            ITimer second = this.ServiceLocator.ResolveSingle<ITimer>();

            Assert.AreNotSame(first, second);
        }

        [TestMethod]
        [TestProperty(ServiceLocatorTests.ConfigFilePropertyName, "SharedTimer.config")]
        public void ResolveSingleShared()
        {
            ITimer first = this.ServiceLocator.ResolveSingle<ITimer>();
            ITimer second = this.ServiceLocator.ResolveSingle<ITimer>();

            Assert.AreSame(first, second);
        }

        [TestMethod]
        [TestProperty(ServiceLocatorTests.ConfigFilePropertyName, "SingleTimer.config")]
        public void ResolveMultipleNotShared()
        {
            IEnumerable<ITimer> first = this.ServiceLocator.ResolveMultiple<ITimer>();
            IEnumerable<ITimer> second = this.ServiceLocator.ResolveMultiple<ITimer>();

            Assert.AreNotSame(first.Single(), second.Single());
        }

        [TestMethod]
        [TestProperty(ServiceLocatorTests.ConfigFilePropertyName, "SharedTimer.config")]
        public void ResolveMultipleShared()
        {
            IEnumerable<ITimer> first = this.ServiceLocator.ResolveMultiple<ITimer>();
            IEnumerable<ITimer> second = this.ServiceLocator.ResolveMultiple<ITimer>();

            Assert.AreSame(first.Single(), second.Single());
        }

        #endregion Resolve Shared

        #region Implementation Constructor

        [TestMethod]
        [TestProperty(ServiceLocatorTests.ConfigFilePropertyName, "DateTimeProviderWithTimer.config")]
        public void ResolveDependency()
        {
            FakeServiceLocatorObjectWithTimer temp = this.ServiceLocator.ResolveSingle<FakeServiceLocatorObjectWithTimer>();

            Assert.IsInstanceOfType(temp.Timer, typeof(TimersTimer));
        }

        [TestMethod]
        [TestProperty(ServiceLocatorTests.ConfigFilePropertyName, "DateTimeProviderWithValidTimerKey.config")]
        public void ResolveDependencyByKey()
        {
            FakeServiceLocatorObjectWithTimer temp = this.ServiceLocator.ResolveSingle<FakeServiceLocatorObjectWithTimer>();

            Assert.IsInstanceOfType(temp.Timer, typeof(TimersTimer));
        }

        #endregion Implementation Constructor

        #region Implementation Properties

        [TestMethod]
        [TestProperty(ServiceLocatorTests.ConfigFilePropertyName, "TimerWithInterval.config")]
        public void PropertyValue()
        {
            ITimer timer = this.ServiceLocator.ResolveSingle<ITimer>();

            Assert.AreEqual(TimeSpan.FromMinutes(15), timer.Interval);
        }

        [TestMethod]
        [TestProperty(ServiceLocatorTests.ConfigFilePropertyName, "DelayStrategiesNoKeys.config")]
        public void CollectionPropertyNoKeys()
        {
            FakeServiceLocatorObject temp = this.ServiceLocator.ResolveSingle<FakeServiceLocatorObject>();

            Assert.AreEqual(3, temp.DelayStrategies.Count);

            Assert.IsTrue(temp.DelayStrategies.OfType<NoDelayStrategy>().Any());
            Assert.IsTrue(temp.DelayStrategies.OfType<ConstantDelayStrategy>().Any());
            Assert.IsTrue(temp.DelayStrategies.OfType<ExponentialDelayStrategy>().Any());
        }

        [TestMethod]
        [TestProperty(ServiceLocatorTests.ConfigFilePropertyName, "DelayStrategiesEmptyKey.config")]
        public void CollectionPropertyEmptyKey()
        {
            FakeServiceLocatorObject temp = this.ServiceLocator.ResolveSingle<FakeServiceLocatorObject>();

            Assert.AreEqual(2, temp.DelayStrategies.Count);

            Assert.IsTrue(temp.DelayStrategies.OfType<NoDelayStrategy>().Any());
            Assert.IsTrue(temp.DelayStrategies.OfType<ExponentialDelayStrategy>().Any());
        }

        [TestMethod]
        [TestProperty(ServiceLocatorTests.ConfigFilePropertyName, "DelayStrategiesOneKey.config")]
        public void CollectionPropertyOneKey()
        {
            FakeServiceLocatorObject temp = this.ServiceLocator.ResolveSingle<FakeServiceLocatorObject>();

            Assert.AreEqual(1, temp.DelayStrategies.Count);

            Assert.IsTrue(temp.DelayStrategies.OfType<NoDelayStrategy>().IsEmpty());
            Assert.IsTrue(temp.DelayStrategies.OfType<ConstantDelayStrategy>().Any());
            Assert.IsTrue(temp.DelayStrategies.OfType<ExponentialDelayStrategy>().IsEmpty());
        }

        [TestMethod]
        [TestProperty(ServiceLocatorTests.ConfigFilePropertyName, "DelayStrategiesTwoKeys.config")]
        public void CollectionPropertyTwoKeys()
        {
            FakeServiceLocatorObject temp = this.ServiceLocator.ResolveSingle<FakeServiceLocatorObject>();

            Assert.AreEqual(2, temp.DelayStrategies.Count);

            Assert.IsTrue(temp.DelayStrategies.OfType<NoDelayStrategy>().Any());
            Assert.IsTrue(temp.DelayStrategies.OfType<ConstantDelayStrategy>().IsEmpty());
            Assert.IsTrue(temp.DelayStrategies.OfType<ExponentialDelayStrategy>().Any());
        }

        #endregion Implementation Properties

        #region Composite Pattern

        [TestMethod]
        [TestProperty(ServiceLocatorTests.ConfigFilePropertyName, "CompositionNone.config")]
        public void CompositeNone()
        {
            IFakeServiceLocatorObject result = this.ServiceLocator.ResolveSingle<IFakeServiceLocatorObject>("Composite");

            Assert.IsInstanceOfType(result, typeof(FakeCompositeServiceLocatorObject));

            FakeCompositeServiceLocatorObject composite = (FakeCompositeServiceLocatorObject)result;

            Assert.AreEqual(0, composite.Count);
        }

        [TestMethod]
        [TestProperty(ServiceLocatorTests.ConfigFilePropertyName, "CompositionAll.config")]
        public void CompositeAll()
        {
            IFakeServiceLocatorObject result = this.ServiceLocator.ResolveSingle<IFakeServiceLocatorObject>("Composite");

            Assert.IsInstanceOfType(result, typeof(FakeCompositeServiceLocatorObject));

            FakeCompositeServiceLocatorObject composite = (FakeCompositeServiceLocatorObject)result;

            Assert.AreEqual(2, composite.Count);

            Assert.IsTrue(composite.OfType<FakeServiceLocatorObject>().Any());
            Assert.IsTrue(composite.OfType<FakeServiceLocatorObjectSimple>().Any());
        }

        [TestMethod]
        [TestProperty(ServiceLocatorTests.ConfigFilePropertyName, "CompositionEmpty.config")]
        public void CompositeEmpty()
        {
            FakeCompositeServiceLocatorObject result = (FakeCompositeServiceLocatorObject)this.ServiceLocator.ResolveSingle<IFakeServiceLocatorObject>("Composite");

            Assert.AreEqual(1, result.Count);

            Assert.IsTrue(result.OfType<FakeServiceLocatorObject>().Any());
        }

        [TestMethod]
        [TestProperty(ServiceLocatorTests.ConfigFilePropertyName, "CompositionExplicit.config")]
        public void CompositeExplicit()
        {
            IFakeServiceLocatorObject result = this.ServiceLocator.ResolveSingle<IFakeServiceLocatorObject>("Composite");

            Assert.IsInstanceOfType(result, typeof(FakeCompositeServiceLocatorObject));

            FakeCompositeServiceLocatorObject composite = (FakeCompositeServiceLocatorObject)result;

            Assert.AreEqual(2, composite.Count);

            Assert.AreEqual(1, composite.OfType<FakeServiceLocatorObject>().Count());
            Assert.AreEqual(1, composite.OfType<FakeServiceLocatorObjectSimple>().Count());
        }

        #endregion Composite Pattern

        #region Other Tests

        [TestMethod]
        [TestProperty(ServiceLocatorTests.ConfigFilePropertyName, "DuplicateServices.config")]
        public void DuplicateServices()
        {
            IDelayStrategy[] strategies = this.ServiceLocator.ResolveMultiple<IDelayStrategy>().ToArray();

            Assert.AreEqual(1, strategies.Length);
        }

        [TestMethod]
        [TestProperty(ServiceLocatorTests.ConfigFilePropertyName, "SharedImplementation.config")]
        public void SharedImplementation()
        {
            Assert.AreSame(this.ServiceLocator.ResolveSingle<IDisposable>(), this.ServiceLocator.ResolveSingle<IComponent>());
        }

        [TestMethod]
        [TestProperty(ServiceLocatorTests.ConfigFilePropertyName, "SetPropertyChainValue.config")]
        public void SetPropertyChainValue()
        {
            FakeServiceLocatorObjectWithTimer temp = this.ServiceLocator.ResolveSingle<FakeServiceLocatorObjectWithTimer>();

            Assert.AreEqual(TimeSpan.FromMinutes(15), temp.Timer.Interval);
        }

        [TestMethod]
        [TestProperty(ServiceLocatorTests.ConfigFilePropertyName, "ResolveDependencyProperty.config")]
        public void ResolveDependencyProperty()
        {
            FakeServiceLocatorObjectSimple temp = this.ServiceLocator.ResolveSingle<FakeServiceLocatorObjectSimple>();

            Assert.IsInstanceOfType(temp.Timer, typeof(TimersTimer));
        }

        [TestMethod]
        [TestProperty(ServiceLocatorTests.ConfigFilePropertyName, "ResolveDependencyPropertyWithKey.config")]
        public void ResolveDependencyPropertyWithKey()
        {
            FakeServiceLocatorObjectSimple temp = this.ServiceLocator.ResolveSingle<FakeServiceLocatorObjectSimple>();

            Assert.IsInstanceOfType(temp.Timer, typeof(TimersTimer));
        }

        [TestMethod]
        [TestProperty(ServiceLocatorTests.ConfigFilePropertyName, "RegisterLocatorAsServiceShared.config")]
        public void RegisterLocatorAsServiceShared()
        {
            Assert.AreSame(this.ServiceLocator, this.ServiceLocator.ResolveSingle<IRadoslavServiceLocator>());
        }

        [TestMethod]
        [TestProperty(ServiceLocatorTests.ConfigFilePropertyName, "CollectionPropertyValues.config")]
        public void CollectionPropertyValues()
        {
            FakeServiceLocatorObject result = this.ServiceLocator.ResolveSingle<FakeServiceLocatorObject>();

            Assert.AreEqual(3, result.SupportedTypes.Count);

            Assert.IsTrue(result.SupportedTypes.Contains(typeof(NoDelayStrategy)));
            Assert.IsTrue(result.SupportedTypes.Contains(typeof(ConstantDelayStrategy)));
            Assert.IsTrue(result.SupportedTypes.Contains(typeof(ExponentialDelayStrategy)));
        }

        [TestMethod]
        [TestProperty(ServiceLocatorTests.ConfigFilePropertyName, "ConfigSources1.config")]
        public void ConfigSources1()
        {
            Assert.IsTrue(this.ServiceLocator.CanResolve<ITimer>());
            Assert.IsTrue(this.ServiceLocator.CanResolve<FakeServiceLocatorObject>());

            Assert.IsNotNull(this.ServiceLocator.ResolveSingle<ITimer>());
            Assert.IsNotNull(this.ServiceLocator.ResolveSingle<FakeServiceLocatorObject>());
        }

        [TestMethod]
        [TestProperty(ServiceLocatorTests.ConfigFilePropertyName, "ConfigSources2.config")]
        public void ConfigSources2()
        {
            Assert.IsTrue(this.ServiceLocator.CanResolve<ITimer>());
            Assert.IsTrue(this.ServiceLocator.CanResolve<FakeServiceLocatorObject>());

            Assert.IsNotNull(this.ServiceLocator.ResolveSingle<ITimer>());
            Assert.IsNotNull(this.ServiceLocator.ResolveSingle<FakeServiceLocatorObject>());
        }

        [TestMethod]
        [TestProperty(ServiceLocatorTests.ConfigFilePropertyName, "ConfigSources3.config")]
        public void ConfigSources3()
        {
            Assert.IsTrue(this.ServiceLocator.CanResolve<ITimer>());
            Assert.IsTrue(this.ServiceLocator.CanResolve<IDelayStrategy>());
            Assert.IsTrue(this.ServiceLocator.CanResolve<FakeServiceLocatorObject>());
            Assert.IsTrue(this.ServiceLocator.CanResolve<FakeServiceLocatorObjectSimple>());

            Assert.IsNotNull(this.ServiceLocator.ResolveSingle<ITimer>());
            Assert.IsNotNull(this.ServiceLocator.ResolveSingle<IDelayStrategy>());
            Assert.IsNotNull(this.ServiceLocator.ResolveSingle<FakeServiceLocatorObject>());
            Assert.IsNotNull(this.ServiceLocator.ResolveSingle<FakeServiceLocatorObjectSimple>());
        }

        [TestMethod]
        [TestProperty(ServiceLocatorTests.ConfigFilePropertyName, "Empty.config")]
        public void CanResolveUndefinedServiceNoMatchingConstructor()
        {
            Assert.IsFalse(this.ServiceLocator.CanResolve<FakeServiceLocatorObjectWithTimer>());
        }

        [TestMethod]
        [TestProperty(ServiceLocatorTests.ConfigFilePropertyName, "Empty.config")]
        [ExpectedException(typeof(ArgumentException))]
        public void ResolveUndefinedServiceNoMatchingConstructor()
        {
            this.ServiceLocator.ResolveSingle<FakeServiceLocatorObjectWithTimer>();
        }

        [TestMethod]
        [TestProperty(ServiceLocatorTests.ConfigFilePropertyName, "SingleTimer.config")]
        public void CanResolveUndefinedService()
        {
            Assert.IsTrue(this.ServiceLocator.CanResolve<FakeServiceLocatorObjectWithTimer>());
        }

        [TestMethod]
        [TestProperty(ServiceLocatorTests.ConfigFilePropertyName, "SingleTimer.config")]
        public void ResolveUndefinedService()
        {
            this.ServiceLocator.ResolveSingle<FakeServiceLocatorObjectWithTimer>();
        }

        [TestMethod]
        [TestProperty(ServiceLocatorTests.ConfigFilePropertyName, "OptionalKeys1.config")]
        public void OptionalKeys1()
        {
            FakeServiceLocatorObject result = this.ServiceLocator.ResolveSingle<FakeServiceLocatorObject>();

            Assert.AreEqual(3, result.DelayStrategies.Count);

            Assert.IsTrue(result.DelayStrategies.OfType<NoDelayStrategy>().Any());
            Assert.IsTrue(result.DelayStrategies.OfType<ConstantDelayStrategy>().Any());
            Assert.IsTrue(result.DelayStrategies.OfType<ExponentialDelayStrategy>().Any());
        }

        [TestMethod]
        [TestProperty(ServiceLocatorTests.ConfigFilePropertyName, "OptionalKeys2.config")]
        public void OptionalKeys2()
        {
            FakeServiceLocatorObject result = this.ServiceLocator.ResolveSingle<FakeServiceLocatorObject>();

            Assert.AreEqual(1, result.DelayStrategies.Count);

            Assert.IsTrue(result.DelayStrategies.OfType<ConstantDelayStrategy>().Any());
        }

        [TestMethod]
        [TestProperty(ServiceLocatorTests.ConfigFilePropertyName, "OptionalKeys3.config")]
        public void OptionalKeys3()
        {
            FakeServiceLocatorObject result = this.ServiceLocator.ResolveSingle<FakeServiceLocatorObject>();

            Assert.AreEqual(2, result.DelayStrategies.Count);

            Assert.IsTrue(result.DelayStrategies.OfType<ConstantDelayStrategy>().Any());
            Assert.IsTrue(result.DelayStrategies.OfType<ExponentialDelayStrategy>().Any());
        }

        [TestMethod]
        [TestProperty(ServiceLocatorTests.ConfigFilePropertyName, "OptionalKeys4.config")]
        public void OptionalKeys4()
        {
            FakeServiceLocatorObject result = this.ServiceLocator.ResolveSingle<FakeServiceLocatorObject>();

            Assert.AreEqual(3, result.DelayStrategies.Count);

            Assert.IsTrue(result.DelayStrategies.OfType<NoDelayStrategy>().Any());
            Assert.IsTrue(result.DelayStrategies.OfType<ConstantDelayStrategy>().Any());
            Assert.IsTrue(result.DelayStrategies.OfType<ExponentialDelayStrategy>().Any());
        }

        [TestMethod]
        [TestProperty(ServiceLocatorTests.ConfigFilePropertyName, "RemoveServiceByKey.config")]
        public void RemoveServiceByKey()
        {
            FakeServiceLocatorObject result = this.ServiceLocator.ResolveSingle<FakeServiceLocatorObject>();

            Assert.AreEqual(2, result.DelayStrategies.Count);

            Assert.IsTrue(result.DelayStrategies.OfType<ConstantDelayStrategy>().Any());
            Assert.IsTrue(result.DelayStrategies.OfType<ExponentialDelayStrategy>().Any());
        }

        [TestMethod]
        [TestProperty(ServiceLocatorTests.ConfigFilePropertyName, "RemoveServices.config")]
        public void RemoveServices()
        {
            FakeServiceLocatorObject result = this.ServiceLocator.ResolveSingle<FakeServiceLocatorObject>();

            Assert.AreEqual(0, result.DelayStrategies.Count);
        }

        #endregion Other Tests

        #region CanResolve

        [TestMethod]
        [TestProperty(ServiceLocatorTests.ConfigFilePropertyName, "SingleTimer.config")]
        public void CanResolveTrue()
        {
            Assert.IsTrue(this.ServiceLocator.CanResolve<ITimer>());
        }

        [TestMethod]
        [TestProperty(ServiceLocatorTests.ConfigFilePropertyName, "SingleTimer.config")]
        public void CanResolveFalse()
        {
            Assert.IsFalse(this.ServiceLocator.CanResolve<ITimerFactory>());
        }

        #endregion CanResolve
    }
}