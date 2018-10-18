namespace Radoslav.Serialization
{
    /// <summary>
    /// A basic functionality of a serializer that can serialize and deserialize an object to a byte array.
    /// </summary>
    public interface IEntityBinarySerializer : IEntityBinarySerializer<object>
    {
    }
}