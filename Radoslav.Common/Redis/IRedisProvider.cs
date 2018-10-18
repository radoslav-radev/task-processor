using System;
using System.Collections.Generic;

namespace Radoslav.Redis
{
    /// <summary>
    /// Basic functionality of a Redis provider.
    /// </summary>
    public interface IRedisProvider : IDisposable
    {
        #region Time

        /// <summary>
        /// Gets the current date and time from Redis, in UTC.
        /// </summary>
        /// <returns>The current date and time from Redis in UTC.</returns>
        DateTime GetServerDateTimeUtc();

        #endregion Time

        #region Keys

        /// <summary>
        /// Returns all keys matching pattern.
        /// </summary>
        /// <param name="pattern">The pattern to match.</param>
        /// <returns>All keys matching the specified pattern.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="pattern"/> is null or empty string.</exception>
        /// <exception cref="ObjectDisposedException">Redis provider has been disposed.</exception>
        IEnumerable<string> SearchKeys(string pattern);

        /// <summary>
        /// Checks if a key exists in Redis.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the specified key exists in Redis; otherwise false.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string.</exception>
        /// <exception cref="ObjectDisposedException">Redis provider has been disposed.</exception>
        bool ContainsKey(string key);

        /// <summary>
        /// Determines the type stored by key in Redis.
        /// </summary>
        /// <param name="key">The key whose type should be determined.</param>
        /// <remarks>If the key does not exist in Redis, <see cref="RedisKeyType.None"/> is returned.</remarks>
        /// <returns>The type stored by specified key in Redis, or <see cref="RedisKeyType.None"/> is key does not exists.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string.</exception>
        /// <exception cref="ObjectDisposedException">Redis provider has been disposed.</exception>
        RedisKeyType GetKeyType(string key);

        /// <summary>
        /// Gets the string value of a specified key.
        /// </summary>
        /// <param name="key">The key which value to retrieve from Redis.</param>
        /// <returns>The string value associated with the specified key, or null or empty string if the key does not exists.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string.</exception>
        /// <exception cref="ObjectDisposedException">Redis provider has been disposed.</exception>
        /// <exception cref="RedisException">The key type is not string.</exception>
        string GetTextValue(string key);

        /// <summary>
        /// Gets a binary value for a specified key.
        /// </summary>
        /// <param name="key">The key which value to retrieve from Redis.</param>
        /// <returns>The binary value associated with the specified key, or null if the key does not exists.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string.</exception>
        /// <exception cref="ObjectDisposedException">Redis provider has been disposed.</exception>
        /// <exception cref="RedisException">The key type is not string.</exception>
        byte[] GetBinaryValue(string key);

        /// <summary>
        /// Set key to hold the string value.
        /// </summary>
        /// <param name="key">The key whose value should be set.</param>
        /// <param name="value">The value to be associated with the specified key.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string.</exception>
        /// <exception cref="ObjectDisposedException">Redis provider has been disposed.</exception>
        void SetValue(string key, string value);

        /// <summary>
        /// Set key to hold the binary value.
        /// </summary>
        /// <param name="key">The key whose value should be set.</param>
        /// <param name="value">The value to be associated with the specified key.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string, or <paramref name="value"/> is null..</exception>
        /// <exception cref="ObjectDisposedException">Redis provider has been disposed.</exception>
        void SetValue(string key, byte[] value);

        /// <summary>
        /// Set key to hold string value if key does not exist. When key already holds a value, no operation is performed.
        /// </summary>
        /// <param name="key">The key whose value should be set.</param>
        /// <param name="value">The value to be associated with the specified key.</param>
        /// <returns>True if the key did not exists and the value was set; otherwise false.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string.</exception>
        /// <exception cref="ObjectDisposedException">Redis provider has been disposed.</exception>
        bool SetValueIfNotExists(string key, string value);

        /// <summary>
        /// Sets a timeout on a key. After the timeout has expired, the key will automatically be deleted from Redis.
        /// </summary>
        /// <param name="key">The key to set a timeout.</param>
        /// <param name="timeout">The timeout to be set on the specified key.</param>
        /// <returns>True if timeout was set successfully; false if the key does not exists or the timeout could not be set.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Parameter <paramref name="timeout"/> is less or equal to <see cref="TimeSpan.Zero"/>.</exception>
        /// <exception cref="ObjectDisposedException">Redis provider has been disposed.</exception>
        bool ExpireKeyIn(string key, TimeSpan timeout);

