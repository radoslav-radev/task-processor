using System;
using System.IO;
using System.Runtime.Serialization.Json;

namespace Radoslav.Serialization
{
    /// <summary>
    /// An implementation of <see cref="IEntitySerializer{TEntity, TSerializationData}"/>
    /// that uses the built-in .NET JSON serialization and serializes the JSON to a byte array.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public class EntityBinaryJsonSerializer<TEntity> : IEntitySerializer<TEntity, byte[]>
    {
        #region IEntitySerializer<TEntity, byte[]> Members

        /// <inheritdoc />
        public bool CanDetermineEntityTypeFromContent
        {
            get { return false; }
        }

        /// <inheritdoc />
        public byte[] Serialize(TEntity entity)
        {
            if (entity == null)
            {
                return new byte[0];
            }

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(entity.GetType());

            using (MemoryStream stream = new MemoryStream())
            {
                serializer.WriteObject(stream, entity);

                return stream.ToArray();
            }
        }

        /// <inheritdoc />
        TEntity IEntitySerializer<TEntity, byte[]>.Deserialize(byte[] content)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public TEntity Deserialize(byte[] content, Type entityType)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException(nameof(entityType));
            }

            if ((content == null) || (content.Length == 0))
            {
                return default(TEntity);
            }

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(entityType);

            using (MemoryStream stream = new MemoryStream(content))
            {
                return (TEntity)serializer.ReadObject(stream);
            }
        }

        #endregion IEntitySerializer<TEntity, byte[]> Members
    }
}