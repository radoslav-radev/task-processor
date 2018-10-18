using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace Radoslav.ServiceLocator.Configuration
{
    [ConfigurationCollection(typeof(ServiceConfigurationElement))]
    internal sealed class ServicesConfigurationCollection : ConfigurationElementCollection
    {
        private readonly Dictionary<Type, List<ServiceConfigurationElement>> contractImplementations = new Dictionary<Type, List<ServiceConfigurationElement>>();
        private readonly List<RemoveServiceConfigurationElement> servicesToRemove = new List<RemoveServiceConfigurationElement>();

        internal bool HasSelfRegistration
        {
            get
            {
                return this.contractImplementations.ContainsKey(typeof(IRadoslavServiceLocator));
            }
        }

        internal bool CanResolve(Type contractType)
        {
            return this.contractImplementations.ContainsKey(contractType);
        }

        internal ServiceConfigurationElement ResolveSingle(Type contractType)
        {
            return this.ResolveSingle(contractType, string.Empty);
        }

        internal ServiceConfigurationElement ResolveSingle(Type contractType, string key)
        {
            List<ServiceConfigurationElement> implementations;

            if (!this.contractImplementations.TryGetValue(contractType, out implementations))
            {
                throw new ArgumentException("Contract type '{0}' was not found in {1} configuration.".FormatInvariant(contractType, this.GetType().Name));
            }

            implementations = implementations
                .Where(e => e.Key == key)
                .ToList();

            switch (implementations.Count)
            {
                case 0:
                    throw new ArgumentException("Contract type '{0}' with key '{1}' was not found in {2} configuration.".FormatInvariant(contractType, key, this.GetType().Name));

                case 1:
                    Trace.WriteLine("EXIT: Contract type '{0}' with key '{1}' resolved to implementation type '{2}'.".FormatInvariant(contractType, key, implementations[0]));

                    return implementations[0];

                default:
                    throw new ArgumentException("Multiple implementations of contract type '{0}' with key '{1}' were found in {2} configuration.".FormatInvariant(contractType, key, this.GetType().Name));
            }
        }

        internal IEnumerable<ServiceConfigurationElement> ResolveMultiple(Type contractType)
        {
            List<ServiceConfigurationElement> result;

            if (this.contractImplementations.TryGetValue(contractType, out result))
            {
                return result;
            }
            else
            {
                return Enumerable.Empty<ServiceConfigurationElement>();
            }
        }

        internal IEnumerable<ServiceConfigurationElement> ResolveMultiple(Type contractType, string key)
        {
            IEnumerable<ServiceConfigurationElement> result = this.ResolveMultiple(contractType);

            if (string.IsNullOrEmpty(key))
            {
                return result.Where(e => (e.ContractType == contractType) && string.IsNullOrEmpty(e.Key));
            }
            else
            {
                return result.Where(e => (e.ContractType == contractType) && (e.Key == key));
            }
        }

        internal void AddConfigSources(SourcesConfigurationCollection configSources)
        {
            foreach (SourceConfigurationElement configSource in configSources)
            {
                ServiceLocatorConfiguration configuration = configSource.LoadConfigSource();

                this.AddContractImplementations(configuration.Services.contractImplementations.Values.SelectMany(v => v));
            }
        }

        internal void PostDeserializeInternal()
        {
            this.AddContractImplementations(this.OfType<ServiceConfigurationElement>());

            this.RemoveServices();

            foreach (ServiceConfigurationElement serviceConfig in this)
            {
                this.ValidateAndConvertValues(serviceConfig);
            }
        }

        internal bool CanResolve(Type contractType, string key)
        {
            List<ServiceConfigurationElement> implementations;

            if (!this.contractImplementations.TryGetValue(contractType, out implementations))
            {
                return false;
            }

            return implementations.Any(i => i.Key == key);
        }

        /// <inheritdoc />
        protected override ConfigurationElement CreateNewElement()
        {
            return new ServiceConfigurationElement();
        }

        /// <inheritdoc />
        protected override object GetElementKey(ConfigurationElement element)
        {
            if (element is ServiceConfigurationElement)
            {
                ServiceConfigurationElement service = (ServiceConfigurationElement)element;

                return string.Join("|", service.ContractType, service.ImplementationType, service.Key);
            }

            throw new NotSupportedException();
        }

        protected override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
        {
            switch (elementName)
            {
                case "add":
                    {
                        ServiceConfigurationElement service = new ServiceConfigurationElement();

                        service.DeserializeElement(reader);

                        this.BaseAdd(service);

                        return true;
                    }

                case "remove":
                    {
                        RemoveServiceConfigurationElement service = new RemoveServiceConfigurationElement();

                        service.DeserializeElement(reader);

                        this.servicesToRemove.Add(service);

                        return true;
                    }

                default:
                    return false;
            }
        }

        private void AddContractImplementations(IEnumerable<ServiceConfigurationElement> services)
        {
            foreach (ServiceConfigurationElement service in services)
            {
                List<ServiceConfigurationElement> implementations;

                if (this.contractImplementations.TryGetValue(service.ContractType, out implementations))
                {
                    if (implementations.Any(s => !string.IsNullOrEmpty(s.Key) && (s.Key == service.Key)))
                    {
                        throw new ConfigurationErrorsException(
                            "A service with key '{0}' is already defined.".FormatInvariant(service.Key),
                            service.ElementInformation.Source,
                            service.ElementInformation.LineNumber);
                    }
                }
                else
                {
                    implementations = new List<ServiceConfigurationElement>();

                    this.contractImplementations.Add(service.ContractType, implementations);
                }

                implementations.Add(service);
            }
        }

        private void RemoveServices()
        {
            foreach (RemoveServiceConfigurationElement service in this.servicesToRemove)
            {
                if (string.IsNullOrEmpty(service.Key))
                {
                    this.contractImplementations.Remove(service.ContractType);
                }
                else
                {
                    List<ServiceConfigurationElement> implementations;

                    if (this.contractImplementations.TryGetValue(service.ContractType, out implementations))
                    {
                        implementations.Remove(s => s.Key == service.Key);
                    }
                }
            }
        }

        private bool CanResolveSingle(Type contractType)
        {
            List<ServiceConfigurationElement> implementations;

            if (!this.contractImplementations.TryGetValue(contractType, out implementations))
            {
                return false;
            }

            return implementations.Count(e => string.IsNullOrEmpty(e.Key)) == 1;
        }

        private void ValidateCompositionConfiguration(ServiceConfigurationElement serviceConfig)
        {
            serviceConfig.ValidateCompositionConfiguration();

            switch (serviceConfig.ImplementationComposition.Mode)
            {
                case CompositionMode.None:
                case CompositionMode.Empty:
                case CompositionMode.All:
                    if ((serviceConfig.ImplementationComposition.ResolveKeys != null) && (serviceConfig.ImplementationComposition.ResolveKeys.Count > 0))
                    {
                        throw new ConfigurationErrorsException(
                            "Implementation type '{0}' composition keys must be empty when mode is {1}.".FormatInvariant(serviceConfig.ImplementationType, serviceConfig.ImplementationComposition.Mode),
                            serviceConfig.ImplementationComposition.ElementInformation.Source,
                            serviceConfig.ImplementationComposition.ElementInformation.LineNumber);
                    }

                    break;

                case CompositionMode.Explicit:
                    if (serviceConfig.ImplementationComposition.ResolveKeys != null)
                    {
                        foreach (string resolveKey in serviceConfig.ImplementationComposition.ResolveKeys)
                        {
                            if (!this.CanResolve(serviceConfig.ContractType, resolveKey))
                            {
                                throw new ConfigurationErrorsException(
                                    "Implementation type '{0}' composition key '{0}' cannot be resolved.".FormatInvariant(serviceConfig.ImplementationType, resolveKey),
                                    serviceConfig.ImplementationComposition.ElementInformation.Source,
                                    serviceConfig.ImplementationComposition.ElementInformation.LineNumber);
                            }
                        }
                    }

                    break;

                default:
                    throw new NotSupportedException<CompositionMode>(serviceConfig.ImplementationComposition.Mode);
            }
        }

        private void ValidateCollectionPropertyConfiguration(ServiceConfigurationElement serviceConfig)
        {
            foreach (CollectionPropertyConfigurationElement propertyConfig in serviceConfig.ImplementationProperties
                .OfType<CollectionPropertyConfigurationElement>()
                .Where(e => e.ResolveType == CollectionPropertyConfigurationElement.CollectionResolveType.Dependency))
            {
                propertyConfig.Validate(serviceConfig.ImplementationType);

                if (propertyConfig.ResolveKeys != null)
                {
                    foreach (string resolveKey in propertyConfig.ResolveKeys)
                    {
                        if (!this.CanResolve(propertyConfig.CollectionElementType, resolveKey))
                        {
                            throw new ConfigurationErrorsException(
                                "Implementation type '{0}' property '{1}' resolve key '{2}' cannot be resolved.".FormatInvariant(serviceConfig.ImplementationType, propertyConfig.PropertyName, resolveKey),
                                propertyConfig.ElementInformation.Source,
                                propertyConfig.ElementInformation.LineNumber);
                        }
                    }
                }
            }
        }

        private void ValidateAndConvertValues(ServiceConfigurationElement serviceConfig)
        {
            this.ValidateCompositionConfiguration(serviceConfig);
            this.ValidateCollectionPropertyConfiguration(serviceConfig);

            if (serviceConfig.IsSelfRegistration)
            {
                return;
            }

            ConstructorInfo[] constructors = serviceConfig.ImplementationType.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
                .Where(constructor => this.MatchConstructor(serviceConfig.ImplementationConstructorParameters, constructor))
                .ToArray();

            switch (constructors.Length)
            {
                case 0:
                    throw new ConfigurationErrorsException(
                        "Matching implementation type '{0}' public constructor was not found.".FormatInvariant(serviceConfig.ImplementationType),
                        serviceConfig.ElementInformation.Source,
                        serviceConfig.ElementInformation.LineNumber);

                case 1:
                    serviceConfig.ImplementationConstructorParameters.ResolvedConstructorInfo = constructors[0];
                    break;

                default:
                    throw new ConfigurationErrorsException(
                        "More than one matching public constructors of the implementation type '{0}' found. Please specify constructor parameters more explicitly.".FormatInvariant(serviceConfig.ImplementationType),
                        serviceConfig.ElementInformation.Source,
                        serviceConfig.ElementInformation.LineNumber);
            }

            Dictionary<string, Type> constructorParameters = serviceConfig.ImplementationConstructorParameters.ResolvedConstructorInfo.GetParameters().ToDictionary(p => p.Name, p => p.ParameterType);

            // Recalculate constructor parameter values because they are filled in MatchConstructor method.
            serviceConfig.ImplementationConstructorParameters
                .OfType<ConstructorValueConfigurationElement>()
                .ForEach(false, e => e.ConvertValue(constructorParameters[e.ParameterName]));
        }

        private bool MatchConstructor(ConstructorConfigurationCollection configuredConstructorParameters, ConstructorInfo constructorCandidate)
        {
            Dictionary<string, Type> constructorParameters = constructorCandidate.GetParameters().ToDictionary(p => p.Name, p => p.ParameterType);

            foreach (ConstructorConfigurationElementBase configParameter in configuredConstructorParameters)
            {
                Type parameterType;

                if (!constructorParameters.TryGetValue(configParameter.ParameterName, out parameterType))
                {
                    return false; // Constructor parameter is defined in configuration that is not present in this ConstructorInfo.
                }

                if (configParameter is ConstructorResolveConfigurationElement)
                {
                    if (!this.CanResolve(parameterType, ((ConstructorResolveConfigurationElement)configParameter).ResolveKey))
                    {
                        return false;
                    }
                }
                else if (configParameter is ConstructorValueConfigurationElement)
                {
                    try
                    {
                        ((ConstructorValueConfigurationElement)configParameter).ConvertValue(parameterType);
                    }
                    catch (ConfigurationErrorsException)
                    {
                        return false;
                    }
                }

                constructorParameters.Remove(configParameter.ParameterName);
            }

            return constructorParameters.IsEmpty() || constructorParameters.All(p => this.CanResolveSingle(p.Value));
        }
    }
}