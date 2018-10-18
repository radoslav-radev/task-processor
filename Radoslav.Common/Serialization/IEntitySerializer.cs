using System;

namespace Radoslav.Serialization
{
    /// <summary>
    /// A basic functionality of a serializer that can serialize and deserialize an entity to a specific format.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TSerializationData">The type of the serialization data, usually a byte array or a string.</typeparam>
    public interface IEntitySerializer<TEntity, TSerializationData>
    {
        /// <summary>
        /// Gets a value indicating whether the serializer can determine the entity type from the content.
        /// </summary>
        /// <remarks>
        /// <para>For example, with binary serialization the value is <c>true</c>, but with XML serialization the value is <c>false</c>.</para>
        /// <para>This property is used for optimization purposes.</para>
        /// <para>When value is <c>true</c>, you can use both <see cref="Deserialize(TSerializationData)"/> and <see cref="Deserialize(TSerializationData, Type)"/> methods,
        /// but it is better to use <see cref="Deserialize(TSerializationData)"/> method. </para>
        /// <para>If value is <c>false</c>, you should use <see cref="Deserialize(TSerializationData, Type)"/> method because
        /// <see cref="Deserialize(TSerializationData)"/> method should throw <see cref="NotSupportedException"/>.</para>
        /// </remarks>
        /// <value>Whether the serializer can determine the entity type from the content.</value>
        bool CanDetermineEntityTypeFromContent { get; }

        /// <summary>
        /// Serializes an entity to serialization data.
        /// </summary>
        /// <param name="entity">The entity to be serialized.</param>
        /// <returns>The serialized data for the entity.</returns>
        TSerializationData Serialize(TEntity entity);

        /// <summary>
        /// Deserializes an entity from serialization data.
        /// </summary>
        /// <param name="content">The serialized data for the object.</param>
        /// <returns>The object deserialized from the serialization data.</returns>
        /// <exception cref="NotSupportedException">The serializer cannot deserialize the object because its type is unknown.
        /// This is the normal behavior if <see cref="CanDetermineEntityTypeFromContent"/> is <c>false</c>.</exception>
        TEntity Deserialize(TSerializationData content);

        /// <summary>
        /// Deserializes an entity from serialization data.
        /// </summary>
        /// <remarks>
        /// If parameter <paramref name="entityType"/> is null, method <see cref="Deserialize(TSerializationData)"/> is called.
        /// </remarks>
        /// <param name="content">The serialized data for the entity.</param>
        /// <param name="entityType">The type of the object to deserialize.</param>
        /// <returns>The object deserialized from the serialization data.</returns>
        TEntity Deserialize(TSerializationData content, Type entityType);
    }
}