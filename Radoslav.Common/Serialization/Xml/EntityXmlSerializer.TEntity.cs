using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Radoslav.Serialization
{
    /// <summary>
    /// An implementation of <see cref="IEntitySerializer{TEntity, TSerializationData}"/>
    /// that uses the built-in .NET XML serialization and serializes to string.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public class EntityXmlSerializer<TEntity> : EntityXmlSerializerBase<TEntity, string>, IEntityStringSerializer<TEntity>
    {
        /// <inheritdoc />
        public override string Serialize(TEntity entity)
        {
            if (entity == null)
            {
                return string.Empty;
            }

            XmlSerializer serializer = new XmlSerializer(entity.GetType());

            StringBuilder result = new StringBuilder();

            using (StringWriter writer = new StringWriter(result, CultureInfo.CurrentCulture))
            {
                serializer.Serialize(writer, entity);
            }

            return result.ToString();
        }

        /// <inheritdoc />
        public override TEntity Deserialize(string content, Type entityType)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException(nameof(entityType));
            }

            if (string.IsNullOrEmpty(content))
            {
                return default(TEntity);
            }

            XmlSerializer serializer = new XmlSerializer(entityType);

            using (StringReader reader = new StringReader(content))
            {
                return (TEntity)serializer.Deserialize(reader);
            }
        }
    }
}