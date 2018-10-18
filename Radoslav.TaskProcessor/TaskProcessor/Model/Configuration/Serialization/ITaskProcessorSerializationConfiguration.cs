using System;

namespace Radoslav.TaskProcessor.Configuration
{
    /// <summary>
    /// Basic functionality for a serialization configuration.
    /// </summary>
    public interface ITaskProcessorSerializationConfiguration
    {
        /// <summary>
        /// Gets the type of the serializer to be used for serialization of entities of specified type.
        /// </summary>
        /// <param name="entityType">The type of the entities for which to return serializer type.</param>
        /// <returns>The type of the serializer to be used for serialization of entities of the specified type, or null if not found.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="entityType"/> is null.</exception>
        Type GetSerializerType(Type entityType);
    }
}