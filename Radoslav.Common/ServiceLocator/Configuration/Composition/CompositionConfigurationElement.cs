using System.ComponentModel;
using System.Configuration;

namespace Radoslav.ServiceLocator.Configuration
{
    internal sealed partial class CompositionConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("mode")]
        internal CompositionMode Mode
        {
            get { return (CompositionMode)base["mode"]; }
        }

        [ConfigurationProperty("resolveKeys")]
        [TypeConverter(typeof(CommaDelimitedStringCollectionConverter))]
        internal CommaDelimitedStringCollection ResolveKeys
        {
            get { return (CommaDelimitedStringCollection)base["resolveKeys"]; }
        }
    }
}