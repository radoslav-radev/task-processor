using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Xml;
using Radoslav.Configuration.Validators;

namespace Radoslav.ServiceLocator.Configuration
{
    internal sealed class ServiceConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("key")]
        internal string Key
        {
            get { return (string)base["key"]; }
        }

        [ConfigurationProperty("contract", IsRequired = true)]
        [TypeConverter(typeof(TypeNameConverter))]
        internal Type ContractType
        {
            get { return (Type)base["contract"]; }
        }

        [ConfigurationProperty("implementation", IsRequired = true)]
        [TypeConverter(typeof(TypeNameConverter))]
        [CompositeValidator(typeof(NotAbstractClassValidator), typeof(PublicConstructorValidator))]
        internal Type ImplementationType
        {
            get { return (Type)base["implementation"]; }
        }

        [ConfigurationProperty("shared")]
        internal bool IsShared
        {
            get { return (bool)base["shared"]; }
        }

        [ConfigurationProperty("constructor")]
        internal ConstructorConfigurationCollection ImplementationConstructorParameters
        {
            get
            {
                return (ConstructorConfigurationCollection)base["constructor"];
            }
        }

        [ConfigurationProperty("properties")]
        internal PropertiesConfigurationCollection ImplementationProperties
        {
            get
            {
                return (PropertiesConfigurationCollection)base["properties"];
            }
        }

        [ConfigurationProperty("composition")]
        internal CompositionConfigurationElement ImplementationComposition
        {
            get
            {
                return (CompositionConfigurationElement)base["composition"];
            }
        }

        internal bool IsSelfRegistration
        {
            get
            {
                return (this.ContractType == typeof(IRadoslavServiceLocator)) && (this.ImplementationType == typeof(RadoslavServiceLocator));
            }
        }

        internal void DeserializeElement(XmlReader reader)
        {
            this.DeserializeElement(reader, false);
        }

        internal void ValidateCompositionConfiguration()
        {
            if (this.ImplementationComposition.Mode == CompositionMode.None)
            {
                return;
            }

            Type collectionInterface = typeof(ICollection<>).MakeGenericType(this.ContractType);

            if (!this.ImplementationType.GetInterfaces().Contains(collectionInterface))
            {
                throw new ConfigurationErrorsException(
                    "Implementation type '{0}' has composition mode '{1}' but is not a collection.".FormatInvariant(this.ImplementationType, this.ImplementationComposition.Mode),
                    this.ElementInformation.Source,
                    this.ElementInformation.LineNumber);
            }
        }

        /// <inheritdoc />
        protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
        {
            base.DeserializeElement(reader, serializeCollectionKey);

            AssignableFromTypeValidator validator = new AssignableFromTypeValidator(this.ContractType);

            try
            {
                validator.Validate(this.ImplementationType);
            }
            catch (ConfigurationErrorsException ex)
            {
                throw new ConfigurationErrorsException(ex.Message, ex, reader);
            }

            if (this.IsSelfRegistration && !this.IsShared)
            {
                throw new ConfigurationErrorsException("'{0}' must be registered as shared.".FormatInvariant(typeof(RadoslavServiceLocator)), reader);
            }
        }

        /// <inheritdoc />
        protected override void PostDeserialize()
        {
            base.PostDeserialize();

            foreach (PropertyConfigurationElementBase propertyConfig in this.ImplementationProperties)
            {
                ValuePropertyConfigurationElement valueConfig = propertyConfig as ValuePropertyConfigurationElement;

                if (valueConfig != null)
                {
                    valueConfig.ConvertValue(this.ImplementationType);
                }

                ResolvePropertyConfigurationElement dependencyConfig = propertyConfig as ResolvePropertyConfigurationElement;

                if (dependencyConfig != null)
                {
                    dependencyConfig.ResolveDependencyType(this.ImplementationType);
                }

                CollectionPropertyConfigurationElement collectionConfig = propertyConfig as CollectionPropertyConfigurationElement;

                if ((collectionConfig != null) && (collectionConfig.ResolveType == CollectionPropertyConfigurationElement.CollectionResolveType.Values))
                {
                    collectionConfig.Validate(this.ImplementationType);

                    foreach (CollectionPropertyValueConfigurationElement config in collectionConfig.Values)
                    {
                        config.ConvertValue(collectionConfig.CollectionElementType);
                    }
                }
            }
        }
    }
}