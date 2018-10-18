namespace Radoslav.Serialization
{
    /// <summary>
    /// A basic functionality of a serializer that can serialize and deserialize an object to a string.
    /// </summary>
    public interface IEntityStringSerializer : IEntityStringSerializer<object>
    {
    }
}