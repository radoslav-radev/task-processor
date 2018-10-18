using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace Radoslav.ServiceLocator.Configuration
{
    internal sealed class CollectionPropertyConfigurationElement : PropertyConfigurationElementBase
    {
        internal enum CollectionResolveType
        {
            Dependency,
            Values
        }

        private enum PropertyCollectionType
        {
            None,
            GenericCollection
        }

        [ConfigurationProperty("resolveKeys")]
        [TypeConverter(typeof(CommaDelimitedStringCollectionConverter))]
        internal CommaDelimitedStringCollection ResolveKeys
        {
            get
            {
                return (CommaDelimitedStringCollection)base["resolveKeys"];
            }
        }

        [ConfigurationProperty("optionalKeys")]
        [TypeConverter(typeof(CommaDelimitedStringCollectionConverter))]
        internal CommaDelimitedStringCollection OptionalKeys
        {
            get
            {
                return (CommaDelimitedStringCollection)base["optionalKeys"];
            }
        }

        [ConfigurationProperty("values")]
        internal CollectionPropertyValuesConfigurationCollection Values
        {
            get
            {
                return (CollectionPropertyValuesConfigurationCollection)base["values"];
            }
        }

        internal Type CollectionElementType { get; private set; }

        internal CollectionResolveType ResolveType { get; private set; }

        private PropertyCollectionType CollectionType { get; set; }

        internal void Validate(Type implementationType)
        {
            this.FillPropertyChain(implementationType, true);

            PropertyInfo property = PropertyChain.Last();

            foreach (Type implementedInterface in new Type[] { property.PropertyType }.Concat(property.PropertyType.GetInterfaces()))
            {
                if (implementedInterface.IsGenericType && (implementedInterface.GetGenericTypeDefinition() == typeof(ICollection<>)))
                {
                    this.CollectionElementType = implementedInterface.GetGenericArguments()[0];

                    this.CollectionType = PropertyCollectionType.GenericCollection;

                    break;
                }
            }

            if (this.CollectionType == PropertyCollectionType.None)
            {
                throw new ConfigurationErrorsException(
                    "Implementation type '{0}' collection property '{1}' of type '{2}' is not supported.".FormatInvariant(implementationType, this.PropertyName, property.PropertyType),
                    this.ElementInformation.Source,
                    this.ElementInformation.LineNumber);
            }
        }

        internal void AddToCollection(object instance, IEnumerable items)
        {
            foreach (PropertyInfo property in this.PropertyChain)
            {
                instance = property.GetValue(instance);

                if (instance == null)
                {
                    throw new InvalidOperationException("Implementation type '{0}' collection property '{1}' is null.".FormatInvariant(instance.GetType(), this.PropertyName));
                }
            }

            MethodInfo addMethod;

            switch (this.CollectionType)
            {
                case PropertyCollectionType.GenericCollection:

                    addMethod = typeof(ICollection<>)
                        .MakeGenericType(this.CollectionElementType)
                        .GetMethod("Add");

                    break;

                default:
                    throw new NotSupportedException<PropertyCollectionType>(this.CollectionType);
            }

            foreach (object item in items)
            {
                addMethod.Invoke(instance, new object[] { item });
            }
        }

        internal void AddValuesToCollection(object instance)
        {
            IEnumerable values = this.Values
                .OfType<CollectionPropertyValueConfigurationElement>()
                .Select(e => e.ConvertedValue);

            this.AddToCollection(instance, values);
        }

        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "resolveKeys", Justification = "This is configuration property name.")]
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "optionalKeys", Justification = "This is configuration property name.")]
        protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
        {
            base.DeserializeElement(reader, serializeCollectionKey);

            if (this.Values.Count > 0)
            {
                if (this.ResolveKeys != null)
                {
                    throw new ConfigurationErrorsException("Cannot set both 'resolveKeys' and 'values'.", reader);
                }

                if (this.OptionalKeys != null)
                {
                    throw new ConfigurationErrorsException("Cannot set both 'optionalKeys' and 'values'.", reader);
                }

                this.ResolveType = CollectionResolveType.Values;
            }
            else
            {
                this.ResolveType = CollectionResolveType.Dependency;
            }
        }
    }
}