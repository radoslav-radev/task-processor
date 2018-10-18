using System.Configuration;

namespace Radoslav.ServiceLocator.Configuration
{
    internal sealed class ConstructorResolveConfigurationElement : ConstructorConfigurationElementBase
    {
        [ConfigurationProperty("resolveKey", IsRequired = true)]
        internal string ResolveKey
        {
            get { return (string)base["resolveKey"]; }
        }
    }
}