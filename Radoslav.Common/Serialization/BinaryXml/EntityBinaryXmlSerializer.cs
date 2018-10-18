namespace Radoslav.Serialization
{
    /// <summary>
    /// An implementation of <see cref="IEntitySerializer{TEntity, TSerializationData}"/>
    /// that uses the built-in .NET XML serialization and serializes the XML to a byte array.
    /// </summary>
    public sealed class EntityBinaryXmlSerializer : EntityBinaryXmlSerializer<object>, IEntityBinarySerializer
    {
    }
}