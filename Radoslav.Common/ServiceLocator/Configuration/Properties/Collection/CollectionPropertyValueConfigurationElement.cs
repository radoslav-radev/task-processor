﻿using System;
using System.ComponentModel;
using System.Configuration;
using Radoslav.Configuration.Validators;

namespace Radoslav.ServiceLocator.Configuration
{
    internal sealed class CollectionPropertyValueConfigurationElement : ConfigurationElement, IValueConfigurationElement
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

        internal void ConvertValue(Type collectionElementType)
        {
            ConfigurationExtensions.ConvertValue(this, collectionElementType);
        }
    }
}