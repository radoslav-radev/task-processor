using System;
using System.Configuration;
using System.Linq;

namespace Radoslav.ServiceLocator.Configuration
{
    internal sealed class ResolvePropertyConfigurationElement : PropertyConfigurationElementBase
    {
        [ConfigurationProperty("resolveKey")]
        internal string ResolveKey
        {
            get { return (string)base["resolveKey"]; }
        }

        internal Type DependencyType { get; private set; }

        internal void ResolveDependencyType(Type implementationType)
        {
            this.FillPropertyChain(implementationType, false);

            this.DependencyType = this.PropertyChain.Last().PropertyType;
        }
    }
}