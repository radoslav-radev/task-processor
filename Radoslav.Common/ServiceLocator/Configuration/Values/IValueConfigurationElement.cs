using System;

namespace Radoslav.ServiceLocator.Configuration
{
    internal interface IValueConfigurationElement
    {
        Type ConverterType { get; }

        string ValueAsString { get; }

        object ConvertedValue { get; set; }
    }
}