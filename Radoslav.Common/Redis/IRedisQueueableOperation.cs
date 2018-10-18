using System;
using System.Collections.Generic;

namespace Radoslav.Redis
{
    /// <summary>
    /// A basic functionality of a complex Redis operation that supports queuing simple operations that are executed as batch.
    /// </summary>
    public interface IRedisQueueableOperation : IDisposable
    {
        /// <summary>
        /// Gets the string value of a specified key.
        /// </summary>
        /// <param name="key">The key which value to retrieve from Redis.</param>
        /// <param name="callback">The callback to execute when the value is retrieved from Redis.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string, or <paramref name="callback" /> is null.</exception>
        /// <exception cref="ObjectDisposedException">The queueable operation has been disposed.</exception>
        void GetTextValue(string key, Action<string> callback);

        /// <summary>
        /// Gets a binary value for a specified key.
        /// </summary>
        /// <param name="key">The key which value to retrieve from Redis.</param>
        /// <param name="callback">The callback to execute when the value is retrieved from Redis.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string.</exception>
        /// <exception cref="ObjectDisposedException">The queueable operation has been disposed.</exception>
        void GetBinaryValue(string key, Action<byte[]> callback);

        /// <summary>
        /// Set key to hold the string value.
        /// </summary>
        /// <param name="key">The key whose value should be set.</param>
        /// <param name="value">The value to be associated with the specified key.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string.</exception>
        /// <exception cref="ObjectDisposedException">The queueable operation has been disposed.</exception>
        void SetValue(string key, string value);

        /// <summary>
        /// Set key to hold the binary value.
        /// </summary>
        /// <param name="key">The key whose value should be set.</param>
        /// <param name="value">The value to be associated with the specified key.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string, or <paramref name="value" /> is null.</exception>
        /// <exception cref="ObjectDisposedException">The queueable operation has been disposed.</exception>
        void SetValue(string key, byte[] value);

        /// <summary>
        /// Sets a timeout on a key. After the timeout has expired, the key will automatically be deleted from Redis.
        /// </summary>
        /// <param name="key">The key to set a timeout.</param>
        /// <param name="timeout">The timeout to be set on the specified key.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Parameter <paramref name="timeout"/> is less or equal to <see cref="TimeSpan.Zero"/>.</exception>
        /// <exception cref="ObjectDisposedException">The queueable operation has been disposed.</exception>
        void ExpireKeyIn(string key, TimeSpan timeout);

        /// <summary>
        /// Removes the specified key from Redis. If the key does not exists, it is ignored.
        /// </summary>
        /// <param name="key">The key to remove.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string.</exception>
        /// <exception cref="ObjectDisposedException">The queueable operation has been disposed.</exception>
        void RemoveKey(string key);

        /// <summary>
        /// Returns all elements of the list stored at key.
        /// </summary>
        /// <param name="key">The key whose list elements to retrieve.</param>
        /// <param name="callback">The callback to execute when the list elements are retrieved from Redis.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string, or <paramref name="callback" /> is null.</exception>
        /// <exception cref="ObjectDisposedException">The queueable operation has been disposed.</exception>
        void GetList(string key, Action<IEnumerable<string>> callback);

        /// <summary>
        /// Removes the first element of a list and returns it as string.
        /// </summary>
        /// <param name="key">The list whose element to pop.</param>
        /// <param name="callback">The callback to execute when the first element is popped from the list.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string, or <paramref name="callback"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">The queueable operation has been disposed.</exception>
        void PopFirstListElementAsText(string key, Action<string> callback);

        /// <summary>
        /// Removes the first element of a list and returns it as byte array.
        /// </summary>
        /// <param name="key">The list whose element to pop.</param>
        /// <param name="callback">The callback to execute when the first element is popped from the list.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string, or <paramref name="callback"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">The queueable operation has been disposed.</exception>
        void PopFirstListElementAsBinary(string key, Action<byte[]> callback);

        /// <summary>
        /// Inserts the specified element at the tail of the list stored at key.
        /// </summary>
        /// <param name="key">The key of the list to insert the specified item.</param>
        /// <param name="element">The item to insert to the list.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string, or parameter <paramref name="element"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">The queueable operation has been disposed.</exception>
        void AddToList(string key, string element);

        /// <summary>
        /// Removes the occurrences of elements equal to value from the list stored at key.
        /// </summary>
        /// <param name="key">The key of the list to remove elements.</param>
        /// <param name="element">The value of the elements who should be removed.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string, or parameter <paramref name="element"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">The queueable operation has been disposed.</exception>
        void RemoveFromList(string key, string element);

