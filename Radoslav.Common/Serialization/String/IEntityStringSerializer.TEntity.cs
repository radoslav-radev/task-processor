namespace Radoslav.Serialization
{
    /// <summary>
    /// A basic functionality of a serializer that can serialize and deserialize an entity to a string.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface IEntityStringSerializer<TEntity> : IEntitySerializer<TEntity, string>
    {
    }
}