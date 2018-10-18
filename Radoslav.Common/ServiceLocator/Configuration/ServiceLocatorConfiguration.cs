using System;
using System.Configuration;

namespace Radoslav.ServiceLocator.Configuration
{
    /// <summary>
    /// A class to configure <see cref="RadoslavServiceLocator"/>.
    /// </summary>
    public sealed class ServiceLocatorConfiguration : ConfigurationSection
    {
        /// <summary>
        /// The section name in App.config configuration sections.
        /// </summary>
        internal const string AppSectionName = "Radoslav.ServiceLocator";

        static ServiceLocatorConfiguration()
        {
            Helpers.RegisterDefaultConverter<Type, TypeNameConverter>();
        }

        /// <summary>
        /// Gets the services defined for the <see cref="RadoslavServiceLocator"/>.
        /// </summary>
        /// <value>The services defined for the <see cref="RadoslavServiceLocator"/>.</value>
        [ConfigurationProperty("services")]
        internal ServicesConfigurationCollection Services
        {
            get
            {
                return (ServicesConfigurationCollection)this["services"];
            }
        }

        [ConfigurationProperty("sources")]
        internal SourcesConfigurationCollection Sources
        {
            get
            {
                return (SourcesConfigurationCollection)this["sources"];
            }
        }

        /// <inheritdoc />
        protected override void PostDeserialize()
        {
            base.PostDeserialize();

            this.Services.AddConfigSources(this.Sources);

            this.Services.PostDeserializeInternal();
        }
    }
}