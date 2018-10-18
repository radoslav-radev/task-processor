using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Redis;
using ServiceStack.Text;
using ServiceStack;

namespace Radoslav.Redis.ServiceStack
{
    internal abstract class ServiceStackQueueableOperation : IRedisQueueableOperation, IDisposable
    {
        private readonly RedisClient client;
        private readonly global::ServiceStack.Redis.Pipeline.IRedisQueueableOperation operation;

        #region Constructors / Destructor

        protected ServiceStackQueueableOperation(RedisClient client, global::ServiceStack.Redis.Pipeline.IRedisQueueableOperation operation)
        {
            this.client = client;
            this.operation = operation;

            this.IsEmpty = true;
        }

        ~ServiceStackQueueableOperation()
        {
            this.Dispose(false);
        }

        #endregion Constructors / Destructor

        #region Properties

        protected bool IsDisposed { get; private set; }

        protected bool IsEmpty { get; set; }

        #endregion Properties

        #region IRedisQueueableOperation Members

        public void GetTextValue(string key, Action<string> callback)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            this.operation.QueueCommand(client => client.GetValue(key), value => callback(value));

            this.IsEmpty = false;
        }

        public void SetValue(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            this.operation.QueueCommand(client => client.Set(key, value));

            this.IsEmpty = false;
        }

        public void SetValue(string key, byte[] value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            this.operation.QueueCommand(client => ((IRedisNativeClient)client).Set(key, value));

            this.IsEmpty = false;
        }

        public void GetBinaryValue(string key, Action<byte[]> callback)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            this.operation.QueueCommand(client => ((IRedisNativeClient)client).Get(key), value => callback(value));

            this.IsEmpty = false;
        }

        public void ExpireKeyIn(string key, TimeSpan timeout)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (timeout <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(timeout), timeout, "Value must be positive.");
            }

            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            this.operation.QueueCommand(client => client.ExpireEntryIn(key, timeout));

            this.IsEmpty = false;
        }

        public void RemoveKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            this.operation.QueueCommand(client => client.Remove(key));

            this.IsEmpty = false;
        }

        public void GetList(string key, Action<IEnumerable<string>> callback)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }

            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            this.operation.QueueCommand(client => client.GetAllItemsFromList(key), values => callback(values));

            this.IsEmpty = false;
        }

        public void PopFirstListElementAsText(string key, Action<string> callback)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }

            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            this.operation.QueueCommand(client => ((RedisClient)client).LPop(key), value => callback(value.FromUtf8Bytes()));

            this.IsEmpty = false;
        }

        public void PopFirstListElementAsBinary(string key, Action<byte[]> callback)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }

            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            this.operation.QueueCommand(client => ((RedisClient)client).LPop(key), value => callback(value));

            this.IsEmpty = false;
        }

        public void AddToList(string key, string element)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            this.operation.QueueCommand(client => client.AddItemToList(key, element));

            this.IsEmpty = false;
        }

        public void RemoveFromList(string key, string element)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            this.operation.QueueCommand(client => client.RemoveItemFromList(key, element));

            this.IsEmpty = false;
        }

        public void GetHash(string key, Action<IReadOnlyDictionary<string, string>> callback)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            this.operation.QueueCommand(
                client => ((RedisClient)client).HGetAll(key),
                array => callback(array.ToStringDictionary()));

            this.IsEmpty = false;
        }

        public void GetHashTextValue(string key, string field, Action<string> callback)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (field == null)
            {
                throw new ArgumentNullException(nameof(field));
            }

            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            this.operation.QueueCommand(client => client.GetValueFromHash(key, field), result => callback(result));

            this.IsEmpty = false;
        }

        public void GetHashBinaryValue(string key, string field, Action<byte[]> callback)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (field == null)
            {
                throw new ArgumentNullException(nameof(field));
            }

            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            this.operation.QueueCommand(client => ((IRedisNativeClient)client).HGet(key, field.ToUtf8Bytes()), result => callback(result));

            this.IsEmpty = false;
        }

        public void SetHashValue(string key, string field, byte[] value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (field == null)
            {
                throw new ArgumentNullException(nameof(field));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            this.operation.QueueCommand(client => ((IRedisNativeClient)client).HSet(key, field.ToUtf8Bytes(), value));

            this.IsEmpty = false;
        }

        public void SetHashValue(string key, string field, string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (field == null)
            {
                throw new ArgumentNullException(nameof(field));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            this.operation.QueueCommand(client => client.SetEntryInHash(key, field, value));

            this.IsEmpty = false;
        }

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

            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            this.operation.QueueCommand(client => client.SetRangeInHash(key, values));

            this.IsEmpty = false;
        }

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

            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            this.operation.QueueCommand(client => ((IRedisNativeClient)client).HMSet(
                key,
                values.Keys.Select(k => k.ToUtf8Bytes()).ToArray(),
                values.Values.ToArray()));

            this.IsEmpty = false;
        }

        public void AddToSet(string key, string member)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (member == null)
            {
                throw new ArgumentNullException("member");
            }

            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            this.operation.QueueCommand(client => client.AddItemToSet(key, member));

            this.IsEmpty = false;
        }

        public void RemoveFromSet(string key, string member)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (member == null)
            {
                throw new ArgumentNullException("member");
            }

            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            this.operation.QueueCommand(client => client.RemoveItemFromSet(key, member));

            this.IsEmpty = false;
        }

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

            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            if (fields.Length == 0)
            {
                return;
            }

            this.operation.QueueCommand(client => ((RedisNativeClient)client).HDel(key, fields.Select(f => f.ToUtf8Bytes()).ToArray()));

            this.IsEmpty = false;
        }

        #endregion IRedisQueueableOperation Members

        #region IDisposable Members

        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Members

        protected virtual void Dispose(bool disposing)
        {
            this.client.Dispose();

            this.IsDisposed = true;
        }
    }
}