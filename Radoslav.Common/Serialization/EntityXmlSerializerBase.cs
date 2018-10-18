using System;

namespace Radoslav.Serialization
{
    /// <summary>
    /// An implementation of <see cref="IEntitySerializer{TEntity, TSerializationData}"/> that uses the built-in .NET XML serialization.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TSerializationData">The type of the serialization data, usually a byte array or a string.</typeparam>
    public abstract class EntityXmlSerializerBase<TEntity, TSerializationData> : IEntitySerializer<TEntity, TSerializationData>
    {
        #region IEntitySerializer<TEntity, TSerializationData> Members

        /// <inheritdoc />
        public bool CanDetermineEntityTypeFromContent
        {
            get { return false; }
        }

        /// <inheritdoc />
        public abstract TSerializationData Serialize(TEntity entity);

        /// <inheritdoc />
        TEntity IEntitySerializer<TEntity, TSerializationData>.Deserialize(TSerializationData content)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public abstract TEntity Deserialize(TSerializationData content, Type entityType);

        #endregion IEntitySerializer<TEntity, TSerializationData> Members
    }
}