        /// <summary>
        /// Increments the number stored at key by one.
        /// </summary>
        /// <remarks>If the key does not exist, it is set to 0 before performing the operation.
        /// An error is returned if the key contains a value of the wrong type or contains a string that can not be represented as integer.</remarks>
        /// <param name="key">The key whose value should be incremented.</param>
        /// <returns>The value of key after the increment.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string.</exception>
        /// <exception cref="ObjectDisposedException">Redis provider has been disposed.</exception>
        /// <exception cref="RedisException">The key type is not string or value cannot be parsed to a number.</exception>
        long IncrementValue(string key);

        /// <summary>
        /// Removes the specified key from Redis. If the key does not exists, it is ignored.
        /// </summary>
        /// <param name="key">The key to remove.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string.</exception>
        /// <exception cref="ObjectDisposedException">Redis provider has been disposed.</exception>
        void RemoveKey(string key);

        #endregion Keys

        #region Pub/Sub

        /// <summary>
        /// Create an instance of <see cref="IRedisMessageSubscription"/>.
        /// </summary>
        /// <returns>An instance of <see cref="IRedisMessageSubscription"/>.</returns>
        /// <exception cref="ObjectDisposedException">Redis provider has been disposed.</exception>
        IRedisMessageSubscription CreateSubscription();

        /// <summary>
        /// Posts a message to a channel.
        /// </summary>
        /// <param name="channel">The channel to post the message to.</param>
        /// <param name="message">The message to post.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="channel"/> is null or empty string, or parameter <paramref name="message"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">Redis provider has been disposed.</exception>
        void PublishMessage(string channel, string message);

        #endregion Pub/Sub

        #region Hash

        /// <summary>
        /// Returns all fields and values of the hash stored at key.
        /// </summary>
        /// <remarks>If the key does not exists in Redis, empty <see cref="IReadOnlyDictionary{String, String}" /> is returned.</remarks>
        /// <param name="key">The key whose hash values to retrieve.</param>
        /// <returns>A dictionary with all hash values associated with the specified key.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string.</exception>
        /// <exception cref="ObjectDisposedException">Redis provider has been disposed.</exception>
        /// <exception cref="RedisException">The key type is not hash.</exception>
        IReadOnlyDictionary<string, string> GetHashAsText(string key);

        /// <summary>
        /// Returns all fields and values of the hash stored at key.
        /// </summary>
        /// <remarks>If the key does not exists in Redis, empty <see cref="IReadOnlyDictionary{String, String}" /> is returned.</remarks>
        /// <param name="key">The key whose hash values to retrieve.</param>
        /// <returns>A dictionary with all hash values associated with the specified key.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string.</exception>
        /// <exception cref="ObjectDisposedException">Redis provider has been disposed.</exception>
        /// <exception cref="RedisException">The key type is not hash.</exception>
        IReadOnlyDictionary<string, byte[]> GetHashAsBinary(string key);

        /// <summary>
        /// Returns all values in the hash stored at key.
        /// </summary>
        /// <remarks>If the key does not exists in Redis, empty <see cref="IEnumerable{String}" /> is returned.</remarks>
        /// <param name="key">The key whose hash values to retrieve.</param>
        /// <returns>A collection with all hash values associated with the specified key.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string.</exception>
        /// <exception cref="ObjectDisposedException">Redis provider has been disposed.</exception>
        /// <exception cref="RedisException">The key type is not hash.</exception>
        IEnumerable<byte[]> GetHashValuesAsBinary(string key);

        /// <summary>
        /// Retrieves a single text value from a hash.
        /// </summary>
        /// <param name="key">The key whose hash value to retrieve.</param>
        /// <param name="field">The field whose value to retrieve.</param>
        /// <returns>The value associated with the specified field in the specified hash, or null if there
        /// is no cache with the specified hash key, or there is no field with the specified field name.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string, or parameter <paramref name="field"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">Redis provider has been disposed.</exception>
        /// <exception cref="RedisException">The key type is not hash.</exception>
        string GetHashTextValue(string key, string field);

