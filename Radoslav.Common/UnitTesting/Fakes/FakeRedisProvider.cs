using System;
using System.Collections.Generic;
using Radoslav.Redis;

namespace Radoslav
{
    public sealed class FakeRedisProvider : IRedisProvider
    {
        public IDisposable AcquireLock(string key)
        {
            throw new NotImplementedException();
        }

        public IDisposable AcquireLock(string key, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public void AddToList(string key, byte[] element)
        {
            throw new NotImplementedException();
        }

        public void AddToList(string key, string element)
        {
            throw new NotImplementedException();
        }

        public void AddToSet(string key, string member)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(string key)
        {
            throw new NotImplementedException();
        }

        public IRedisPipeline CreatePipeline()
        {
            throw new NotImplementedException();
        }

        public IRedisMessageSubscription CreateSubscription()
        {
            return new FakeRedisSubscription();
        }

        public IRedisTransaction CreateTransaction()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }

        public TResult ExecuteLuaScript<TResult>(string script, params string[] parameters)
        {
            throw new NotImplementedException();
        }

        public bool ExpireKeyIn(string key, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public void FlushAll()
        {
            throw new NotImplementedException();
        }

        public byte[] GetBinaryValue(string key)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyDictionary<string, byte[]> GetHashAsBinary(string key)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyDictionary<string, string> GetHashAsText(string key)
        {
            throw new NotImplementedException();
        }

        public byte[] GetHashBinaryValue(string key, string field)
        {
            throw new NotImplementedException();
        }

        public string GetHashTextValue(string key, string field)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<byte[]> GetHashValuesAsBinary(string key)
        {
            throw new NotImplementedException();
        }

        public RedisKeyType GetKeyType(string key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<byte[]> GetListAsBinary(string key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetListAsText(string key)
        {
            throw new NotImplementedException();
        }

        public string GetListElement(string key, int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetListRange(string key, int startIndex, int endIndex)
        {
            throw new NotImplementedException();
        }

        public DateTime GetServerDateTimeUtc()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetSet(string key)
        {
            throw new NotImplementedException();
        }

        public string GetTextValue(string key)
        {
            throw new NotImplementedException();
        }

        public long IncrementValue(string key)
        {
            throw new NotImplementedException();
        }

        public long IncrementValueInHash(string key, string field, int incrementBy)
        {
            throw new NotImplementedException();
        }

        public byte[] PopFirstListElementAsBinary(string key)
        {
            throw new NotImplementedException();
        }

        public string PopFirstListElementAsText(string key)
        {
            throw new NotImplementedException();
        }

        public void PublishMessage(string channel, string message)
        {
            throw new NotImplementedException();
        }

        public void RemoveFromHash(string key, params string[] fields)
        {
            throw new NotImplementedException();
        }

        public void RemoveFromList(string key, string element)
        {
            throw new NotImplementedException();
        }

        public void RemoveFromSet(string key, string member)
        {
            throw new NotImplementedException();
        }

        public void RemoveKey(string key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> SearchKeys(string pattern)
        {
            throw new NotImplementedException();
        }

        public void SetHashValue(string key, string field, byte[] value)
        {
            throw new NotImplementedException();
        }

        public void SetHashValue(string key, string field, string value)
        {
            throw new NotImplementedException();
        }

        public void SetHashValues(string key, IReadOnlyDictionary<string, byte[]> values)
        {
            throw new NotImplementedException();
        }

        public void SetHashValues(string key, IReadOnlyDictionary<string, string> values)
        {
            throw new NotImplementedException();
        }

        public void SetValue(string key, byte[] value)
        {
            throw new NotImplementedException();
        }

        public void SetValue(string key, string value)
        {
            throw new NotImplementedException();
        }

        public bool SetValueIfNotExists(string key, string value)
        {
            throw new NotImplementedException();
        }
    }
}