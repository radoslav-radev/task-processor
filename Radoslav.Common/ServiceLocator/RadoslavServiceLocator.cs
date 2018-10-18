using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Radoslav.Configuration;
using Radoslav.ServiceLocator.Configuration;

namespace Radoslav.ServiceLocator
{
    /// <summary>
    /// An implementation of <see cref="IRadoslavServiceLocator" /> that uses App.config file to define contracts and implementations.
    /// </summary>
    public sealed class RadoslavServiceLocator : IRadoslavServiceLocator
    {
        private static readonly Lazy<RadoslavServiceLocator> DefaultInstanceInternal = new Lazy<RadoslavServiceLocator>(() => new RadoslavServiceLocator());

        private readonly ServiceLocatorConfiguration configuration;
        private readonly Dictionary<Type, object> singletonInstancesByImplementationType = new Dictionary<Type, object>();

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RadoslavServiceLocator"/> class.
        /// </summary>
        /// <param name="configurationFilePath">Path to a service locator configuration file.</param>
        /// <exception cref="ConfigurationErrorsException">Failed to load configuration from the specified file.</exception>
        public RadoslavServiceLocator(string configurationFilePath)
            : this(ConfigurationHelpers.Load<ServiceLocatorConfiguration>(ServiceLocatorConfiguration.AppSectionName, configurationFilePath))
        {
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="RadoslavServiceLocator"/> class from being created.
        /// </summary>
        /// <exception cref="ConfigurationErrorsException">Failed to load configuration from App.config file.</exception>
        private RadoslavServiceLocator()
            : this(ConfigurationHelpers.Load<ServiceLocatorConfiguration>(ServiceLocatorConfiguration.AppSectionName))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RadoslavServiceLocator"/> class.
        /// </summary>
        /// <param name="configuration">Service locator configuration.</param>
        /// <exception cref="ArgumentNullException">Parameter <see cref="configuration"/> is null.</exception>
        private RadoslavServiceLocator(ServiceLocatorConfiguration configuration)
        {
            Trace.WriteLine("ENTER: Constructing {0} ...".FormatInvariant(this.GetType().Name));

            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            this.configuration = configuration;

            if (this.configuration.Services.HasSelfRegistration)
            {
                this.singletonInstancesByImplementationType.Add(typeof(RadoslavServiceLocator), this);
            }

            Trace.WriteLine("EXIT: {0} constructed.".FormatInvariant(this.GetType().Name));
        }

        #endregion Constructors

        /// <summary>
        /// Gets the default instance of the service locator.
        /// </summary>
        /// <remarks>The default instance is initialized with the configuration from the App.config file.</remarks>
        /// <value>The default instance of the service locator.</value>
        public static RadoslavServiceLocator DefaultInstance
        {
            get
            {
                return RadoslavServiceLocator.DefaultInstanceInternal.Value;
            }
        }

        #region IRadoslavServiceLocator Members

        /// <inheritdoc />
        public bool CanResolve(Type contractType)
        {
            if (contractType == null)
            {
                throw new ArgumentNullException(nameof(contractType));
            }

            if (this.configuration.Services.CanResolve(contractType))
            {
                return true;
            }

            return contractType.GetConstructors().Count(c => c.GetParameters().AllOrEmpty(p => this.CanResolve(p.ParameterType))) == 1;
        }

        /// <inheritdoc />
        public object ResolveSingle(Type contractType)
        {
            if (contractType == null)
            {
                throw new ArgumentNullException(nameof(contractType));
            }

            if (this.configuration.Services.CanResolve(contractType))
            {
                ServiceConfigurationElement serviceConfig = this.configuration.Services.ResolveSingle(contractType);

                return this.ResolveInstance(serviceConfig);
            }

            ConstructorInfo[] constructors = contractType.GetConstructors()
                 .Where(c => c.GetParameters().AllOrEmpty(p => this.CanResolve(p.ParameterType)))
                 .ToArray();

            switch (constructors.Length)
            {
                case 0:
                    throw new ArgumentException("Cannot resolve contract '{0}' because it is not defined and there is no public constructor that could be used.".FormatInvariant(contractType));

                case 1:
                    return constructors[0].Invoke(constructors[0].GetParameters()
                        .Select(p => this.ResolveSingle(p.ParameterType))
                        .ToArray());

                default:
                    throw new ArgumentException("Cannot resolve contract '{0}' because it is not defined and there are multiple public constructors that could be used.".FormatInvariant(contractType));
            }
        }

        /// <inheritdoc />
        private object ResolveSingle(Type contractType, string key)
        {
            ServiceConfigurationElement serviceConfig = this.configuration.Services.ResolveSingle(contractType, key);

            return this.ResolveInstance(serviceConfig);
        }

        /// <inheritdoc />
        public IEnumerable ResolveMultiple(Type contractType)
        {
            if (contractType == null)
            {
                throw new ArgumentNullException(nameof(contractType));
            }

            return this.configuration.Services.ResolveMultiple(contractType)
                .Select(e => this.ResolveInstance(e));
        }

        #endregion IRadoslavServiceLocator Members

        // For unit tests.
        internal TContract ResolveSingle<TContract>(string key)
        {
            return (TContract)this.ResolveSingle(typeof(TContract), key);
        }

        private object ResolveInstance(ServiceConfigurationElement configElement)
        {
            if (configElement.IsShared)
            {
                return this.ResolveSingleton(configElement);
            }
            else
            {
                return this.CreateAndInitializeInstance(configElement, false);
            }
        }

        private object ResolveSingleton(ServiceConfigurationElement configElement)
        {
            object result;

            if (!this.singletonInstancesByImplementationType.TryGetValue(configElement.ImplementationType, out result))
            {
                result = this.CreateAndInitializeInstance(configElement, true);
            }

            return result;
        }

        private object ResolveConstructorAndCreateInstance(ServiceConfigurationElement configElement)
        {
            List<object> constructorArguments = new List<object>();

            foreach (ParameterInfo constructorParameter in configElement.ImplementationConstructorParameters.ResolvedConstructorInfo.GetParameters())
            {
                object argument;

                ConstructorConfigurationElementBase constructorParameterConfig = configElement.ImplementationConstructorParameters[constructorParameter.Name];

                if (constructorParameterConfig == null)
                {
                    argument = this.ResolveSingle(constructorParameter.ParameterType);
                }
                else if (constructorParameterConfig is ConstructorValueConfigurationElement)
                {
                    argument = ((ConstructorValueConfigurationElement)constructorParameterConfig).ConvertedValue;
                }
                else if (constructorParameterConfig is ConstructorResolveConfigurationElement)
                {
                    argument = this.ResolveSingle(constructorParameter.ParameterType, ((ConstructorResolveConfigurationElement)constructorParameterConfig).ResolveKey);
                }
                else
                {
                    throw new NotSupportedException<ConstructorConfigurationElementBase>(constructorParameterConfig);
                }

                constructorArguments.Add(argument);
            }

            return configElement.ImplementationConstructorParameters.ResolvedConstructorInfo.Invoke(constructorArguments.ToArray());
        }

        private void ComposeInstance(ServiceConfigurationElement configElement, object result)
        {
            IEnumerable<ServiceConfigurationElement> childrenConfig;

            switch (configElement.ImplementationComposition.Mode)
            {
                case CompositionMode.None:
                    childrenConfig = null;
                    break;

                case CompositionMode.All:
                    childrenConfig = this.configuration.Services.ResolveMultiple(configElement.ContractType)
                        .Except(configElement);
                    break;

                case CompositionMode.Empty:
                    childrenConfig = this.configuration.Services.ResolveMultiple(configElement.ContractType, string.Empty)
                        .Except(configElement);
                    break;

                case CompositionMode.Explicit:
                    childrenConfig = configElement.ImplementationComposition.ResolveKeys
                        .OfType<string>()
                        .Select(key => this.configuration.Services.ResolveSingle(configElement.ContractType, key));
                    break;

                default:
                    throw new NotSupportedException<CompositionMode>(configElement.ImplementationComposition.Mode);
            }

            if (childrenConfig != null)
            {
                MethodInfo addMethod = typeof(ICollection<>).MakeGenericType(configElement.ContractType).GetMethod("Add");

                foreach (ServiceConfigurationElement childConfig in childrenConfig)
                {
                    object child = this.ResolveInstance(childConfig);

                    addMethod.Invoke(result, new object[] { child });
                }
            }
        }

        private void InitializeProperties(ServiceConfigurationElement configElement, object result)
        {
            foreach (PropertyConfigurationElementBase propertyConfig in configElement.ImplementationProperties.OfType<PropertyConfigurationElementBase>())
            {
                ValuePropertyConfigurationElement valueConfig = propertyConfig as ValuePropertyConfigurationElement;

                if (valueConfig != null)
                {
                    valueConfig.SetPropertyValue(result, valueConfig.ConvertedValue);

                    continue;
                }

                ResolvePropertyConfigurationElement dependencyConfig = propertyConfig as ResolvePropertyConfigurationElement;

                if (dependencyConfig != null)
                {
                    object dependency;

                    if (string.IsNullOrEmpty(dependencyConfig.ResolveKey))
                    {
                        dependency = this.ResolveSingle(dependencyConfig.DependencyType);
                    }
                    else
                    {
                        dependency = this.ResolveSingle(dependencyConfig.DependencyType, dependencyConfig.ResolveKey);
                    }

                    dependencyConfig.SetPropertyValue(result, dependency);

                    continue;
                }

                CollectionPropertyConfigurationElement collectionConfig = propertyConfig as CollectionPropertyConfigurationElement;

                if (collectionConfig != null)
                {
                    switch (collectionConfig.ResolveType)
                    {
                        case CollectionPropertyConfigurationElement.CollectionResolveType.Dependency:
                            {
                                IEnumerable items = null;

                                if (collectionConfig.ResolveKeys == null)
                                {
                                    if (collectionConfig.OptionalKeys == null)
                                    {
                                        items = this.configuration.Services.ResolveMultiple(collectionConfig.CollectionElementType)
                                            .Except(configElement)
                                            .Select(e => this.ResolveInstance(e));
                                    }
                                }
                                else if (collectionConfig.ResolveKeys.Count == 0)
                                {
                                    items = this.configuration.Services.ResolveMultiple(collectionConfig.CollectionElementType, string.Empty)
                                        .Except(configElement)
                                        .Select(e => this.ResolveInstance(e));
                                }
                                else
                                {
                                    items = collectionConfig.ResolveKeys
                                        .OfType<string>()
                                        .Select(key => this.ResolveSingle(collectionConfig.CollectionElementType, key))
                                        .ToArray();
                                }

                                if (items != null)
                                {
                                    collectionConfig.AddToCollection(result, items);
                                }

                                if (collectionConfig.OptionalKeys != null)
                                {
                                    items = collectionConfig.OptionalKeys
                                        .OfType<string>()
                                        .Where(key => this.configuration.Services.CanResolve(collectionConfig.CollectionElementType, key))
                                        .Select(key => this.ResolveSingle(collectionConfig.CollectionElementType, key))
                                        .ToArray();

                                    collectionConfig.AddToCollection(result, items);
                                }
                            }

                            break;

                        case CollectionPropertyConfigurationElement.CollectionResolveType.Values:
                            collectionConfig.AddValuesToCollection(result);

                            break;

                        default:
                            throw new NotSupportedException<CollectionPropertyConfigurationElement.CollectionResolveType>(collectionConfig.ResolveType);
                    }

                    continue;
                }

                throw new NotSupportedException<PropertyConfigurationElementBase>(propertyConfig);
            }
        }

        private object CreateAndInitializeInstance(ServiceConfigurationElement configElement, bool addSingleton)
        {
            object result = this.ResolveConstructorAndCreateInstance(configElement);

            if (addSingleton)
            {
                this.singletonInstancesByImplementationType.Add(configElement.ImplementationType, result);
            }

            this.ComposeInstance(configElement, result);

            this.InitializeProperties(configElement, result);

            return result;
        }
    }
}