        /// <summary>
        /// Retrieves a single binary value from a hash.
        /// </summary>
        /// <param name="key">The key whose hash value to retrieve.</param>
        /// <param name="field">The field whose value to retrieve.</param>
        /// <returns>The value associated with the specified field in the specified hash, or null if there
        /// is no cache with the specified hash key, or there is no field with the specified field name.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string, or parameter <paramref name="field"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">Redis provider has been disposed.</exception>
        /// <exception cref="RedisException">The key type is not hash.</exception>
        byte[] GetHashBinaryValue(string key, string field);

        /// <summary>
        /// Sets a single text value in a hash.
        /// </summary>
        /// <param name="key">The key whose hash value to set.</param>
        /// <param name="field">The field whose value to set.</param>
        /// <param name="value">The value to set in the hash for the field.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string,
        /// or <paramref name="field"/> is null, or <paramref name="value"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">Redis provider has been disposed.</exception>
        void SetHashValue(string key, string field, string value);

        /// <summary>
        /// Sets a single binary value in a hash.
        /// </summary>
        /// <param name="key">The key whose hash value to set.</param>
        /// <param name="field">The field whose value to set.</param>
        /// <param name="value">The value to set in the hash for the field.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string,
        /// or <paramref name="field"/> is null, or <paramref name="value"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">Redis provider has been disposed.</exception>
        void SetHashValue(string key, string field, byte[] value);

        /// <summary>
        /// Sets the specified fields to their respective values in the hash stored at key.
        /// </summary>
        /// <remarks>This command overwrites any existing fields in the hash. If key does not exist, a new key holding a hash is created.</remarks>
        /// <param name="key">The key whose hash values to set.</param>
        /// <param name="values">The values to set in hash.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string, or parameter <paramref name="values"/> is null.</exception>
        /// <exception cref="ArgumentException">One or more values in the dictionary <paramref name="values"/> are null.</exception>
        /// <exception cref="ObjectDisposedException">Redis provider has been disposed.</exception>
        void SetHashValues(string key, IReadOnlyDictionary<string, string> values);

        /// <summary>
        /// Sets the specified fields to their respective values in the hash stored at key.
        /// </summary>
        /// <remarks>This command overwrites any existing fields in the hash. If key does not exist, a new key holding a hash is created.</remarks>
        /// <param name="key">The key whose hash values to set.</param>
        /// <param name="values">The values to set in hash.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string, or parameter <paramref name="values"/> is null.</exception>
        /// <exception cref="ArgumentException">One or more values in the dictionary <paramref name="values"/> are null.</exception>
        /// <exception cref="ObjectDisposedException">Redis provider has been disposed.</exception>
        void SetHashValues(string key, IReadOnlyDictionary<string, byte[]> values);

        /// <summary>
        /// Increments the number stored at field in the hash stored at key by increment.
        /// </summary>
        /// <remarks>If key does not exist, a new key holding a hash is created. If field does not exist the value is set to 0 before the operation is performed.</remarks>
        /// <param name="key">The key whose hash value to increment.</param>
        /// <param name="field">The field whose value to increment.</param>
        /// <param name="incrementBy">The step with which to increment the hash value.</param>
        /// <returns>The value at field after the increment operation.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string, or parameter <paramref name="field"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">Redis provider has been disposed.</exception>
        /// <exception cref="RedisException">Key type is not hash or value cannot be parsed to a number.</exception>
        long IncrementValueInHash(string key, string field, int incrementBy);

        /// <summary>
        /// Removes the specified fields from the hash stored at key.
        /// </summary>
        /// <param name="key">The key whose field to remove.</param>
        /// <param name="fields">The fields to remove.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string, or parameter <paramref name="fields"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">Redis provider has been disposed.</exception>
        /// <exception cref="RedisException">Key type is not hash.</exception>
        void RemoveFromHash(string key, params string[] fields);

        #endregion Hash

        #region List

        /// <summary>
        /// Returns all elements of the list stored at key as strings.
        /// </summary>
        /// <remarks>If <paramref name="key"/> does not exists in Redis, empty <see cref="IEnumerable{String}"/> is returned.</remarks>
        /// <param name="key">The key whose list elements to retrieve.</param>
        /// <returns>All elements of the list stored at key.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string.</exception>
        /// <exception cref="ObjectDisposedException">Redis provider has been disposed.</exception>
        /// <exception cref="RedisException">Key type is not list.</exception>
        IEnumerable<string> GetListAsText(string key);

