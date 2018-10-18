using System;
using System.Collections.Generic;
using Radoslav.Serialization;
using Radoslav.ServiceLocator;
using Radoslav.TaskProcessor.Configuration;

namespace Radoslav.TaskProcessor.Serialization
{
    /// <summary>
    /// An implementation of <see cref="IEntityBinarySerializer"/> that uses configuration to serialize tasks.
    /// </summary>
    public sealed class ConfigurationBinarySerializer : IEntityBinarySerializer
    {
        private readonly ITaskProcessorSerializationConfiguration configuration;
        private readonly Lazy<IEntityBinarySerializer> defaultSerializer;
        private readonly IRadoslavServiceLocator locator;
        private readonly Dictionary<Type, IEntityBinarySerializer> serializersByEntityType = new Dictionary<Type, IEntityBinarySerializer>();

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationBinarySerializer"/> class.
        /// </summary>
        /// <param name="configProvider">The configuration provider which should provide the configuration to be used by the configuration serializer.</param>
        /// <param name="locator">The service locator to use when resolving services.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="configProvider"/> or <paramref name="locator"/> is null.</exception>
        public ConfigurationBinarySerializer(ITaskProcessorConfigurationProvider configProvider, IRadoslavServiceLocator locator)
        {
            if (configProvider == null)
            {
                throw new ArgumentNullException(nameof(configProvider));
            }

            if (locator == null)
            {
                throw new ArgumentNullException(nameof(locator));
            }

            this.configuration = configProvider.GetSerializationConfiguration();

            if (this.configuration == null)
            {
                throw new ArgumentException("Configuration provider returned null configuration.".FormatInvariant(), nameof(configProvider));
            }

            this.defaultSerializer = new Lazy<IEntityBinarySerializer>(() => this.GetSerializer(typeof(object), false));

            this.locator = locator;
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Gets the service locator used to resolve entity serializers.
        /// </summary>
        /// <value>The service locator used to resolve entity serializers.</value>
        public IRadoslavServiceLocator ServiceLocator
        {
            get { return this.locator; }
        }

        #endregion Properties

        #region ITaskSerializer Members

        /// <inheritdoc />
        public bool CanDetermineEntityTypeFromContent
        {
            get { return false; }
        }

        /// <inheritdoc />
        public byte[] Serialize(object entity)
        {
            if (entity == null)
            {
                return new byte[0];
            }

            IEntityBinarySerializer serializer = this.GetSerializer(entity.GetType(), true);

            return serializer.Serialize(entity);
        }

        /// <inheritdoc />
        public object Deserialize(byte[] content)
        {
            if ((content == null) || (content.Length == 0))
            {
                return null;
            }

            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public object Deserialize(byte[] content, Type entityType)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException(nameof(entityType));
            }

            if ((content == null) || (content.Length == 0))
            {
                return null;
            }

            IEntityBinarySerializer serializer = this.GetSerializer(entityType, true);

            return serializer.Deserialize(content, entityType);
        }

        #endregion ITaskSerializer Members

        private IEntityBinarySerializer GetSerializer(Type entityType, bool returnDefaultOrThrowError)
        {
            IEntityBinarySerializer result;

            if (!this.serializersByEntityType.TryGetValue(entityType, out result))
            {
                if (this.configuration != null)
                {
                    Type serializerType = this.configuration.GetSerializerType(entityType);

                    if (serializerType != null)
                    {
                        if (this.locator.CanResolve(serializerType))
                        {
                            result = this.locator.ResolveSingle<IEntityBinarySerializer>(serializerType);
                        }
                        else
                        {
                            result = serializerType.CreateInstance<IEntityBinarySerializer>();
                        }
                    }
                }

                if (result == null)
                {
                    if (returnDefaultOrThrowError)
                    {
                        result = this.defaultSerializer.Value;
                    }
                    else
                    {
                        throw new InvalidOperationException("Default serializer not found in serialization configuration.".FormatInvariant());
                    }
                }

                this.serializersByEntityType.Add(entityType, result);
            }

            return result;
        }
    }
}