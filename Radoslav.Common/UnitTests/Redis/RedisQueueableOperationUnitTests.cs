using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Redis;

namespace Radoslav.Common.UnitTests
{
    [TestClass]
    public abstract class RedisQueueableOperationUnitTests
    {
        #region Properties & Initialize

        public TestContext TestContext { get; set; }

        protected IRedisProvider Provider
        {
            get
            {
                return (IRedisProvider)this.TestContext.Properties["Provider"];
            }
        }

        protected IRedisQueueableOperation QueueableOperation
        {
            get
            {
                return (IRedisQueueableOperation)this.TestContext.Properties["QueueableOperation"];
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            this.TestContext.Properties.Add("Provider", this.CreateRedisProvider());
            this.TestContext.Properties.Add("QueueableOperation", this.CreateQueueableOperation());
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this.QueueableOperation.Dispose();
            this.Provider.Dispose();
        }

        #endregion Properties & Initialize

        #region GetTextValue

        [TestMethod]
        public void GetTextValue()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.Provider.SetValue(key, "Hello");

            string value = null;

            this.QueueableOperation.GetTextValue(key, v => value = v);

            this.CompleteOperation();

            Assert.AreEqual("Hello", value);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void GetTextValueDisposed()
        {
            this.QueueableOperation.Dispose();

            this.QueueableOperation.GetTextValue("Temp", value => { });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetTextValueNullKey()
        {
            this.QueueableOperation.GetTextValue(null, value => { });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetTextValueEmptyKey()
        {
            this.QueueableOperation.GetTextValue(string.Empty, value => { });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetTextValueNullCallback()
        {
            this.QueueableOperation.GetTextValue("Temp", null);
        }

        #endregion GetTextValue

        #region SetTextValue

        [TestMethod]
        public void SetTextValue()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.QueueableOperation.SetValue(key, "Hello");

            this.CompleteOperation();

            Assert.AreEqual("Hello", this.Provider.GetTextValue(key));
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void SetTextValueDisposed()
        {
            this.QueueableOperation.Dispose();

            this.QueueableOperation.SetValue("Temp", string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetTextValueNullKey()
        {
            this.QueueableOperation.SetValue(null, string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetTextValueEmptyKey()
        {
            this.QueueableOperation.SetValue(string.Empty, string.Empty);
        }

        [TestMethod]
        public void SetTextValueNullValue()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.QueueableOperation.SetValue(key, (string)null);

            this.CompleteOperation();

            Assert.IsTrue(string.IsNullOrEmpty(this.Provider.GetTextValue(key)));
        }

        #endregion SetTextValue

        #region GetBinaryValue

        [TestMethod]
        public void GetBinaryValue()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.Provider.SetValue(key, Encoding.UTF8.GetBytes("Hello"));

            byte[] value = null;

            this.QueueableOperation.GetBinaryValue(key, v => value = v);

            this.CompleteOperation();

            Assert.AreEqual("Hello", Encoding.UTF8.GetString(value));
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void GetBinaryValueDisposed()
        {
            this.QueueableOperation.Dispose();

            this.QueueableOperation.GetBinaryValue("Temp", value => { });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetBinaryValueNullKey()
        {
            this.QueueableOperation.GetBinaryValue(null, value => { });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetBinaryValueEmptyKey()
        {
            this.QueueableOperation.GetBinaryValue(string.Empty, value => { });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetBinaryValueNullCallback()
        {
            this.QueueableOperation.GetBinaryValue("Temp", null);
        }

        #endregion GetBinaryValue

        #region SetBinaryValue

        [TestMethod]
        public void SetBinaryValue()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.QueueableOperation.SetValue(key, Encoding.UTF8.GetBytes("Hello"));

            this.CompleteOperation();

            Assert.AreEqual("Hello", Encoding.UTF8.GetString(this.Provider.GetBinaryValue(key)));
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void SetBinaryValueDisposed()
        {
            this.QueueableOperation.Dispose();

            this.QueueableOperation.SetValue("Temp", new byte[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetBinaryValueNullKey()
        {
            this.QueueableOperation.SetValue(null, new byte[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetBinaryValueEmptyKey()
        {
            this.QueueableOperation.SetValue(string.Empty, new byte[0]);
        }

        [TestMethod]
        public void SetBinaryValueNullValue()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.QueueableOperation.SetValue(key, (byte[])null);

            this.CompleteOperation();

            byte[] result = this.Provider.GetBinaryValue(key);

            Assert.IsTrue((result == null) || (result.Length == 0));
        }

        #endregion SetBinaryValue

        #region RemoveKey

        [TestMethod]
        public void RemoveKey()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.Provider.SetValue(key, "Hello");

            this.QueueableOperation.RemoveKey(key);

            this.CompleteOperation();

            Assert.IsNull(this.Provider.GetTextValue(key));
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void RemoveKeyDisposed()
        {
            this.QueueableOperation.Dispose();

            this.QueueableOperation.RemoveKey("Temp");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveKeyNullKey()
        {
            this.QueueableOperation.RemoveKey(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveKeyEmptyKey()
        {
            this.QueueableOperation.RemoveKey(string.Empty);
        }

        #endregion RemoveKey

        #region AddToSet

        [TestMethod]
        public void AddToSet()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.QueueableOperation.AddToSet(key, "A");
            this.QueueableOperation.AddToSet(key, "B");

            this.CompleteOperation();

            IEnumerable<string> actualValues = this.Provider.GetSet(key);

            Assert.AreEqual(2, actualValues.Count());

            Assert.IsTrue(actualValues.Contains("A"));
            Assert.IsTrue(actualValues.Contains("B"));
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void AddToSetDisposed()
        {
            this.QueueableOperation.Dispose();

            this.QueueableOperation.AddToSet("Temp", "1");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddToSetNullKey()
        {
            this.QueueableOperation.AddToSet(null, "1");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddToSetEmptyKey()
        {
            this.QueueableOperation.AddToSet(string.Empty, "1");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddToSetNullElement()
        {
            this.QueueableOperation.AddToSet("Temp", null);
        }

        #endregion AddToSet

        #region RemoveFromSet

        [TestMethod]
        public void RemoveFromSet()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.Provider.AddToSet(key, "A");
            this.Provider.AddToSet(key, "B");

            this.QueueableOperation.RemoveFromSet(key, "A");

            this.CompleteOperation();

            IEnumerable<string> actualValues = this.Provider.GetSet(key);

            Assert.AreEqual(1, actualValues.Count());

            Assert.IsFalse(actualValues.Contains("A"));
            Assert.IsTrue(actualValues.Contains("B"));
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void RemoveFromSetDisposed()
        {
            this.QueueableOperation.Dispose();

            this.QueueableOperation.RemoveFromSet("Temp", "A");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveFromSetNullKey()
        {
            this.QueueableOperation.RemoveFromSet(null, "A");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveFromSetEmptyKey()
        {
            this.QueueableOperation.RemoveFromSet(string.Empty, "A");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveFromSetNullElement()
        {
            this.QueueableOperation.RemoveFromSet("Temp", null);
        }

        #endregion RemoveFromSet

        #region GetList

        [TestMethod]
        public void GetList()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.Provider.AddToList(key, "A");
            this.Provider.AddToList(key, "B");

            IEnumerable<string> values;

            this.QueueableOperation.GetList(key, v => values = v);

            this.CompleteOperation();

            IEnumerable<string> actualValues = this.Provider.GetListAsText(key);

            Assert.AreEqual(2, actualValues.Count());

            Assert.IsTrue(actualValues.Contains("A"));
            Assert.IsTrue(actualValues.Contains("B"));
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void GetListDisposed()
        {
            this.QueueableOperation.Dispose();

            this.QueueableOperation.GetList("Temp", values => { });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetListNullKey()
        {
            this.QueueableOperation.GetList(null, values => { });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetListEmptyKey()
        {
            this.QueueableOperation.GetList(string.Empty, values => { });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetListNullCallback()
        {
            this.QueueableOperation.GetList("Key", null);
        }

        #endregion GetList

        #region PopFirstListElementAsText

        [TestMethod]
        public void PopFirstListElementAsText()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.Provider.AddToList(key, "A");
            this.Provider.AddToList(key, "B");

            string result1 = null, result2 = null, result3 = null;

            this.QueueableOperation.PopFirstListElementAsText(key, v => result1 = v);
            this.QueueableOperation.PopFirstListElementAsText(key, v => result2 = v);
            this.QueueableOperation.PopFirstListElementAsText(key, v => result3 = v);

            this.CompleteOperation();

            Assert.AreEqual("A", result1);
            Assert.AreEqual("B", result2);
            Assert.IsTrue(string.IsNullOrEmpty(result3));
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void PopFirstListElementAsTextDisposed()
        {
            this.QueueableOperation.Dispose();

            this.QueueableOperation.PopFirstListElementAsText("Temp", values => { });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PopFirstListElementAsTextNullKey()
        {
            this.QueueableOperation.PopFirstListElementAsText(null, values => { });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PopFirstListElementAsTextEmptyKey()
        {
            this.QueueableOperation.PopFirstListElementAsText(string.Empty, values => { });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PopFirstListElementAsTextNullCallback()
        {
            this.QueueableOperation.PopFirstListElementAsText("Key", null);
        }

        #endregion PopFirstListElementAsText

        #region PopFirstListElementAsBinary

        [TestMethod]
        public void PopFirstListElementAsBinary()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.Provider.AddToList(key, Encoding.ASCII.GetBytes("A"));
            this.Provider.AddToList(key, Encoding.ASCII.GetBytes("B"));

            byte[] result1 = null, result2 = null, result3 = null;

            this.QueueableOperation.PopFirstListElementAsBinary(key, v => result1 = v);
            this.QueueableOperation.PopFirstListElementAsBinary(key, v => result2 = v);
            this.QueueableOperation.PopFirstListElementAsBinary(key, v => result3 = v);

            this.CompleteOperation();

            Assert.AreEqual("A", Encoding.ASCII.GetString(result1));
            Assert.AreEqual("B", Encoding.ASCII.GetString(result2));
            Assert.IsNull(result3);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void PopFirstListElementAsBinaryDisposed()
        {
            this.QueueableOperation.Dispose();

            this.QueueableOperation.PopFirstListElementAsBinary("Temp", values => { });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PopFirstListElementAsBinaryNullKey()
        {
            this.QueueableOperation.PopFirstListElementAsBinary(null, values => { });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PopFirstListElementAsBinaryEmptyKey()
        {
            this.QueueableOperation.PopFirstListElementAsBinary(string.Empty, values => { });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PopFirstListElementAsBinaryNullCallback()
        {
            this.QueueableOperation.PopFirstListElementAsBinary("Key", null);
        }

        #endregion PopFirstListElementAsBinary

        #region AddToList

        [TestMethod]
        public void AddToList()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.QueueableOperation.AddToList(key, "A");
            this.QueueableOperation.AddToList(key, "B");

            this.CompleteOperation();

            IEnumerable<string> actualValues = this.Provider.GetListAsText(key);

            Assert.AreEqual(2, actualValues.Count());

            Assert.IsTrue(actualValues.Contains("A"));
            Assert.IsTrue(actualValues.Contains("B"));
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void AddToListDisposed()
        {
            this.QueueableOperation.Dispose();

            this.QueueableOperation.AddToList("Temp", "A");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddToListNullKey()
        {
            this.QueueableOperation.AddToList(null, "A");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddToListEmptyKey()
        {
            this.QueueableOperation.AddToList(string.Empty, "A");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddToListNullElement()
        {
            this.QueueableOperation.AddToList("Temp", null);
        }

        #endregion AddToList

        #region RemoveFromList

        [TestMethod]
        public void RemoveFromList()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.Provider.AddToList(key, "A");
            this.Provider.AddToList(key, "B");

            this.QueueableOperation.RemoveFromList(key, "A");

            this.CompleteOperation();

            IEnumerable<string> actualValues = this.Provider.GetListAsText(key);

            Assert.AreEqual(1, actualValues.Count());

            Assert.IsFalse(actualValues.Contains("A"));
            Assert.IsTrue(actualValues.Contains("B"));
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void RemoveFromListDisposed()
        {
            this.QueueableOperation.Dispose();

            this.QueueableOperation.AddToList("Temp", "A");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveFromListNullKey()
        {
            this.QueueableOperation.AddToList(null, "A");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveFromListEmptyKey()
        {
            this.QueueableOperation.AddToList(string.Empty, "A");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveFromListNullElement()
        {
            this.QueueableOperation.AddToList("Temp", null);
        }

        #endregion RemoveFromList

        #region GetHash

        [TestMethod]
        public void GetHash()
        {
            Dictionary<string, string> expectedValues = new Dictionary<string, string>();

            expectedValues.Add("A", "1");
            expectedValues.Add("B", "2");

            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.Provider.SetHashValues(key, expectedValues);

            IReadOnlyDictionary<string, string> actualValues = null;

            this.QueueableOperation.GetHash(key, values => actualValues = values);

            this.CompleteOperation();

            Assert.IsNotNull(actualValues);

            Assert.AreEqual(2, actualValues.Count);

            Assert.AreEqual("1", actualValues["A"]);
            Assert.AreEqual("2", actualValues["B"]);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void GetHashDisposed()
        {
            this.QueueableOperation.Dispose();

            this.QueueableOperation.GetHash("Temp", values => { });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetHashNullKey()
        {
            this.QueueableOperation.GetHash(null, values => { });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetHashEmptyKey()
        {
            this.QueueableOperation.GetHash(string.Empty, values => { });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetHashNullValues()
        {
            this.QueueableOperation.GetHash("Temp", null);
        }

        #endregion GetHash

        #region GetHashTextValue

        [TestMethod]
        public void GetHashTextValue()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.Provider.SetHashValue(key, "Hello", "World");

            string value = null;

            this.QueueableOperation.GetHashTextValue(key, "Hello", v => value = v);

            this.CompleteOperation();

            Assert.AreEqual("World", value);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void GetHashTextValueDisposed()
        {
            this.QueueableOperation.Dispose();

            this.QueueableOperation.GetHashTextValue("Temp", "Field", value => { });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetHashTextValueNullKey()
        {
            this.QueueableOperation.GetHashTextValue(null, "Field", value => { });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetHashTextValueEmptyKey()
        {
            this.QueueableOperation.GetHashTextValue(string.Empty, "Field", value => { });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetHashTextValueNullField()
        {
            this.QueueableOperation.GetHashTextValue("Temp", null, value => { });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetHashTextValueNullCallback()
        {
            this.QueueableOperation.GetHashTextValue("Temp", "Field", null);
        }

        #endregion GetHashTextValue

        #region GetHashTextValue

        [TestMethod]
        public void GetHashBinaryValue()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.Provider.SetHashValue(key, "Hello", Encoding.ASCII.GetBytes("World"));

            byte[] value = null;

            this.QueueableOperation.GetHashBinaryValue(key, "Hello", v => value = v);

            this.CompleteOperation();

            Assert.AreEqual("World", Encoding.ASCII.GetString(value));
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void GetHashBinaryValueDisposed()
        {
            this.QueueableOperation.Dispose();

            this.QueueableOperation.GetHashBinaryValue("Temp", "Field", value => { });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetHashBinaryValueNullKey()
        {
            this.QueueableOperation.GetHashBinaryValue(null, "Field", value => { });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetHashBinaryValueEmptyKey()
        {
            this.QueueableOperation.GetHashBinaryValue(string.Empty, "Field", value => { });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetHashBinaryValueNullField()
        {
            this.QueueableOperation.GetHashBinaryValue("Temp", null, value => { });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetHashBinaryValueNullCallback()
        {
            this.QueueableOperation.GetHashBinaryValue("Temp", "Field", null);
        }

        #endregion GetHashTextValue

        #region SetHashValues (Text)

        [TestMethod]
        public void SetHashValuesText()
        {
            Dictionary<string, string> expectedValues = new Dictionary<string, string>();

            expectedValues.Add("A", "1");
            expectedValues.Add("B", "2");

            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.QueueableOperation.SetHashValues(key, expectedValues);

            this.CompleteOperation();

            IReadOnlyDictionary<string, string> actualValues = this.Provider.GetHashAsText(key);

            Assert.IsNotNull(actualValues);

            Assert.AreEqual(2, actualValues.Count);

            Assert.AreEqual("1", actualValues["A"]);
            Assert.AreEqual("2", actualValues["B"]);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void SetHashValuesTextDisposed()
        {
            this.QueueableOperation.Dispose();

            this.QueueableOperation.SetHashValues("Temp", new Dictionary<string, string>());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetHashValuesTextNullKey()
        {
            this.QueueableOperation.SetHashValues(null, new Dictionary<string, string>());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetHashValuesTextEmptyKey()
        {
            this.QueueableOperation.SetHashValues(string.Empty, new Dictionary<string, string>());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetHashValuesTextNullValues()
        {
            this.QueueableOperation.SetHashValues("Temp", default(IReadOnlyDictionary<string, string>));
        }

        #endregion SetHashValues (Text)

        #region SetHashValues (Binary)

        [TestMethod]
        public void SetHashValuesBinary()
        {
            Dictionary<string, byte[]> expectedValues = new Dictionary<string, byte[]>();

            expectedValues.Add("A", Encoding.ASCII.GetBytes("1"));
            expectedValues.Add("B", Encoding.ASCII.GetBytes("2"));

            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.QueueableOperation.SetHashValues(key, expectedValues);

            this.CompleteOperation();

            IReadOnlyDictionary<string, byte[]> actualValues = this.Provider.GetHashAsBinary(key);

            Assert.IsNotNull(actualValues);

            Assert.AreEqual(2, actualValues.Count);

            Assert.AreEqual("1", Encoding.ASCII.GetString(actualValues["A"]));
            Assert.AreEqual("2", Encoding.ASCII.GetString(actualValues["B"]));
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void SetHashValuesBinaryDisposed()
        {
            this.QueueableOperation.Dispose();

            this.QueueableOperation.SetHashValues("Temp", new Dictionary<string, byte[]>());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetHashValuesBinaryNullKey()
        {
            this.QueueableOperation.SetHashValues(null, new Dictionary<string, byte[]>());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetHashValuesBinaryEmptyKey()
        {
            this.QueueableOperation.SetHashValues(string.Empty, new Dictionary<string, byte[]>());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetHashValuesBinaryNullValues()
        {
            this.QueueableOperation.SetHashValues("Temp", default(IReadOnlyDictionary<string, byte[]>));
        }

        #endregion SetHashValues (Binary)

        #region SetHashValue (Text)

        [TestMethod]
        public void SetHashTextValue()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.QueueableOperation.SetHashValue(key, "Hello", "World");

            this.CompleteOperation();

            Assert.AreEqual("World", this.Provider.GetHashTextValue(key, "Hello"));
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void SetHashTextValueDisposed()
        {
            this.QueueableOperation.Dispose();
            this.QueueableOperation.SetHashValue("Key", "Field", "Value");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetHashTextValueNullKey()
        {
            this.QueueableOperation.SetHashValue(null, "Field", "Value");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetHashTextalueEmptyKey()
        {
            this.QueueableOperation.SetHashValue(string.Empty, "Field", "Value");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetHashTextValueNullField()
        {
            this.QueueableOperation.SetHashValue("Key", null, "Value");
        }

        [TestMethod]
        public void SetHashTextValueEmptyField()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.QueueableOperation.SetHashValue(key, string.Empty, "Value");

            this.CompleteOperation();

            Assert.AreEqual("Value", this.Provider.GetHashTextValue(key, string.Empty));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetHashTextValueNullValue()
        {
            this.QueueableOperation.SetHashValue("Key", "Field", default(string));
        }

        #endregion SetHashValue (Text)

        #region SetHashValue (Binary)

        [TestMethod]
        public void SetHashBinaryValue()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.QueueableOperation.SetHashValue(key, "Hello", Encoding.ASCII.GetBytes("World"));

            this.CompleteOperation();

            byte[] content = this.Provider.GetHashBinaryValue(key, "Hello");

            Assert.AreEqual("World", Encoding.ASCII.GetString(content));
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void SetHashBinaryValueDisposed()
        {
            this.QueueableOperation.Dispose();
            this.QueueableOperation.SetHashValue("Key", "Field", new byte[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetHashBinaryValueNullKey()
        {
            this.QueueableOperation.SetHashValue(null, "Field", new byte[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetHashBinaryValueEmptyKey()
        {
            this.QueueableOperation.SetHashValue(string.Empty, "Field", new byte[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetHashBinaryValueNullField()
        {
            this.QueueableOperation.SetHashValue("Key", null, new byte[0]);
        }

        [TestMethod]
        public void SetHashBinaryValueEmptyField()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.QueueableOperation.SetHashValue(key, string.Empty, Encoding.ASCII.GetBytes("Value"));

            this.CompleteOperation();

            Assert.AreEqual("Value", Encoding.ASCII.GetString(this.Provider.GetHashBinaryValue(key, string.Empty)));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetHashBinaryValueNullValue()
        {
            this.QueueableOperation.SetHashValue("Key", "Field", default(byte[]));
        }

        #endregion SetHashValue (Binary)

        #region RemoveFromHash

        [TestMethod]
        public void RemoveFromHash()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.Provider.SetHashValue(key, "A", "1");
            this.Provider.SetHashValue(key, "B", "2");
            this.Provider.SetHashValue(key, "C", "3");
            this.Provider.SetHashValue(key, "D", "4");

            this.QueueableOperation.RemoveFromHash(key, "A", "C");

            this.CompleteOperation();

            var result = this.Provider.GetHashAsText(key);

            Assert.AreEqual(2, result.Count());

            Assert.IsFalse(result.ContainsKey("A"));
            Assert.IsTrue(result.ContainsKey("B"));
            Assert.IsFalse(result.ContainsKey("C"));
            Assert.IsTrue(result.ContainsKey("D"));
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void RemoveFromHashDisposed()
        {
            this.QueueableOperation.Dispose();

            this.QueueableOperation.RemoveFromHash("Temp", "A");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveFromHashNullKey()
        {
            this.QueueableOperation.RemoveFromHash(null, "A");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveFromHashEmptyKey()
        {
            this.QueueableOperation.RemoveFromHash(string.Empty, "A");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveFromHashNullFields()
        {
            this.QueueableOperation.RemoveFromHash("Temp", null);
        }

        [TestMethod]
        public void RemoveFromHashNoFields()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.Provider.SetHashValue(key, "A", "1");

            this.QueueableOperation.RemoveFromHash(key);

            this.CompleteOperation();

            var result = this.Provider.GetHashAsText(key);

            Assert.AreEqual(1, result.Count());

            Assert.IsTrue(result.ContainsKey("A"));
        }

        #endregion RemoveFromHash

        [TestMethod]
        public void CompleteEmptyOperation()
        {
            this.CompleteOperation();
        }

        protected abstract void CompleteOperation();

        protected abstract IRedisProvider CreateRedisProvider();

        protected abstract IRedisQueueableOperation CreateQueueableOperation();
    }
}