        /// <summary>
        /// Returns all elements of the list stored at key as byte arrays.
        /// </summary>
        /// <remarks>If <paramref name="key"/> does not exists in Redis, empty <see cref="IEnumerable{String}"/> is returned.</remarks>
        /// <param name="key">The key whose list elements to retrieve.</param>
        /// <returns>All elements of the list stored at key.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string.</exception>
        /// <exception cref="ObjectDisposedException">Redis provider has been disposed.</exception>
        /// <exception cref="RedisException">Key type is not list.</exception>
        IEnumerable<byte[]> GetListAsBinary(string key);

        /// <summary>
        /// Gets a range of elements from a list.
        /// </summary>
        /// <remarks>
        /// <para>If <paramref name="key"/> does not exists in Redis, empty <see cref="IEnumerable{String}"/> is returned.</para>
        /// <para>The offsets <paramref name="startIndex" /> and <paramref name="endIndex" /> are zero-based indexes, with 0 being the first element of the list (the head of the list), 1 being the next element and so on.
        /// These offsets can also be negative numbers indicating offsets starting at the end of the list.For example, -1 is the last element of the list, -2 the penultimate, and so on.</para>
        /// </remarks>
        /// <param name="key">The key whose list elements to retrieve.</param>
        /// <param name="startIndex">The first zero-based index to retrieve.</param>
        /// <param name="endIndex">The last zero-based index to retrieve.</param>
        /// <returns>The range of elements in the list between the specified indexes.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string.</exception>
        /// <exception cref="ObjectDisposedException">Redis provider has been disposed.</exception>
        /// <exception cref="RedisException">Key type is not list.</exception>
        IEnumerable<string> GetListRange(string key, int startIndex, int endIndex);

        /// <summary>
        /// Returns a list element specified by its index.
        /// </summary>
        /// <param name="key">The list whose element to retrieve.</param>
        /// <param name="index">The zero-based index of the element to retrieve.</param>
        /// <returns>The element in the list with the specified index, or null if index is greater than the size of the list.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string.</exception>
        /// <exception cref="ObjectDisposedException">Redis provider has been disposed.</exception>
        /// <exception cref="RedisException">Key type is not list.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Parameter <paramref name="index"/> is less than 0.</exception>
        string GetListElement(string key, int index);

        /// <summary>
        /// Removes the first element of a list and returns it as string.
        /// </summary>
        /// <param name="key">The list whose element to pop.</param>
        /// <returns>The removed first element from the list, or null if the list is empty.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string.</exception>
        /// <exception cref="ObjectDisposedException">Redis provider has been disposed.</exception>
        /// <exception cref="RedisException">Key type is not list.</exception>
        string PopFirstListElementAsText(string key);

        /// <summary>
        /// Removes the first element of a list and returns it as byte array.
        /// </summary>
        /// <param name="key">The list whose element to pop.</param>
        /// <returns>The removed first element from the list, or null if the list is empty.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string.</exception>
        /// <exception cref="ObjectDisposedException">Redis provider has been disposed.</exception>
        /// <exception cref="RedisException">Key type is not list.</exception>
        byte[] PopFirstListElementAsBinary(string key);

        /// <summary>
        /// Inserts the specified element at the tail of the list stored at key.
        /// </summary>
        /// <param name="key">The key of the list to insert the specified item.</param>
        /// <param name="element">The item to insert to the list.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string, or parameter <paramref name="element"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">Redis provider has been disposed.</exception>
        /// <exception cref="RedisException">Key type is not list.</exception>
        void AddToList(string key, string element);

        /// <summary>
        /// Inserts the specified element at the tail of the list stored at key.
        /// </summary>
        /// <param name="key">The key of the list to insert the specified item.</param>
        /// <param name="element">The item to insert to the list.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string, or parameter <paramref name="element"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">Redis provider has been disposed.</exception>
        /// <exception cref="RedisException">Key type is not list.</exception>
        void AddToList(string key, byte[] element);

