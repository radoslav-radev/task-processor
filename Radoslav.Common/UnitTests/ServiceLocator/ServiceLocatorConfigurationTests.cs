using System;
using System.Configuration;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Configuration;
using Radoslav.ServiceLocator.Configuration;

namespace Radoslav.UnitTests.ServiceLocator
{
    [TestClass]
    public sealed class ServiceLocatorConfigurationTests
    {
        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void AbstractImplementationType()
        {
            ServiceLocatorConfigurationTests.LoadConfiguration("AbstractImplementationType.config");
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void NoPublicConstructor()
        {
            ServiceLocatorConfigurationTests.LoadConfiguration("NoPublicConstructor.config");
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void MultiplePublicConstructors()
        {
            ServiceLocatorConfigurationTests.LoadConfiguration("MultiplePublicConstructors.config");
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void ContractNotAssignable()
        {
            ServiceLocatorConfigurationTests.LoadConfiguration("ContractNotAssignable.config");
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void MultipleContractsWithSameKey()
        {
            ServiceLocatorConfigurationTests.LoadConfiguration("MultipleTimersWithSameKeys.config");
        }

        [TestMethod]
        public void MultipleContractsWithoutKeys()
        {
            ServiceLocatorConfigurationTests.LoadConfiguration("MultipleTimersWithoutKeys.config");
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void DependencyNotFound()
        {
            ServiceLocatorConfigurationTests.LoadConfiguration("DateTimeProviderNoTimer.config");
        }

        #region Implementation Constructor

        [TestMethod]
        public void ConstructorValueParameter()
        {
            ServiceLocatorConfiguration configuration = ServiceLocatorConfigurationTests.LoadConfiguration("ConstantDelayStrategy.config");

            ConstructorValueConfigurationElement configElement = configuration.Services
                .OfType<ServiceConfigurationElement>()
                .Single()
                .ImplementationConstructorParameters
                .OfType<ConstructorValueConfigurationElement>()
                .Single();

            Assert.AreEqual("00:15:00", configElement.ValueAsString);

            Assert.AreEqual(TimeSpan.FromMinutes(15), configElement.ConvertedValue);
        }

        [TestMethod]
        public void ConstructorResolveParameterByKey()
        {
            ServiceLocatorConfiguration configuration = ServiceLocatorConfigurationTests.LoadConfiguration("DateTimeProviderWithValidTimerKey.config");

            ServiceConfigurationElement serviceConfig = configuration.Services
                .OfType<ServiceConfigurationElement>()
                .Last();

            Assert.IsNotNull(serviceConfig.ImplementationConstructorParameters.ResolvedConstructorInfo);

            ConstructorResolveConfigurationElement configElement = serviceConfig.ImplementationConstructorParameters
                .OfType<ConstructorResolveConfigurationElement>()
                .Single();

            Assert.AreEqual("MyTimer", configElement.ResolveKey);
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void ConstructorResolveParameterWithInvalidKey()
        {
            ServiceLocatorConfigurationTests.LoadConfiguration("DateTimeProviderWithInvalidTimerKey.config");
        }

        #endregion Implementation Constructor

        #region Implementation Properties

        [TestMethod]
        public void PropertyValue()
        {
            ServiceLocatorConfiguration configuration = ServiceLocatorConfigurationTests.LoadConfiguration("TimerWithInterval.config");

            ValuePropertyConfigurationElement configElement = configuration.Services
                .OfType<ServiceConfigurationElement>()
                .Single()
                .ImplementationProperties
                .OfType<ValuePropertyConfigurationElement>()
                .Single();

            Assert.AreEqual("00:15:00", configElement.ValueAsString);

            Assert.AreEqual(TimeSpan.FromMinutes(15), configElement.ConvertedValue);
        }

        [TestMethod]
        public void CollectionPropertyNoKeys()
        {
            ServiceLocatorConfiguration configuration = ServiceLocatorConfigurationTests.LoadConfiguration("DelayStrategiesNoKeys.config");

            CollectionPropertyConfigurationElement configElement = configuration.Services
                .OfType<ServiceConfigurationElement>()
                .Last()
                .ImplementationProperties
                .OfType<CollectionPropertyConfigurationElement>()
                .Single();

            Assert.IsNull(configElement.ResolveKeys);
        }

        [TestMethod]
        public void CollectionPropertyEmptyKey()
        {
            ServiceLocatorConfiguration configuration = ServiceLocatorConfigurationTests.LoadConfiguration("DelayStrategiesEmptyKey.config");

            CollectionPropertyConfigurationElement configElement = configuration.Services
                .OfType<ServiceConfigurationElement>()
                .Last()
                .ImplementationProperties
                .OfType<CollectionPropertyConfigurationElement>()
                .Single();

            Assert.AreEqual(0, configElement.ResolveKeys.Count);
        }

        [TestMethod]
        public void CollectionPropertyOneKey()
        {
            ServiceLocatorConfiguration configuration = ServiceLocatorConfigurationTests.LoadConfiguration("DelayStrategiesOneKey.config");

            CollectionPropertyConfigurationElement configElement = configuration.Services
                .OfType<ServiceConfigurationElement>()
                .Last()
                .ImplementationProperties
                .OfType<CollectionPropertyConfigurationElement>()
                .Single();

            Assert.AreEqual(1, configElement.ResolveKeys.Count);

            Assert.AreEqual("Constant", configElement.ResolveKeys[0]);
        }

        [TestMethod]
        public void CollectionPropertyTwoKeys()
        {
            ServiceLocatorConfiguration configuration = ServiceLocatorConfigurationTests.LoadConfiguration("DelayStrategiesTwoKeys.config");

            CollectionPropertyConfigurationElement configElement = configuration.Services
                .OfType<ServiceConfigurationElement>()
                .Last()
                .ImplementationProperties
                .OfType<CollectionPropertyConfigurationElement>()
                .Single();

            Assert.AreEqual(2, configElement.ResolveKeys.Count);

            Assert.AreEqual("None", configElement.ResolveKeys[0]);
            Assert.AreEqual("Exponential", configElement.ResolveKeys[1]);
        }

        #endregion Implementation Properties

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void DuplicateKeys()
        {
            ServiceLocatorConfigurationTests.LoadConfiguration("DuplicateKeys.config");
        }

        #region Composition

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void ImplementationTypeIsNotCollection()
        {
            ServiceLocatorConfigurationTests.LoadConfiguration("CompositionNoCollection.config");
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void CompositionResolveKeysAll()
        {
            ServiceLocatorConfigurationTests.LoadConfiguration("CompositionResolveKeysAll.config");
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void CompositionResolveKeysEmpty()
        {
            ServiceLocatorConfigurationTests.LoadConfiguration("CompositionResolveKeysEmpty.config");
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void CompositionResolveKeysNone()
        {
            ServiceLocatorConfigurationTests.LoadConfiguration("CompositionResolveKeysNone.config");
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void CompositionResolveUnkownKey()
        {
            ServiceLocatorConfigurationTests.LoadConfiguration("CompositionResolveUnkownKey.config");
        }

        #endregion Composition

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void RegisterLocatorAsService()
        {
            ServiceLocatorConfigurationTests.LoadConfiguration("RegisterLocatorAsService.config");
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void CollectionPropertyDependencyValues()
        {
            ServiceLocatorConfigurationTests.LoadConfiguration("CollectionPropertyDependencyValues.config");
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void ConfigSourcesDuplicateKey()
        {
            ServiceLocatorConfigurationTests.LoadConfiguration("ConfigSourcesDuplicateKey.config");
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void ConfigSourceNotFound()
        {
            ServiceLocatorConfigurationTests.LoadConfiguration("ConfigSourceNotFound.config");
        }

        private static ServiceLocatorConfiguration LoadConfiguration(string configurationFilePath)
        {
            return ConfigurationHelpers.Load<ServiceLocatorConfiguration>(ServiceLocatorConfiguration.AppSectionName, Path.Combine("ServiceLocator", "Config", configurationFilePath));
        }
    }
}