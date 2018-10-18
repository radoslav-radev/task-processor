using System;

namespace Radoslav.Serialization
{
    /// <summary>
    /// Class for serialization extensions methods.
    /// </summary>
    public static class SerializationExtensions
    {
        /// <summary>
        /// Deserializes an entity from serialization data.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TSerializationData">The type of the serialization data, usually a byte array or a string.</typeparam>
        /// <param name="serializer">The serializer which to deserialize the entity.</param>
        /// <param name="content">The serialized data for the entity.</param>
        /// <returns>The object deserialized from the byte array.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="serializer"/> is null.</exception>
        public static TEntity Deserialize<TEntity, TSerializationData>(this IEntitySerializer<TEntity, TSerializationData> serializer, TSerializationData content)
        {
            if (serializer == null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            return serializer.Deserialize(content, typeof(TEntity));
        }
    }
}