        /// <summary>
        /// Removes the occurrences of elements equal to value from the list stored at key.
        /// </summary>
        /// <param name="key">The key of the list to remove elements.</param>
        /// <param name="element">The value of the elements who should be removed.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string, or parameter <paramref name="element"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">Redis provider has been disposed.</exception>
        /// <exception cref="RedisException">Key type is not list.</exception>
        void RemoveFromList(string key, string element);

        #endregion List

        #region Set

        /// <summary>
        /// Returns all the members of the set value stored at key.
        /// </summary>
        /// <remarks>If the key does not exists in Redis, empty <see cref="IEnumerable{String}" /> is returned.</remarks>
        /// <param name="key">The key whose member to retrieve.</param>
        /// <returns>All members of the set stored at key.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string.</exception>
        /// <exception cref="ObjectDisposedException">Redis provider has been disposed.</exception>
        /// <exception cref="RedisException">Key type is not set.</exception>
        IEnumerable<string> GetSet(string key);

        /// <summary>
        /// Adds a member to the set stored at key.
        /// </summary>
        /// <param name="key">The key of the set to add the specified member.</param>
        /// <param name="member">The member to add to the set.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string, or parameter <paramref name="member"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">Redis provider has been disposed.</exception>
        /// <exception cref="RedisException">Key type is not set.</exception>
        void AddToSet(string key, string member);

        /// <summary>
        /// Removes a member from the set stored at key.
        /// </summary>
        /// <param name="key">The key whose members to remove.</param>
        /// <param name="member">The member to remove from set.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string, or parameter <paramref name="member"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">Redis provider has been disposed.</exception>
        /// <exception cref="RedisException">Key type is not set.</exception>
        void RemoveFromSet(string key, string member);

        #endregion Set

        #region Server

        /// <summary>
        /// Deletes all keys in all existing Redis databases.
        /// </summary>
        void FlushAll();

        #endregion Server

        #region Transaction, Pipelines, Locks, Scripts

        /// <summary>
        /// Creates a Redis pipeline.
        /// </summary>
        /// <returns>A Redis pipeline object.</returns>
        /// <exception cref="ObjectDisposedException">Redis provider has been disposed.</exception>
        IRedisPipeline CreatePipeline();

        /// <summary>
        /// Creates a Redis transaction.
        /// </summary>
        /// <returns>A Redis transaction object.</returns>
        /// <exception cref="ObjectDisposedException">Redis provider has been disposed.</exception>
        IRedisTransaction CreateTransaction();

        /// <summary>
        /// Acquires a lock specified by an unique key.
        /// </summary>
        /// <param name="key">An unique key for the lock.</param>
        /// <returns>A Redis lock object.</returns>
        /// <exception cref="ObjectDisposedException">Redis provider has been disposed.</exception>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty.</exception>
        IDisposable AcquireLock(string key);

        /// <summary>
        /// Acquires a lock specified by an unique key, or throws an exception if the timeout is exceeded.
        /// </summary>
        /// <param name="key">An unique key for the lock.</param>
        /// <param name="timeout">The timeout for acquiring the lock.</param>
        /// <returns>A Redis lock object.</returns>
        /// <exception cref="ObjectDisposedException">Redis provider has been disposed.</exception>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Parameter <paramref name="timeout"/> is negative.</exception>
        /// <exception cref="TimeoutException">The lock cannot be acquired within the specified <paramref name="timeout"/>.</exception>
        IDisposable AcquireLock(string key, TimeSpan timeout);

        /// <summary>
        /// Executes a LUA scripts.
        /// </summary>
        /// <typeparam name="TResult">The type of the result returned by the script.</typeparam>
        /// <param name="script">The body of the LUA script to execute.</param>
        /// <param name="parameters">The parameters for the LUA script.</param>
        /// <returns>The result returned by the LUA script.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="script"/> is null or empty string, or <paramref name="parameters"/> is null.</exception>
        /// <exception cref="RedisException">Parameter <paramref name="script"/> is an invalid script or an error occurred during script execution.</exception>
        /// <exception cref="ObjectDisposedException">Redis provider has been disposed.</exception>
        /// <exception cref="NotSupportedException">Result type <typeparamref name="TResult"/> is not supported.</exception>
        TResult ExecuteLuaScript<TResult>(string script, params string[] parameters);

        #endregion Transaction, Pipelines, Locks, Scripts
    }
}