        /// <summary>
        /// Adds a member to the set stored at key.
        /// </summary>
        /// <param name="key">The key of the set to add the specified member.</param>
        /// <param name="member">The member to add to the set.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string, or parameter <paramref name="member"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">The queueable operation has been disposed.</exception>
        void AddToSet(string key, string member);

        /// <summary>
        /// Removes a member from the set stored at key.
        /// </summary>
        /// <param name="key">The key whose members to remove.</param>
        /// <param name="member">The member to remove from set.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string, or parameter <paramref name="member"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">The queueable operation has been disposed.</exception>
        void RemoveFromSet(string key, string member);

        /// <summary>
        /// Returns all fields and values of the hash stored at key.
        /// </summary>
        /// <param name="key">The key whose hash values to retrieve.</param>
        /// <param name="callback">The callback to execute when the hash is retrieved from Redis.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string, or <paramref name="callback"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">The queueable operation has been disposed.</exception>
        void GetHash(string key, Action<IReadOnlyDictionary<string, string>> callback);

        /// <summary>
        /// Retrieves a single text value from a hash.
        /// </summary>
        /// <param name="key">The key whose hash value to retrieve.</param>
        /// <param name="field">The field whose value to retrieve.</param>
        /// <param name="callback">The callback to execute when the hash value is retrieved from Redis.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string, or parameter <paramref name="field"/> is null, or <paramref name="callback"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">The queueable operation has been disposed.</exception>
        void GetHashTextValue(string key, string field, Action<string> callback);

        /// <summary>
        /// Retrieves a single binary value from a hash.
        /// </summary>
        /// <param name="key">The key whose hash value to retrieve.</param>
        /// <param name="field">The field whose value to retrieve.</param>
        /// <param name="callback">The callback to execute when the hash value is retrieved from Redis.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string, or parameter <paramref name="field"/> is null, or <paramref name="callback"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">The queueable operation has been disposed.</exception>
        void GetHashBinaryValue(string key, string field, Action<byte[]> callback);

        /// <summary>
        /// Sets a single text value in a hash.
        /// </summary>
        /// <param name="key">The key whose hash value to set.</param>
        /// <param name="field">The field whose value to set.</param>
        /// <param name="value">The value to set in the hash for the field.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string,
        /// or <paramref name="field"/> is null, or <paramref name="value"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">The queueable operation has been disposed.</exception>
        void SetHashValue(string key, string field, string value);

        /// <summary>
        /// Sets a single binary value in a hash.
        /// </summary>
        /// <param name="key">The key whose hash value to set.</param>
        /// <param name="field">The field whose value to set.</param>
        /// <param name="value">The value to set in the hash for the field.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string,
        /// or <paramref name="field"/> is null, or <paramref name="value"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">The queueable operation has been disposed.</exception>
        void SetHashValue(string key, string field, byte[] value);

        /// <summary>
        /// Sets the specified fields to their respective values in the hash stored at key.
        /// </summary>
        /// <remarks>This command overwrites any existing fields in the hash. If key does not exist, a new key holding a hash is created.</remarks>
        /// <param name="key">The key whose hash values to set.</param>
        /// <param name="values">The values to set in hash.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string, or parameter <paramref name="values"/> is null.</exception>
        /// <exception cref="ArgumentException">One or more values in the dictionary <paramref name="values"/> are null.</exception>
        /// <exception cref="ObjectDisposedException">The queueable operation has been disposed.</exception>
        void SetHashValues(string key, IReadOnlyDictionary<string, string> values);

        /// <summary>
        /// Sets the specified fields to their respective values in the hash stored at key.
        /// </summary>
        /// <remarks>This command overwrites any existing fields in the hash. If key does not exist, a new key holding a hash is created.</remarks>
        /// <param name="key">The key whose hash values to set.</param>
        /// <param name="values">The values to set in hash.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string, or parameter <paramref name="values"/> is null.</exception>
        /// <exception cref="ArgumentException">One or more values in the dictionary <paramref name="values"/> are null.</exception>
        /// <exception cref="ObjectDisposedException">The queueable operation has been disposed.</exception>
        void SetHashValues(string key, IReadOnlyDictionary<string, byte[]> values);

        /// <summary>
        /// Removes the specified fields from the hash stored at key.
        /// </summary>
        /// <param name="key">The key whose field to remove.</param>
        /// <param name="fields">The fields to remove.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty string, or parameter <paramref name="fields"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">Redis provider has been disposed.</exception>
        /// <exception cref="RedisException">Key type is not hash.</exception>
        void RemoveFromHash(string key, params string[] fields);
    }
}