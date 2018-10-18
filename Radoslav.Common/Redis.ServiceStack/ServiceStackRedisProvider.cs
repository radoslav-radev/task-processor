using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ServiceStack;
using ServiceStack.Redis;
using ServiceStack.Text;

namespace Radoslav.Redis.ServiceStack
{
    /// <summary>
    /// An implementation of <see cref="IRedisProvider"/> that uses ServiceStack as an underlying provider.
    /// </summary>
    public sealed partial class ServiceStackRedisProvider : IRedisProvider, IDisposable
    {
        private readonly RedisClient client;
        private readonly string debugName = typeof(ServiceStackRedisProvider).Name;

        private bool isDisposed;

        #region Constructors & Destructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceStackRedisProvider"/> class.
        /// </summary>
        public ServiceStackRedisProvider()
        {
            Trace.WriteLine("ENTER: Constructing {0} ...".FormatInvariant(this.debugName));

            this.client = new RedisClient();

            Trace.WriteLine("EXIT: {0} constructed.".FormatInvariant(this.debugName));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceStackRedisProvider"/> class.
        /// </summary>
        /// <param name="host">The address where Redis is hosted.</param>
        public ServiceStackRedisProvider(string host)
        {
            Trace.WriteLine("ENTER: Constructing {0} ...".FormatInvariant(this.debugName));

            if (host == null)
            {
                host = string.Empty;
            }

            this.client = new RedisClient(host);

            Trace.WriteLine("EXIT: {0} constructed.".FormatInvariant(this.debugName));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceStackRedisProvider"/> class.
        /// </summary>
        /// <param name="host">The address where Redis is hosted.</param>
        /// <param name="port">The port on which to communicate with Redis.</param>
        /// <exception cref="ArgumentOutOfRangeException">Parameter <paramref name="port"/> is less than 0 or greater than 65536.</exception>
        public ServiceStackRedisProvider(string host, int port)
        {
            Trace.WriteLine("ENTER: Constructing {0} ...".FormatInvariant(this.debugName));

            if (host == null)
            {
                host = string.Empty;
            }

            if ((port < 0) || (port > 65536))
            {
                throw new ArgumentOutOfRangeException(nameof(port), port, "Port must be between 0 and 65536.");
            }

            this.client = new RedisClient(host, port);

            Trace.WriteLine("EXIT: {0} constructed.".FormatInvariant(this.debugName));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceStackRedisProvider"/> class.
        /// </summary>
        /// <param name="host">The address where Redis is hosted.</param>
        /// <param name="port">The port to use when connecting to Redis.</param>
        /// <param name="password">The password to use when connecting to Redis.</param>
        /// <param name="database">The database to connect to.</param>
        public ServiceStackRedisProvider(string host, int port, string password, int database)
        {
            Trace.WriteLine("ENTER: Constructing {0} ...".FormatInvariant(this.debugName));

            if (host == null)
            {
                host = string.Empty;
            }

            this.client = new RedisClient(host, port, password, database);

            Trace.WriteLine("EXIT: {0} constructed.".FormatInvariant(this.debugName));
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="ServiceStackRedisProvider"/> class.
        /// </summary>
        ~ServiceStackRedisProvider()
        {
            Trace.WriteLine("ENTER: Destructing {0} ...".FormatInvariant(this.debugName));

            this.Dispose();

            Trace.WriteLine("EXIT: {0} destructed.".FormatInvariant(this.debugName));
        }

        #endregion Constructors & Destructor

        #region IDisposable Members

        /// <inheritdoc />
        public void Dispose()
        {
            Trace.WriteLine("ENTER: Disposing {0} ...".FormatInvariant(this.debugName));

            if (this.isDisposed)
            {
                Trace.WriteLine("EXIT: {0} is already disposed.".FormatInvariant(this.debugName));

                return;
            }

            this.client.Dispose();

            GC.SuppressFinalize(this);

            this.isDisposed = true;

            Trace.WriteLine("EXIT: {0} disposed.".FormatInvariant(this.debugName));
        }

        #endregion IDisposable Members

        #region IRedisProvider/Time Members

        /// <inheritdoc />
        public DateTime GetServerDateTimeUtc()
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.debugName);
            }

            byte[][] value;

            lock (this.client)
            {
                value = this.client.Time();
            }

            return ServiceStackHelpers.ConvertDateTime(value);
        }

        #endregion IRedisProvider/Time Members

        #region IRedisProvider/Keys Members

        /// <inheritdoc />
        public IEnumerable<string> SearchKeys(string pattern)
        {
            Trace.WriteLine("ENTER: Searching keys with pattern '{0}' ...".FormatInvariant(pattern));

            if (string.IsNullOrEmpty(pattern))
            {
                throw new ArgumentNullException("pattern");
            }

            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.debugName);
            }

