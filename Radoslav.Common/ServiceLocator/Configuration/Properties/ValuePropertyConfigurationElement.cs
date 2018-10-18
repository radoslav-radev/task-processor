using System;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using Radoslav.Configuration.Validators;

namespace Radoslav.ServiceLocator.Configuration
{
    internal sealed class ValuePropertyConfigurationElement : PropertyConfigurationElementBase, IValueConfigurationElement
    {
        [ConfigurationProperty("value", IsRequired = true)]
        public string ValueAsString
        {
            get { return (string)base["value"]; }
        }

        [ConfigurationProperty("converter")]
        [TypeConverter(typeof(TypeNameConverter))]
        [AssignableFromTypeValidator(typeof(TypeConverter))]
        public Type ConverterType
        {
            get { return (Type)base["converter"]; }
        }

        public object ConvertedValue { get; set; }

        internal void ConvertValue(Type implementationType)
        {
            this.FillPropertyChain(implementationType, false);

            ConfigurationExtensions.ConvertValue(this, PropertyChain.Last().PropertyType);
        }
    }
}