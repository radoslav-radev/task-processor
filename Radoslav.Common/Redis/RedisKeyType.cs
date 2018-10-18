namespace Radoslav.Redis
{
    /// <summary>
    /// An enumeration for the value typed associated with a Redis key.
    /// </summary>
    public enum RedisKeyType
    {
        /// <summary>
        /// No type, for example because the key does not exists.
        /// </summary>
        None,

        /// <summary>
        /// String type.
        /// </summary>
        String,

        /// <summary>
        /// List type.
        /// </summary>
        List,

        /// <summary>
        /// Unordered set type.
        /// </summary>
        Set,

        /// <summary>
        /// Sorted set type.
        /// </summary>
        SortedSet,

        /// <summary>
        /// Hash type.
        /// </summary>
        Hash
    }
}