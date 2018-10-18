using System;
using System.ComponentModel;
using System.Configuration;

namespace Radoslav.ServiceLocator.Configuration
{
    internal static class ConfigurationExtensions
    {
        internal static void ConvertValue<TConfigurationElement>(this TConfigurationElement element, Type valueType)
            where TConfigurationElement : ConfigurationElement, IValueConfigurationElement
        {
            TypeConverter converter;

            if (element.ConverterType == null)
            {
                converter = Helpers.GetDefaultTypeConverter(valueType);
            }
            else
            {
                converter = (TypeConverter)Activator.CreateInstance(element.ConverterType);
            }

            if (!converter.CanConvertFrom(element.ValueAsString.GetType()))
            {
                throw new ConfigurationErrorsException(
                    "'{0}' cannot convert from '{1}'.".FormatInvariant(converter, element.ValueAsString.GetType()),
                    element.ElementInformation.Source,
                    element.ElementInformation.LineNumber);
            }

            if (!converter.IsValid(element.ValueAsString))
            {
                throw new ConfigurationErrorsException(
                    "Value '{0}' is not valid for converter '{1}'.".FormatInvariant(element.ValueAsString, converter.GetType()),
                    element.ElementInformation.Source,
                    element.ElementInformation.LineNumber);
            }

            try
            {
                element.ConvertedValue = converter.ConvertFromInvariantString(element.ValueAsString);
            }
            catch (Exception ex)
            {
                throw new ConfigurationErrorsException(
                    "'{0}' failed to convert '{1}' to '{2}'.".FormatInvariant(converter.GetType(), element.ValueAsString, valueType),
                    ex,
                     element.ElementInformation.Source,
                     element.ElementInformation.LineNumber);
            }
        }
    }
}