using System;
using System.IO;
using System.Xml.Serialization;

namespace Radoslav.Serialization
{
    /// <summary>
    /// An implementation of <see cref="IEntitySerializer{TEntity, TSerializationData}"/>
    /// that uses the built-in .NET XML serialization and serializes the XML to a byte array.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public class EntityBinaryXmlSerializer<TEntity> : EntityXmlSerializerBase<TEntity, byte[]>, IEntityBinarySerializer<TEntity>
    {
        /// <inheritdoc />
        public override byte[] Serialize(TEntity entity)
        {
            if (entity == null)
            {
                return new byte[0];
            }

            XmlSerializer serializer = new XmlSerializer(entity.GetType());

            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Serialize(stream, entity);

                return stream.ToArray();
            }
        }

        /// <inheritdoc />
        public override TEntity Deserialize(byte[] content, Type entityType)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException(nameof(entityType));
            }

            if ((content == null) || (content.Length == 0))
            {
                return default(TEntity);
            }

            XmlSerializer serializer = new XmlSerializer(entityType);

            using (MemoryStream stream = new MemoryStream(content))
            {
                return (TEntity)serializer.Deserialize(stream);
            }
        }
    }
}