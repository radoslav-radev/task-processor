using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Radoslav.Serialization
{
    /// <summary>
    /// An implementation of <see cref="IEntitySerializer{TEntity, TSerializationData}" /> that uses the built-in .NET binary serialization.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public class EntityBinarySerializer<TEntity> : IEntityBinarySerializer<TEntity>
    {
        #region IEntitySerializer<TEntity, byte[]> Members

        /// <inheritdoc />
        public bool CanDetermineEntityTypeFromContent
        {
            get { return true; }
        }

        /// <inheritdoc />
        public byte[] Serialize(TEntity entity)
        {
            if (entity == null)
            {
                return new byte[0];
            }

            BinaryFormatter formatter = new BinaryFormatter();

            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, entity);

                return stream.ToArray();
            }
        }

        /// <inheritdoc />
        public TEntity Deserialize(byte[] content)
        {
            if ((content == null) || (content.Length == 0))
            {
                return default(TEntity);
            }

            BinaryFormatter formatter = new BinaryFormatter();

            using (MemoryStream stream = new MemoryStream(content))
            {
                return (TEntity)formatter.Deserialize(stream);
            }
        }

        /// <inheritdoc />
        TEntity IEntitySerializer<TEntity, byte[]>.Deserialize(byte[] content, Type entityType)
        {
            return this.Deserialize(content);
        }

        #endregion IEntitySerializer<TEntity, byte[]> Members
    }
}