            lock (this.client)
            {
                return this.client.SearchKeys(pattern);
            }
        }

        /// <inheritdoc />
        public bool ContainsKey(string key)
        {
            Trace.WriteLine("ENTER: Checking if key '{0}' exists ...".FormatInvariant(key));

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.debugName);
            }

            bool result;

            lock (this.client)
            {
                result = this.client.ContainsKey(key);
            }

            if (result)
            {
                Trace.WriteLine("EXIT: Key '{0}' exists.".FormatInvariant(key));
            }
            else
            {
                Trace.WriteLine("EXIT: Key '{0}' does not exists.".FormatInvariant(key));
            }

            return result;
        }

        /// <inheritdoc />
        public RedisKeyType GetKeyType(string key)
        {
            Trace.WriteLine("ENTER: Getting type for key '{0}' ...".FormatInvariant(key));

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.debugName);
            }

            global::ServiceStack.Redis.RedisKeyType keyType = this.client.GetEntryType(key);

            RedisKeyType result = ServiceStackHelpers.ConvertKeyType(keyType);

            Trace.WriteLine("EXIT: Return type {0} for key '{0}' ...".FormatInvariant(result, key));

            return result;
        }

        /// <inheritdoc />
        public string GetTextValue(string key)
        {
            Trace.WriteLine("ENTER: Getting value for key '{0}' ...".FormatInvariant(key));

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.debugName);
            }

            string result;

            try
            {
                lock (this.client)
                {
                    result = this.client.GetValue(key);
                }
            }
            catch (RedisResponseException ex)
            {
                throw new RedisException("Failed to get value for key '{0}'. See inner exception for details.".FormatInvariant(key), ex);
            }

            Trace.WriteLine("EXIT: Value '{0}' returned for key '{1}'.".FormatInvariant(result, key));

            return result;
        }

        /// <inheritdoc />
        public byte[] GetBinaryValue(string key)
        {
            Trace.WriteLine("ENTER: Getting binary value for key '{0}' ...".FormatInvariant(key));

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.debugName);
            }

            byte[] result;

            try
            {
                lock (this.client)
                {
                    result = this.client.Get(key);
                }
            }
            catch (RedisResponseException ex)
            {
                throw new RedisException("Failed to get value for key '{0}'. See inner exception for details.".FormatInvariant(key), ex);
            }

            Trace.WriteLine("EXIT: Binary value returned for key '{0}'.".FormatInvariant(key));

            return result;
        }

        /// <inheritdoc />
        public void SetValue(string key, string value)
        {
            Trace.WriteLine("ENTER: Setting value '{0}' to key '{1}' ...".FormatInvariant(value, key));

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.debugName);
            }

            lock (this.client)
            {
                this.client.Set(key, value);
            }

            Trace.WriteLine("EXIT: Value '{0}' set to key '{1}'.".FormatInvariant(value, key));
        }

        /// <inheritdoc />
        public void SetValue(string key, byte[] value)
        {
            Trace.WriteLine("ENTER: Setting binary value to key '{0}' ...".FormatInvariant(key));

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.debugName);
            }

            lock (this.client)
            {
                this.client.Set(key, value);
            }

            Trace.WriteLine("EXIT: Binary value set to key '{0}'.".FormatInvariant(key));
        }

        /// <inheritdoc />
        public bool SetValueIfNotExists(string key, string value)
        {
            Trace.WriteLine("ENTER: Setting value '{0}' to key '{1}' if not exists ...".FormatInvariant(value, key));

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.debugName);
            }

            bool result;

            lock (this.client)
            {
                result = this.client.SetValueIfExists(key, value ?? string.Empty);
            }

            if (result)
            {
                Trace.WriteLine("EXIT: Value '{0}' set to key '{1}'.".FormatInvariant(value, key));
            }
            else
            {
                Trace.WriteLine("EXIT: Value '{0}' was not set to key '{1}' because it already has another value.".FormatInvariant(value, key));
            }

            return result;
        }

        /// <inheritdoc />
        public long IncrementValue(string key)
        {
            Trace.WriteLine("ENTER: Incrementing value for key '{0}' ...'.".FormatInvariant(key));

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.debugName);
            }

            long result;

            try
            {
                lock (this.client)
                {
                    result = this.client.IncrementValue(key);
                }
            }
            catch (RedisResponseException ex)
            {
                throw new RedisException("Failed to increment value for key '{0}'. See inner exception for details.".FormatInvariant(key), ex);
            }

            Trace.WriteLine("EXIT: Value for key '{0}' incremented to {1}.'.".FormatInvariant(key, result));

            return result;
        }

        /// <inheritdoc />
        public bool ExpireKeyIn(string key, TimeSpan timeout)
        {
            Trace.WriteLine("ENTER: Setting '{0}' expiration to key '{1}' ...".FormatInvariant(timeout, key));

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (timeout <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("timeout", timeout, "Value must be positive.");
            }

            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.debugName);
            }

            bool result;

            lock (this.client)
            {
                result = this.client.ExpireEntryIn(key, timeout);
            }

            if (result)
            {
                Trace.WriteLine("EXIT: Expiration '{0}' set successfully to key '{1}'.".FormatInvariant(timeout, key));
            }
            else
            {
                Trace.WriteLine("EXIT: Expiration '{0}' was not set to key '{1}' because key does not exists or expiration cannot be set.".FormatInvariant(timeout, key));
            }

            return result;
        }

        /// <inheritdoc />
        public void RemoveKey(string key)
        {
            Trace.WriteLine("ENTER: Removing key '{0}' ...".FormatInvariant(key));

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.debugName);
            }

            lock (this.client)
            {
                this.client.Remove(key);
            }

            Trace.WriteLine("EXIT: Key '{0}' removed.".FormatInvariant(key));
        }

        #endregion IRedisProvider/Keys Members

        #region IRedisProvider/Pub/Sub Members

        /// <inheritdoc />
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = SuppressMessages.CallerResponsibleForDispose)]
        public IRedisMessageSubscription CreateSubscription()
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.debugName);
            }

            return new ServiceStackSubscription(this.client.CloneClient());
        }

        /// <inheritdoc />
        public void PublishMessage(string channel, string message)
        {
            Trace.WriteLine("ENTER: Publishing message '{0}' to channel '{1}' ...".FormatInvariant(message, channel));

            if (string.IsNullOrEmpty(channel))
            {
                throw new ArgumentNullException("channel");
            }

            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.debugName);
            }

            lock (this.client)
            {
                this.client.PublishMessage(channel, message);
            }

            Trace.WriteLine("EXIT: Message '{0}' published to channel '{1}'.".FormatInvariant(message, channel));
        }

        #endregion IRedisProvider/Pub/Sub Members

        #region IRedisProvider/Hash Members

        /// <inheritdoc />
        public IReadOnlyDictionary<string, string> GetHashAsText(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.debugName);
            }

            try
            {
                lock (this.client)
                {
                    return this.client.GetAllEntriesFromHash(key);
                }
            }
            catch (RedisResponseException ex)
            {
                throw new RedisException("Failed to get hash for key '{0}'. See inner exception for details.".FormatInvariant(key), ex);
            }
        }

        /// <inheritdoc />
        public IReadOnlyDictionary<string, byte[]> GetHashAsBinary(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.debugName);
            }

            byte[][] result;

            try
            {
                lock (this.client)
                {
                    result = this.client.HGetAll(key);
                }
            }
            catch (RedisResponseException ex)
            {
                throw new RedisException("Failed to get hash for key '{0}'. See inner exception for details.".FormatInvariant(key), ex);
            }

            return result.ToBinaryDictionary();
        }

        /// <inheritdoc />
        public IEnumerable<byte[]> GetHashValuesAsBinary(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.debugName);
            }

            byte[][] result;

            try
            {
                lock (this.client)
                {
                    result = this.client.HVals(key);
                }
            }
            catch (RedisResponseException ex)
            {
                throw new RedisException("Failed to get hash for key '{0}'. See inner exception for details.".FormatInvariant(key), ex);
            }

            return result;
        }

        /// <inheritdoc />
        public string GetHashTextValue(string key, string field)
        {
            Trace.WriteLine("ENTER: Getting field '{0}' from hash '{1}' ...".FormatInvariant(field, key));

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (field == null)
            {
                throw new ArgumentNullException("field");
            }

            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.debugName);
            }

            string result;

            try
            {
                lock (this.client)
                {
                    result = this.client.GetValueFromHash(key, field);
                }
            }
            catch (RedisResponseException ex)
            {
                throw new RedisException("Failed to get field '{0}' from hash '{1}'. See inner exception for details.".FormatInvariant(field, key), ex);
            }

            Trace.WriteLine("EXIT: Field '{0}' from hash '{1}' retrieved: {2}.".FormatInvariant(field, key, result));

            return result;
        }

        /// <inheritdoc />
        public byte[] GetHashBinaryValue(string key, string field)
        {
            Trace.WriteLine("ENTER: Getting field '{0}' from hash '{1}' ...".FormatInvariant(field, key));

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (field == null)
            {
                throw new ArgumentNullException("field");
            }

            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.debugName);
            }

            byte[] result;

            try
            {
                lock (this.client)
                {
                    result = this.client.HGet(key, field.ToUtf8Bytes());
                }
            }
            catch (RedisResponseException ex)
            {
                throw new RedisException("Failed to get field '{0}' from hash '{1}'. See inner exception for details.".FormatInvariant(field, key), ex);
            }

            Trace.WriteLine("EXIT: Field '{0}' from hash '{1}' retrieved.".FormatInvariant(field, key));

            return result;
        }

        /// <inheritdoc />
        public void SetHashValue(string key, string field, string value)
        {
            Trace.WriteLine("ENTER: Setting field '{0}' in hash '{1}' to '{2}' ...".FormatInvariant(field, key, value));

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (field == null)
            {
                throw new ArgumentNullException("field");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.debugName);
            }

            try
            {
                lock (this.client)
                {
                    this.client.SetEntryInHash(key, field, value);
                }
            }
            catch (RedisResponseException ex)
            {
                throw new RedisException("Failed to set field '{0}' in hash '{1}' to '{2}'. See inner exception for details.".FormatInvariant(field, key, value), ex);
            }

            Trace.WriteLine("EXIT: Field '{0}' in hash '{1}' set to '{2}'.".FormatInvariant(field, key, value));
        }

        /// <inheritdoc />
        public void SetHashValue(string key, string field, byte[] value)
        {
            Trace.WriteLine("ENTER: Setting field '{0}' in hash '{1}' to binary value ...".FormatInvariant(field, key));

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (field == null)
            {
                throw new ArgumentNullException("field");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.debugName);
            }

            try
            {
                lock (this.client)
                {
                    this.client.HSet(key, field.ToUtf8Bytes(), value);
                }
            }
            catch (RedisResponseException ex)
            {
                throw new RedisException("Failed to set field '{0}' in hash '{1}' to binary value. See inner exception for details.".FormatInvariant(field, key), ex);
            }

            Trace.WriteLine("EXIT: Field '{0}' in hash '{1}' set to binary value.".FormatInvariant(field, key));
        }

        /// <inheritdoc />
        public void SetHashValues(string key, IReadOnlyDictionary<string, string> values)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (values == null)
            {
                throw new ArgumentNullException("values");
            }

            Trace.WriteLine("ENTER: Setting {0} values to hash '{1}' ...".FormatInvariant(values.Count, key));

            if (values.Count == 0)
            {
                Trace.WriteLine("EXIT: No values to set to hash '{0}'.".FormatInvariant(key));

                return;
            }

            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.debugName);
            }

            lock (this.client)
            {
                this.client.SetRangeInHash(key, values);
            }

            Trace.WriteLine("EXIT: {0} values set to hash '{1}'.".FormatInvariant(values.Count, key));
        }

        /// <inheritdoc />
        public void SetHashValues(string key, IReadOnlyDictionary<string, byte[]> values)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (values == null)
            {
                throw new ArgumentNullException("values");
            }

            Trace.WriteLine("ENTER: Setting {0} values to hash '{1}' ...".FormatInvariant(values.Count, key));

            if (values.Count == 0)
            {
                Trace.WriteLine("EXIT: No values to set to hash '{0}'.".FormatInvariant(key));

                return;
            }

            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.debugName);
            }

            lock (this.client)
            {
                this.client.HMSet(
                    key,
                    values.Keys.Select(k => k.ToUtf8Bytes()).ToArray(),
                    values.Values.ToArray());
            }

            Trace.WriteLine("EXIT: {0} values set to hash '{1}'.".FormatInvariant(values.Count, key));
        }

        /// <inheritdoc />
        public long IncrementValueInHash(string key, string field, int incrementBy)
        {
            Trace.WriteLine("ENTER: Incrementing field '{0}' in hash '{1}' by {2} ...".FormatInvariant(field, key, incrementBy));

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (field == null)
            {
                throw new ArgumentNullException("field");
            }

            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.debugName);
            }

            long result;

            try
            {
                lock (this.client)
                {
                    result = this.client.IncrementValueInHash(key, field, incrementBy);
                }
            }
            catch (RedisResponseException ex)
            {
                throw new RedisException("Failed to increment field '{0}' in hash '{1}'. See inner exception for details.".FormatInvariant(field, key), ex);
            }

            Trace.WriteLine("EXIT: Field '{0}' in hash '{1}' incremented by {2} to {3}.".FormatInvariant(field, key, incrementBy, result));

            return result;
        }

        /// <inheritdoc />
        public void RemoveFromHash(string key, params string[] fields)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (fields == null)
            {
                throw new ArgumentNullException("fields");
            }

            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.debugName);
            }

            Trace.WriteLine("ENTER: Removing fields '{0}' from hash '{1}' ...".FormatInvariant(fields.ToString(","), key));

            if (fields.Length == 0)
            {
                Trace.WriteLine("EXIT: No fields to remove from hash '{0}'.".FormatInvariant(key));

                return;
            }

            try
            {
                lock (this.client)
                {
                    this.client.HDel(key, fields.Select(f => f.ToUtf8Bytes()).ToArray());
                }
            }
            catch (RedisResponseException ex)
            {
                throw new RedisException("Failed to remove fields {0} from hash '{1}'. See inner exception for details.".FormatInvariant(fields.ToString(","), key), ex);
            }

            Trace.WriteLine("EXIT: Fields {0} removed from hash '{1}'.".FormatInvariant(fields.ToString(","), key));
        }

        #endregion IRedisProvider/Hash Members

        #region IRedisProvider/List Members

        /// <inheritdoc />
        public IEnumerable<string> GetListAsText(string key)
        {
            Trace.WriteLine("ENTER: Getting Redis list '{0}' ...".FormatInvariant(key));

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.debugName);
            }

            ICollection<string> result;

            try
            {
                lock (this.client)
                {
                    result = this.client.GetAllItemsFromList(key);
                }
            }
            catch (RedisResponseException ex)
            {
                throw new RedisException("Failed to get list '{0}'. See inner exception for details.".FormatInvariant(key), ex);
            }

            Trace.WriteLine("EXIT: Redis list '{0}' retrieved with {1} elements.".FormatInvariant(key, result.Count));

            return result;
        }

        /// <inheritdoc />
        public IEnumerable<byte[]> GetListAsBinary(string key)
        {
            Trace.WriteLine("ENTER: Getting Redis list '{0}' ...".FormatInvariant(key));

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.debugName);
            }

            byte[][] result;

            try
            {
                lock (this.client)
                {
                    result = this.client.LRange(key, 0, -1);
                }
            }
            catch (RedisResponseException ex)
            {
                throw new RedisException("Failed to get list '{0}'. See inner exception for details.".FormatInvariant(key), ex);
            }

            Trace.WriteLine("EXIT: Redis list '{0}' retrieved with {1} elements.".FormatInvariant(key, result.Length));

            return result;
        }

        /// <inheritdoc />
        public IEnumerable<string> GetListRange(string key, int startIndex, int endIndex)
        {
            Trace.WriteLine("ENTER: Getting list '{0}' range from {1} to {2} ...".FormatInvariant(key, startIndex, endIndex));

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.debugName);
            }

            ICollection<string> result;

            try
            {
                lock (this.client)
                {
                    result = this.client.GetRangeFromList(key, startIndex, endIndex);
                }
            }
            catch (RedisResponseException ex)
            {
                throw new RedisException("Failed to get list '{0}' range from {1} to {2}. See inner exception for details.".FormatInvariant(key, startIndex, endIndex), ex);
            }

            Trace.WriteLine("EXIT: List '{0}' range from {1} to {2} retrieved with {3} elements.".FormatInvariant(key, startIndex, endIndex, result.Count));

            return result;
        }

        /// <inheritdoc />
        public string GetListElement(string key, int index)
        {
            Trace.WriteLine("ENTER: Getting list '{0}' element {1} ...".FormatInvariant(key, index));

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.debugName);
            }

            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index", index, "Value must not be negative.");
            }

            string result;

            try
            {
                lock (this.client)
                {
                    result = this.client.GetItemFromList(key, index);
                }
            }
            catch (RedisResponseException ex)
            {
                throw new RedisException("Failed to get list '{0}'. See inner exception for details.".FormatInvariant(key), ex);
            }

            Trace.WriteLine("EXIT: List '{0}' element {1} retrieved: {2}.".FormatInvariant(key, index, result));

            return result;
        }

        /// <inheritdoc />
        public void AddToList(string key, string element)
        {
            Trace.WriteLine("ENTER: Adding '{0}' to list '{1}' ...".FormatInvariant(element, key));

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.debugName);
            }

            try
            {
                lock (this.client)
                {
                    this.client.AddItemToList(key, element);
                }
            }
            catch (RedisResponseException ex)
            {
                throw new RedisException("Failed to add element '{0}' to list '{1}'. See inner exception for details.".FormatInvariant(element, key), ex);
            }

            Trace.WriteLine("EXIT: '{0}' added to list '{1}'.".FormatInvariant(element, key));
        }

        /// <inheritdoc />
        public void AddToList(string key, byte[] element)
        {
            Trace.WriteLine("ENTER: Adding binary element to list '{0}' ...".FormatInvariant(key));

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.debugName);
            }

            try
            {
                lock (this.client)
                {
                    this.client.RPush(key, element);
                }
            }
            catch (RedisResponseException ex)
            {
                throw new RedisException("Failed to add element '{0}' to list '{1}'. See inner exception for details.".FormatInvariant(element, key), ex);
            }

            Trace.WriteLine("EXIT: Binary element added to list '{0}'.".FormatInvariant(key));
        }

        /// <inheritdoc />
        public void RemoveFromList(string key, string element)
        {
            Trace.WriteLine("ENTER: Removing '{0}' from list '{1}' ...".FormatInvariant(element, key));

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.debugName);
            }

            try
            {
                lock (this.client)
                {
                    this.client.RemoveItemFromList(key, element);
                }
            }
            catch (RedisResponseException ex)
            {
                throw new RedisException("Failed to remove element '{0}' from list '{1}'. See inner exception for details.".FormatInvariant(element, key), ex);
            }

            Trace.WriteLine("EXIT: '{0}' removed from list '{1}'.".FormatInvariant(element, key));
        }

        /// <inheritdoc />
        public string PopFirstListElementAsText(string key)
        {
            Trace.WriteLine("ENTER: Popping first list '{0}' element ...".FormatInvariant(key));

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.debugName);
            }

            string result;

            try
            {
                lock (this.client)
                {
                    result = this.client.LPop(key).FromUtf8Bytes();
                }
            }
            catch (RedisResponseException ex)
            {
                throw new RedisException("Failed to pop first element from list '{0}'. See inner exception for details.".FormatInvariant(key), ex);
            }

            Trace.WriteLine("EXIT: First list '{0}' element popped: '{1}'.".FormatInvariant(key, result));

            return result;
        }

        /// <inheritdoc />
        public byte[] PopFirstListElementAsBinary(string key)
        {
            Trace.WriteLine("ENTER: Popping first list '{0}' element ...".FormatInvariant(key));

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.debugName);
            }

            byte[] result;

            try
            {
                lock (this.client)
                {
                    result = this.client.LPop(key);
                }
            }
            catch (RedisResponseException ex)
            {
                throw new RedisException("Failed to pop first element from list '{0}'. See inner exception for details.".FormatInvariant(key), ex);
            }

            Trace.WriteLine("EXIT: First list '{0}' element popped.".FormatInvariant(key));

            return result;
        }

        #endregion IRedisProvider/List Members

        #region IRedisProvider/Set Members

        /// <inheritdoc />
        public IEnumerable<string> GetSet(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.debugName);
            }

            try
            {
                lock (this.client)
                {
                    return this.client.GetAllItemsFromSet(key);
                }
            }
            catch (RedisResponseException ex)
            {
                throw new RedisException("Failed to get set '{0}'. See inner exception for details.".FormatInvariant(key), ex);
            }
        }

        /// <inheritdoc />
        public void AddToSet(string key, string member)
        {
            Trace.WriteLine("ENTER: Adding item '{0}' to set '{1}' ...".FormatInvariant(member, key));

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (member == null)
            {
                throw new ArgumentNullException("member");
            }

            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.debugName);
            }

            try
            {
                lock (this.client)
                {
                    this.client.AddItemToSet(key, member);
                }
            }
            catch (RedisResponseException ex)
            {
                throw new RedisException("Failed to add member '{0}' to set '{1}'. See inner exception for details.".FormatInvariant(member, key), ex);
            }

            Trace.WriteLine("EXIT: Item '{0}' added to set '{1}'.".FormatInvariant(member, key));
        }

        /// <inheritdoc />
        public void RemoveFromSet(string key, string member)
        {
            Trace.WriteLine("ENTER: Removing item '{0}' from set '{1}' ...".FormatInvariant(member, key));

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (member == null)
            {
                throw new ArgumentNullException("member");
            }

            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.debugName);
            }

            try
            {
                lock (this.client)
                {
                    this.client.RemoveItemFromSet(key, member);
                }
            }
            catch (RedisResponseException ex)
            {
                throw new RedisException("Failed to remove member '{0}' from set '{1}'. See inner exception for details.".FormatInvariant(member, key), ex);
            }

            Trace.WriteLine("EXIT: Item '{0}' removed from set '{1}'.".FormatInvariant(member, key));
        }

        #endregion IRedisProvider/Set Members

        #region IRedisProvider/Server

        /// <inheritdoc />
        public void FlushAll()
        {
            Trace.WriteLine("ENTER: Flushing all ...");

            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.debugName);
            }

            lock (this.client)
            {
                this.client.FlushAll();
            }

            Trace.WriteLine("EXIT: All flushed.");
        }

        #endregion IRedisProvider/Server

        #region IRedisProvider/Transaction, Pipelines, Locks

        /// <inheritdoc />
        public IRedisPipeline CreatePipeline()
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            RedisClient clone = this.client.CloneClient();

            return new ServiceStackPipeline(clone, clone.CreatePipeline());
        }

        /// <inheritdoc />
        public IRedisTransaction CreateTransaction()
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            RedisClient clone = this.client.CloneClient();

            return new ServiceStackTransaction(clone, clone.CreateTransaction());
        }

        /// <inheritdoc />
        public IDisposable AcquireLock(string key)
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            return this.client.AcquireLock(key);
        }

        /// <inheritdoc />
        public IDisposable AcquireLock(string key, TimeSpan timeout)
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (timeout < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("timeout", timeout, "Value must not be negative.");
            }

            return this.client.AcquireLock(key, timeout);
        }

        #endregion IRedisProvider/Transaction, Pipelines, Locks

        /// <inheritdoc />
        public TResult ExecuteLuaScript<TResult>(string script, params string[] parameters)
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.debugName);
            }

            if (string.IsNullOrWhiteSpace(script))
            {
                throw new ArgumentNullException("script");
            }

            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            byte[][] parameterBytes = parameters.Select(p => p.ToUtf8Bytes()).ToArray();

            byte[][] rawResult;

            try
            {
                rawResult = this.client.Eval(script, parameters.Length, parameterBytes);
            }
            catch (RedisResponseException ex)
            {
                throw new RedisException(ex.Message, ex);
            }

            if (typeof(TResult) == typeof(IReadOnlyDictionary<string, string>))
            {
                string[] keysAndValues = rawResult.Select(r => r.FromUtf8Bytes()).ToArray();

                Dictionary<string, string> result = new Dictionary<string, string>();

                if ((keysAndValues.Length > 0) && !string.IsNullOrEmpty(keysAndValues[0]))
                {
                    for (int i = 0; i < keysAndValues.Length; i += 2)
                    {
                        result.Add(keysAndValues[i], keysAndValues[i + 1]);
                    }
                }

                return (TResult)(object)result;
            }

            throw new NotSupportedException<Type>(typeof(TResult));
        }
    }
}