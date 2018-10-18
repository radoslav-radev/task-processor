namespace Radoslav.Serialization
{
    /// <summary>
    /// An implementation of <see cref="IEntitySerializer{TEntity, TSerializationData}"/>
    /// that uses the built-in .NET XML serialization and serializes to string.
    /// </summary>
    public sealed class EntityXmlSerializer : EntityXmlSerializer<object>, IEntityStringSerializer
    {
    }
}