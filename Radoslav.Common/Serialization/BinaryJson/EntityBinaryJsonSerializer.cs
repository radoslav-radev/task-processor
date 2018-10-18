namespace Radoslav.Serialization
{
    /// <summary>
    /// An implementation of <see cref="IEntitySerializer{TEntity, TSerializationData}"/>
    /// that uses the built-in .NET JSON serialization and serializes the JSON to a byte array.
    /// </summary>
    public sealed class EntityBinaryJsonSerializer : EntityBinaryJsonSerializer<object>, IEntityBinarySerializer
    {
    }
}