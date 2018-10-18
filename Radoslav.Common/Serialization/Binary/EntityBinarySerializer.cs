namespace Radoslav.Serialization
{
    /// <summary>
    /// An implementation of <see cref="IEntitySerializer{TEntity, TSerializationData}" /> that uses the built-in .NET binary serialization.
    /// </summary>
    public sealed class EntityBinarySerializer : EntityBinarySerializer<object>, IEntityBinarySerializer
    {
    }
}