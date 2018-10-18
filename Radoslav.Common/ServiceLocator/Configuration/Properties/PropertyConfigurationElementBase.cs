using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace Radoslav.ServiceLocator.Configuration
{
    internal abstract class PropertyConfigurationElementBase : ConfigurationElement
    {
        protected readonly List<PropertyInfo> PropertyChain = new List<PropertyInfo>();

        [ConfigurationProperty("property", IsRequired = true, IsKey = true)]
        internal string PropertyName
        {
            get { return (string)base["property"]; }
        }

        internal void DeserializeElementInternal(XmlReader reader)
        {
            this.DeserializeElement(reader, false);
        }

        internal virtual void SetPropertyValue(object instance, object value)
        {
            Trace.WriteLine("ENTER: Setting property '{0}' value '{1}' to '{2}' ...".FormatInvariant(this.PropertyName, value, instance));

            for (int i = 0; i < this.PropertyChain.Count - 1; i++)
            {
                instance = this.PropertyChain[i].GetValue(instance);
            }

            try
            {
                this.PropertyChain.Last().SetValue(instance, value);

                Trace.WriteLine("EXIT: Property '{0}' value '{1}' set to '{2}'.".FormatInvariant(this.PropertyName, value, instance));
            }
            catch (Exception ex)
            {
                throw new ConfigurationErrorsException(
                    "Failed to set property '{0}' value '{1}' to '{2}'.".FormatInvariant(this.PropertyName, value, instance),
                    ex,
                    this.ElementInformation.Source,
                    this.ElementInformation.LineNumber);
            }
        }

        protected void FillPropertyChain(Type implementationType, bool allowReadOnly)
        {
            IEnumerable<PropertyInfo> propertyChain = this.GetPropertyChain(implementationType, allowReadOnly);

            this.PropertyChain.AddRange(propertyChain);
        }

        private IEnumerable<PropertyInfo> GetPropertyChain(Type implementationType, bool allowReadOnly)
        {
            string[] propertyNames = this.PropertyName.Split('.');

            for (int i = 0; i < propertyNames.Length; i++)
            {
                PropertyInfo property = implementationType.GetProperty(propertyNames[i]);

                if (property == null)
                {
                    throw new ConfigurationErrorsException(
                        "Property '{0}' was not found in type '{1}'.".FormatInvariant(propertyNames[i], implementationType),
                        this.ElementInformation.Source,
                        this.ElementInformation.LineNumber);
                }

                // All properties but last must be readable.
                if (i < propertyNames.Length - 1)
                {
                    if (!property.CanRead)
                    {
                        throw new ConfigurationErrorsException(
                            "Property '{0}' in type '{1}' is write-only.".FormatInvariant(property.Name, implementationType),
                            this.ElementInformation.Source,
                            this.ElementInformation.LineNumber);
                    }
                }
                else
                {
                    // Last property must be writable.
                    if (!allowReadOnly && !property.CanWrite)
                    {
                        throw new ConfigurationErrorsException(
                            "Property '{0}' in type '{1}' is read-only.".FormatInvariant(property.Name, implementationType),
                            this.ElementInformation.Source,
                            this.ElementInformation.LineNumber);
                    }
                }

                yield return property;

                implementationType = property.PropertyType;
            }
        }
    }
}