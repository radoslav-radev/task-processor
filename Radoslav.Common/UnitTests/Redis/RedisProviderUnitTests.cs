using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Redis;

namespace Radoslav.Common.UnitTests
{
    [TestClass]
    public abstract class RedisProviderUnitTests
    {
        #region Propertes & Initialization

        private const string TempKeyPrefix = "Radoslav$TaskProcessor$UnitTests$";

        public TestContext TestContext { get; set; }

        protected IRedisProvider RedisProvider
        {
            get
            {
                return (IRedisProvider)this.TestContext.Properties["RedisProvider"];
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            this.TestContext.Properties.Add("RedisProvider", this.CreateRedisProvider());
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this.RedisProvider.Dispose();
        }

        #endregion Propertes & Initialization

        #region DateTime

        [TestMethod]
        public void GetServerDateTimeUtc()
        {
            DateTime redisTime = this.RedisProvider.GetServerDateTimeUtc();

            long distanceInTicks = Math.Abs(DateTime.UtcNow.Ticks - redisTime.Ticks);

            Assert.IsTrue(distanceInTicks < TimeSpan.FromSeconds(1).Ticks);

            Assert.AreEqual(DateTimeKind.Utc, redisTime.Kind);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void GetServerDateTimeUtcDisposed()
        {
            this.RedisProvider.Dispose();

            this.RedisProvider.GetServerDateTimeUtc();
        }

        #endregion DateTime

        #region Keys

        [TestMethod]
        public void SearchKeys()
        {
            this.RedisProvider.SetValue(TempKeyPrefix + "SearchKeys$Value", "Value");
            this.RedisProvider.AddToList(TempKeyPrefix + "SearchKeys$List", "List");
            this.RedisProvider.AddToSet(TempKeyPrefix + "SearchKeys$Set", "Set");
            this.RedisProvider.SetHashValues(TempKeyPrefix + "SearchKeys$Hash", new Dictionary<string, string>() { { "Key", "Value" } });

            this.RedisProvider.SetValue(TempKeyPrefix + "SearchKeysDummy$Value", "Value");
            this.RedisProvider.AddToList(TempKeyPrefix + "SearchKeysDummy$List", "List");
            this.RedisProvider.AddToSet(TempKeyPrefix + "SearchKeysDummy$Set", "Set");
            this.RedisProvider.SetHashValues(TempKeyPrefix + "SearchKeysDummy$Hash", new Dictionary<string, string>() { { "Key", "Value" } });

            string[] foundKeys = this.RedisProvider.SearchKeys(TempKeyPrefix + "SearchKeys$*").ToArray();

            CollectionAssert.Contains(foundKeys, TempKeyPrefix + "SearchKeys$Value");
            CollectionAssert.Contains(foundKeys, TempKeyPrefix + "SearchKeys$List");
            CollectionAssert.Contains(foundKeys, TempKeyPrefix + "SearchKeys$Set");
            CollectionAssert.Contains(foundKeys, TempKeyPrefix + "SearchKeys$Hash");

            CollectionAssert.DoesNotContain(foundKeys, TempKeyPrefix + "SearchKeysDummy$Value");
            CollectionAssert.DoesNotContain(foundKeys, TempKeyPrefix + "SearchKeysDummy$List");
            CollectionAssert.DoesNotContain(foundKeys, TempKeyPrefix + "SearchKeysDummy$Set");
            CollectionAssert.DoesNotContain(foundKeys, TempKeyPrefix + "SearchKeysDummy$Hash");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SearchKeysNullPattern()
        {
            this.RedisProvider.SearchKeys(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SearchKeysEmptyPattern()
        {
            this.RedisProvider.SearchKeys(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void SearchKeysDisposed()
        {
            this.RedisProvider.Dispose();

            this.RedisProvider.SearchKeys("Pattern");
        }

        [TestMethod]
        public void ContainsKey()
        {
            this.RedisProvider.SetValue(TempKeyPrefix + "ContainsKey$Value", "Value");
            this.RedisProvider.AddToList(TempKeyPrefix + "ContainsKey$List", "List");
            this.RedisProvider.AddToSet(TempKeyPrefix + "ContainsKey$Set", "Set");
            this.RedisProvider.SetHashValues(TempKeyPrefix + "ContainsKey$Hash", new Dictionary<string, string>() { { "Key", "Value" } });

            Assert.IsTrue(this.RedisProvider.ContainsKey(TempKeyPrefix + "ContainsKey$Value"));
            Assert.IsTrue(this.RedisProvider.ContainsKey(TempKeyPrefix + "ContainsKey$List"));
            Assert.IsTrue(this.RedisProvider.ContainsKey(TempKeyPrefix + "ContainsKey$Set"));
            Assert.IsTrue(this.RedisProvider.ContainsKey(TempKeyPrefix + "ContainsKey$Hash"));

            Assert.IsFalse(this.RedisProvider.ContainsKey(TempKeyPrefix + "ContainsKey$Value1"));
            Assert.IsFalse(this.RedisProvider.ContainsKey(TempKeyPrefix + "ContainsKey$List1"));
            Assert.IsFalse(this.RedisProvider.ContainsKey(TempKeyPrefix + "ContainsKey$Set1"));
            Assert.IsFalse(this.RedisProvider.ContainsKey(TempKeyPrefix + "ContainsKey$Hash1"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ContainsKeyNullKey()
        {
            this.RedisProvider.ContainsKey(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ContainsKeyEmptyKey()
        {
            this.RedisProvider.ContainsKey(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void ContainsKeyDisposed()
        {
            this.RedisProvider.Dispose();

            this.RedisProvider.ContainsKey("Key");
        }

        [TestMethod]
        public void GetKeyType()
        {
            this.RedisProvider.SetValue(TempKeyPrefix + "GetKeyType$Value", "Value");
            this.RedisProvider.AddToList(TempKeyPrefix + "GetKeyType$List", "List");
            this.RedisProvider.AddToSet(TempKeyPrefix + "GetKeyType$Set", "Set");
            this.RedisProvider.SetHashValues(TempKeyPrefix + "GetKeyType$Hash", new Dictionary<string, string>() { { "Key", "Value" } });

            Assert.AreEqual(RedisKeyType.String, this.RedisProvider.GetKeyType(TempKeyPrefix + "GetKeyType$Value"));
            Assert.AreEqual(RedisKeyType.List, this.RedisProvider.GetKeyType(TempKeyPrefix + "GetKeyType$List"));
            Assert.AreEqual(RedisKeyType.Set, this.RedisProvider.GetKeyType(TempKeyPrefix + "GetKeyType$Set"));
            Assert.AreEqual(RedisKeyType.Hash, this.RedisProvider.GetKeyType(TempKeyPrefix + "GetKeyType$Hash"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetKeyTypeNullKey()
        {
            this.RedisProvider.GetKeyType(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetKeyTypeEmptyKey()
        {
            this.RedisProvider.GetKeyType(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void GetKeyTypeDisposed()
        {
            this.RedisProvider.Dispose();

            this.RedisProvider.GetKeyType("Key");
        }

        [TestMethod]
        public void GetKeyTypeKeyNotExists()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            Assert.AreEqual(RedisKeyType.None, this.RedisProvider.GetKeyType(key));
        }

        [TestMethod]
        public void GetTextValue()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            string value = Guid.NewGuid().ToString();

            this.RedisProvider.SetValue(key, value);

            Assert.AreEqual(value, this.RedisProvider.GetTextValue(key));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetTextValueNullKey()
        {
            this.RedisProvider.GetTextValue(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetTextValueEmptyKey()
        {
            this.RedisProvider.GetTextValue(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void GetTextValueDisposed()
        {
            this.RedisProvider.Dispose();

            this.RedisProvider.GetTextValue("Key");
        }

        [TestMethod]
        [ExpectedException(typeof(RedisException))]
        public void GetTextValueWrongKeyType()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.AddToList(key, string.Empty);

            this.RedisProvider.GetTextValue(key);
        }

        [TestMethod]
        public void GetTextValueKeyNotExists()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            Assert.IsTrue(string.IsNullOrEmpty(this.RedisProvider.GetTextValue(key)));
        }

        [TestMethod]
        public void GetBinaryValue()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            Guid value = Guid.NewGuid();

            this.RedisProvider.SetValue(key, value.ToByteArray());

            Assert.AreEqual(value, new Guid(this.RedisProvider.GetBinaryValue(key)));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetBinaryValueNullKey()
        {
            this.RedisProvider.GetBinaryValue(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetBinaryValueEmptyKey()
        {
            this.RedisProvider.GetBinaryValue(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void GetBinaryValueDisposed()
        {
            this.RedisProvider.Dispose();

            this.RedisProvider.GetBinaryValue("Key");
        }

        [TestMethod]
        [ExpectedException(typeof(RedisException))]
        public void GetBinaryValueWrongKeyType()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.AddToList(key, string.Empty);

            this.RedisProvider.GetBinaryValue(key);
        }

        [TestMethod]
        public void GetBinaryValueKeyNotExists()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            Assert.IsNull(this.RedisProvider.GetBinaryValue(key));
        }

        [TestMethod]
        public void SetValue()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetValue(key, "Value 1");

            Assert.AreEqual("Value 1", this.RedisProvider.GetTextValue(key));
        }

        [TestMethod]
        public void SetValueOverrideValue()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetValue(key, "Value 1");

            Assert.AreEqual("Value 1", this.RedisProvider.GetTextValue(key));

            this.RedisProvider.SetValue(key, "Value 2");

            Assert.AreEqual("Value 2", this.RedisProvider.GetTextValue(key));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetValueNullKey()
        {
            this.RedisProvider.SetValue(null, "Value");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetValueEmptyKey()
        {
            this.RedisProvider.SetValue(string.Empty, "Value");
        }

        [TestMethod]
        public void SetValueNullValue()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetValue(key, default(string));

            Assert.IsTrue(string.IsNullOrEmpty(this.RedisProvider.GetTextValue(key)));
        }

        [TestMethod]
        public void SetValueEmptyValue()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetValue(key, string.Empty);

            Assert.IsTrue(string.IsNullOrEmpty(this.RedisProvider.GetTextValue(key)));
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void SetValueDisposed()
        {
            this.RedisProvider.Dispose();

            this.RedisProvider.SetValue("Key", "Value");
        }

        [TestMethod]
        public void SetBinaryValue()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetValue(key, Encoding.UTF8.GetBytes("Hello World"));

            Assert.AreEqual("Hello World", Encoding.UTF8.GetString(this.RedisProvider.GetBinaryValue(key)));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetBinaryValueNullKey()
        {
            this.RedisProvider.SetValue(null, new byte[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetBinaryValueEmptyKey()
        {
            this.RedisProvider.SetValue(string.Empty, new byte[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetBinaryValueNullValue()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetValue(key, default(byte[]));

            Assert.IsTrue(string.IsNullOrEmpty(this.RedisProvider.GetTextValue(key)));
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void SetBinaryValueDisposed()
        {
            this.RedisProvider.Dispose();

            this.RedisProvider.SetValue("Key", new byte[0]);
        }

        [TestMethod]
        public void SetValueIfNotExists()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            Assert.IsFalse(this.RedisProvider.ContainsKey(key));

            this.RedisProvider.SetValueIfNotExists(key, "Value 1");

            Assert.AreEqual("Value 1", this.RedisProvider.GetTextValue(key));

            this.RedisProvider.SetValueIfNotExists(key, "Value 2");

            Assert.AreEqual("Value 1", this.RedisProvider.GetTextValue(key));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetValueIfNotExistsNullKey()
        {
            this.RedisProvider.SetValueIfNotExists(null, "Value");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetValueIfNotExistsEmptyKey()
        {
            this.RedisProvider.SetValueIfNotExists(string.Empty, "Value");
        }

        [TestMethod]
        public void SetValueIfNotExistsNullValue()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            Assert.IsFalse(this.RedisProvider.ContainsKey(key));

            this.RedisProvider.SetValueIfNotExists(key, null);

            Assert.IsTrue(string.IsNullOrEmpty(this.RedisProvider.GetTextValue(key)));

            this.RedisProvider.SetValueIfNotExists(key, "Value 2");

            Assert.IsTrue(string.IsNullOrEmpty(this.RedisProvider.GetTextValue(key)));
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void SetValueIfNotExistsDisposed()
        {
            this.RedisProvider.Dispose();

            this.RedisProvider.SetValueIfNotExists("Key", "Value");
        }

        [TestMethod]
        public void ExpireKeyIn()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetValue(key, "Value");

            Assert.IsTrue(this.RedisProvider.ExpireKeyIn(key, TimeSpan.FromSeconds(1)));

            Thread.Sleep(TimeSpan.FromSeconds(0.75));

            Assert.IsTrue(this.RedisProvider.ContainsKey(key));

            Assert.IsTrue(this.RedisProvider.ExpireKeyIn(key, TimeSpan.FromSeconds(1)));

            Thread.Sleep(TimeSpan.FromSeconds(0.75));

            Assert.IsTrue(this.RedisProvider.ContainsKey(key));

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            Assert.IsFalse(this.RedisProvider.ContainsKey(key));

            Assert.IsFalse(this.RedisProvider.ExpireKeyIn(key, TimeSpan.FromSeconds(1)));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExpireKeyInNullKey()
        {
            this.RedisProvider.ExpireKeyIn(null, TimeSpan.FromMinutes(1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExpireKeyInEmptyKey()
        {
            this.RedisProvider.ExpireKeyIn(string.Empty, TimeSpan.FromMinutes(1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ExpireKeyInZeroTimeout()
        {
            this.RedisProvider.ExpireKeyIn("Key", TimeSpan.Zero);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ExpireKeyInNegativeTimeout()
        {
            this.RedisProvider.ExpireKeyIn("Key", TimeSpan.FromMinutes(-1));
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void ExpireKeyInDisposed()
        {
            this.RedisProvider.Dispose();

            this.RedisProvider.ExpireKeyIn("Key", TimeSpan.FromMinutes(1));
        }

        [TestMethod]
        public void ExpireKeyInKeyNotExists()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            Assert.IsFalse(this.RedisProvider.ExpireKeyIn(key, TimeSpan.FromMinutes(1)));
        }

        [TestMethod]
        public void IncrementValue()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetValue(key, "5");

            Assert.AreEqual(6, this.RedisProvider.IncrementValue(key));
            Assert.AreEqual(7, this.RedisProvider.IncrementValue(key));
            Assert.AreEqual(8, this.RedisProvider.IncrementValue(key));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IncrementValueNullKey()
        {
            this.RedisProvider.IncrementValue(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IncrementValueEmptyKey()
        {
            this.RedisProvider.IncrementValue(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void IncrementValueAfterDispose()
        {
            this.RedisProvider.Dispose();

            this.RedisProvider.IncrementValue("Key");
        }

        [TestMethod]
        public void IncrementValueNullValue()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetValue(key, default(string));

            Assert.AreEqual(1, this.RedisProvider.IncrementValue(key));
        }

        [TestMethod]
        public void IncrementValueEmptyValue()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetValue(key, string.Empty);

            Assert.AreEqual(1, this.RedisProvider.IncrementValue(key));
        }

        [TestMethod]
        [ExpectedException(typeof(RedisException))]
        public void IncrementValueThatIsNotInteger()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetValue(key, "Dummy");

            this.RedisProvider.IncrementValue(key);
        }

        [TestMethod]
        [ExpectedException(typeof(RedisException))]
        public void IncrementValueOfWrongType()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.AddToList(key, string.Empty);

            this.RedisProvider.IncrementValue(key);
        }

        [TestMethod]
        public void IncrementValueKeyNotExists()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            Assert.AreEqual(1, this.RedisProvider.IncrementValue(key));
        }

        [TestMethod]
        public void RemoveKey()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetValue(key, "Value");

            Assert.IsTrue(this.RedisProvider.ContainsKey(key));

            this.RedisProvider.RemoveKey(key);

            Assert.IsFalse(this.RedisProvider.ContainsKey(key));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveKeyNullKey()
        {
            this.RedisProvider.RemoveKey(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveKeyEmptyKey()
        {
            this.RedisProvider.RemoveKey(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void RemoveKeyDisposed()
        {
            this.RedisProvider.Dispose();

            this.RedisProvider.RemoveKey("Key");
        }

        [TestMethod]
        public void RemoveKeyKeyNotExists()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.RemoveKey(key);
        }

        #endregion Keys

        #region Pub/Sub

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CreateSubscriptionAfterDispose()
        {
            this.RedisProvider.Dispose();

            this.RedisProvider.CreateSubscription();
        }

        [TestMethod]
        public void PublishMessage()
        {
            this.RedisProvider.PublishMessage("Hello", "World");
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void PublishMessageAfterDispose()
        {
            this.RedisProvider.Dispose();

            this.RedisProvider.PublishMessage("Hello", "World");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PublishMessageNullChannel()
        {
            this.RedisProvider.PublishMessage(null, "World");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PublishMessageEmptyChannel()
        {
            this.RedisProvider.PublishMessage(string.Empty, "World");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PublishMessageNullMessage()
        {
            this.RedisProvider.PublishMessage("Hello", null);
        }

        [TestMethod]
        public void PublishMessageEmptyMessage()
        {
            this.RedisProvider.PublishMessage("Hello", string.Empty);
        }

        #endregion Pub/Sub

        #region Hash

        [TestMethod]
        public void IncrementValueInHash()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetHashValues(key, new Dictionary<string, string>() { { "Key", "5" } });

            this.RedisProvider.IncrementValueInHash(key, "Key", 10);

            IReadOnlyDictionary<string, string> values = this.RedisProvider.GetHashAsText(key);

            Assert.AreEqual("15", values["Key"]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IncrementValueInHashNullKey()
        {
            this.RedisProvider.IncrementValueInHash(null, "Field", 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IncrementValueInHashEmptyKey()
        {
            this.RedisProvider.IncrementValueInHash(string.Empty, "Field", 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IncrementValueInHashNullField()
        {
            this.RedisProvider.IncrementValueInHash("Key", null, 1);
        }

        [TestMethod]
        public void IncrementValueInHashEmptyField()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            Assert.AreEqual(1, this.RedisProvider.IncrementValueInHash(key, string.Empty, 1));
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void IncrementValueInHashDisposed()
        {
            this.RedisProvider.Dispose();

            this.RedisProvider.IncrementValueInHash("Key", "Field", 1);
        }

        [TestMethod]
        public void IncrementValueInHashEmptyValue()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetHashValues(key, new Dictionary<string, string>() { { "Field", string.Empty } });

            Assert.AreEqual(1, this.RedisProvider.IncrementValueInHash(key, "Field", 1));
        }

        [TestMethod]
        [ExpectedException(typeof(RedisException))]
        public void IncrementValueInHashThatIsNotInteger()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetHashValues(key, new Dictionary<string, string>() { { "Field", "Value" } });

            this.RedisProvider.IncrementValueInHash(key, "Field", 1);
        }

        [TestMethod]
        [ExpectedException(typeof(RedisException))]
        public void IncrementValueInHashOfWrongType()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetValue(key, string.Empty);

            this.RedisProvider.IncrementValueInHash(key, "Dummy", 1);
        }

        [TestMethod]
        public void IncrementValueInHashKeyNotExists()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            Assert.AreEqual(1, this.RedisProvider.IncrementValueInHash(key, "Key", 1));
        }

        #endregion Hash

        #region Lists

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetListElementNullKey()
        {
            this.RedisProvider.GetListElement(null, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetListElementNegativeIndex()
        {
            this.RedisProvider.GetListElement("Key", -1);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void GetListElementDisposed()
        {
            this.RedisProvider.Dispose();

            this.RedisProvider.GetListElement("Key", 0);
        }

        [TestMethod]
        [ExpectedException(typeof(RedisException))]
        public void GetListElementWrongKeyType()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetValue(key, "Value");

            this.RedisProvider.GetListElement(key, 0);
        }

        [TestMethod]
        public void GetListElement()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.AddToList(key, "A");
            this.RedisProvider.AddToList(key, "B");

            Assert.AreEqual("A", this.RedisProvider.GetListElement(key, 0));
            Assert.AreEqual("B", this.RedisProvider.GetListElement(key, 1));

            Assert.IsNull(this.RedisProvider.GetListElement(key, 2));
        }

        [TestMethod]
        public void GetListRange()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            Enumerable.Range(0, 10).ForEach(false, i => this.RedisProvider.AddToList(key, i.ToString(CultureInfo.InvariantCulture)));

            string[] result = this.RedisProvider.GetListRange(key, 0, 5).ToArray();

            Enumerable.Range(0, 5).ForEach(false, i => Assert.AreEqual(i.ToString(CultureInfo.InvariantCulture), result[i]));
        }

        [TestMethod]
        public void GetListRangeEndIndexGreaterThanListSize()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            Enumerable.Range(0, 5).ForEach(false, i => this.RedisProvider.AddToList(key, i.ToString(CultureInfo.InvariantCulture)));

            string[] result = this.RedisProvider.GetListRange(key, 0, 10).ToArray();

            Enumerable.Range(0, 5).ForEach(false, i => Assert.AreEqual(i.ToString(CultureInfo.InvariantCulture), result[i]));
        }

        [TestMethod]
        public void GetListRangeStartIndexEqualsEndIndex()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            Enumerable.Range(0, 5).ForEach(false, i => this.RedisProvider.AddToList(key, i.ToString(CultureInfo.InvariantCulture)));

            Assert.AreEqual("1", this.RedisProvider.GetListRange(key, 1, 1).Single());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetListRangeNullKey()
        {
            this.RedisProvider.GetListRange(null, 0, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetListRangeEmptyKey()
        {
            this.RedisProvider.GetListRange(string.Empty, 0, 1);
        }

        [TestMethod]
        public void GetListRangeKeyNotExists()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            Assert.IsFalse(this.RedisProvider.GetListRange(key, 0, 1).Any());
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void GetListRangeDisposed()
        {
            this.RedisProvider.Dispose();

            this.RedisProvider.GetListRange("Key", 0, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(RedisException))]
        public void GetListRangeWrongKeyType()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetValue(key, "Value");

            this.RedisProvider.GetListRange(key, 0, 1);
        }

        [TestMethod]
        public void RemoveFromList()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.AddToList(key, "Element 1");
            this.RedisProvider.AddToList(key, "Element 2");

            this.RedisProvider.RemoveFromList(key, "Element 1");

            string[] list = this.RedisProvider.GetListAsText(key).ToArray();

            Assert.AreEqual(1, list.Length);

            Assert.AreEqual("Element 2", list[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveFromListNullKey()
        {
            this.RedisProvider.RemoveFromList(null, "Element");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveFromListEmptyKey()
        {
            this.RedisProvider.RemoveFromList(string.Empty, "Element");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveFromListNullElement()
        {
            this.RedisProvider.RemoveFromList("Key", null);
        }

        [TestMethod]
        public void RemoveFromListEmptyElement()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.AddToList(key, string.Empty);
            this.RedisProvider.RemoveFromList(key, string.Empty);

            Assert.IsFalse(this.RedisProvider.GetListAsText(key).Any(e => string.IsNullOrEmpty(e)));
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void RemoveFromListDisposed()
        {
            this.RedisProvider.Dispose();

            this.RedisProvider.RemoveFromList("Key", "Element");
        }

        [TestMethod]
        [ExpectedException(typeof(RedisException))]
        public void RemoveFromListWrongKeyType()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetValue(key, "Value");

            this.RedisProvider.RemoveFromList(key, "Element");
        }

        #endregion Lists

        #region Sets

        [TestMethod]
        public void GetSet()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            int count = 3;

            Enumerable.Range(0, count).ForEach(false, i => this.RedisProvider.AddToSet(key, i.ToString(CultureInfo.InvariantCulture)));

            string[] set = this.RedisProvider.GetSet(key).ToArray();

            Enumerable.Range(0, count).ForEach(false, i => Assert.AreEqual(i.ToString(CultureInfo.InvariantCulture), set[i]));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetSetNullKey()
        {
            this.RedisProvider.GetSet(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetSetEmptyKey()
        {
            this.RedisProvider.GetSet(string.Empty);
        }

        [TestMethod]
        public void GetSetKeyNotExists()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            Assert.IsFalse(this.RedisProvider.GetSet(key).Any());
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void GetSetDisposed()
        {
            this.RedisProvider.Dispose();

            this.RedisProvider.GetSet("Key");
        }

        [TestMethod]
        [ExpectedException(typeof(RedisException))]
        public void GetSetWrongKeyType()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetValue(key, "Value");

            this.RedisProvider.GetSet(key);
        }

        [TestMethod]
        public void AddToSet()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.AddToSet(key, "Element");

            Assert.IsTrue(this.RedisProvider.GetSet(key).Any(e => e == "Element"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddToSetNullKey()
        {
            this.RedisProvider.AddToSet(null, "Element");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddToSetEmptyKey()
        {
            this.RedisProvider.AddToSet(string.Empty, "Element");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddToSetNullElement()
        {
            this.RedisProvider.AddToSet("Key", null);
        }

        [TestMethod]
        public void AddToSetEmptyElement()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.AddToSet(key, string.Empty);

            Assert.IsTrue(this.RedisProvider.GetSet(key).Any(e => string.IsNullOrEmpty(e)));
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void AddToSetDisposed()
        {
            this.RedisProvider.Dispose();

            this.RedisProvider.AddToSet("Key", "Element");
        }

        [TestMethod]
        [ExpectedException(typeof(RedisException))]
        public void AddToSetWrongKeyType()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetValue(key, "Value");

            this.RedisProvider.AddToSet(key, "Element");
        }

        [TestMethod]
        public void RemoveFromSet()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.AddToSet(key, "Element 1");
            this.RedisProvider.AddToSet(key, "Element 2");

            this.RedisProvider.RemoveFromSet(key, "Element 1");

            string[] set = this.RedisProvider.GetSet(key).ToArray();

            Assert.AreEqual(1, set.Length);

            Assert.AreEqual("Element 2", set[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveFromSetNullKey()
        {
            this.RedisProvider.RemoveFromSet(null, "Element");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveFromSetEmptyKey()
        {
            this.RedisProvider.RemoveFromSet(string.Empty, "Element");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveFromSetNullElement()
        {
            this.RedisProvider.RemoveFromSet("Key", null);
        }

        [TestMethod]
        public void RemoveFromSetEmptyElement()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.AddToSet(key, string.Empty);
            this.RedisProvider.RemoveFromSet(key, string.Empty);

            Assert.IsFalse(this.RedisProvider.GetSet(key).Any(e => string.IsNullOrEmpty(e)));
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void RemoveFromSetDisposed()
        {
            this.RedisProvider.Dispose();

            this.RedisProvider.RemoveFromSet("Key", "Element");
        }

        [TestMethod]
        [ExpectedException(typeof(RedisException))]
        public void RemoveFromSetWrongKeyType()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetValue(key, "Value");

            this.RedisProvider.RemoveFromSet(key, "Element");
        }

        #endregion Sets

        #region Server

        [TestMethod]
        public void FlushAll()
        {
            // Do not call this.RedisProvider.FlushAll() because it might interfere with other tests running in parallel.
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void FlushAllAfterDispose()
        {
            this.RedisProvider.Dispose();
            this.RedisProvider.FlushAll();
        }

        #endregion Server

        #region Pipeline & Transaction

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CreatePipelineAfterDispose()
        {
            this.RedisProvider.Dispose();

            this.RedisProvider.CreatePipeline();
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CreateTransactionAfterDispose()
        {
            this.RedisProvider.Dispose();

            this.RedisProvider.CreatePipeline();
        }

        #endregion Pipeline & Transaction

        #region Acquire Lock

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void AcquireLockDisposed1()
        {
            this.RedisProvider.Dispose();

            this.RedisProvider.AcquireLock("Key");
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void AcquireLockDisposed2()
        {
            this.RedisProvider.Dispose();

            this.RedisProvider.AcquireLock("Key", TimeSpan.MaxValue);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AcquireLockNullKey1()
        {
            this.RedisProvider.AcquireLock(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AcquireLockNullKey2()
        {
            this.RedisProvider.AcquireLock(null, TimeSpan.MaxValue);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AcquireLockEmptyKey1()
        {
            this.RedisProvider.AcquireLock(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AcquireLockEmptyKey2()
        {
            this.RedisProvider.AcquireLock(string.Empty, TimeSpan.MaxValue);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AcquireLockNegativeTimeout()
        {
            using (this.RedisProvider.AcquireLock("Key", TimeSpan.FromSeconds(-1)))
            {
            }
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void AcquireLock()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            ThreadPool.QueueUserWorkItem(state => this.RedisProvider.AcquireLock(key));

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            using (this.RedisProvider.AcquireLock(key, TimeSpan.FromSeconds(1)))
            {
            }
        }

        [TestMethod]
        public void ReleaseLockAfterDispose()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            using (this.RedisProvider.AcquireLock(key))
            {
            }

            using (this.RedisProvider.AcquireLock(key))
            {
            }
        }

        [TestMethod]
        public void ReleaseLockAfterTimeout()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            ThreadPool.QueueUserWorkItem(state => this.RedisProvider.AcquireLock(key, TimeSpan.FromSeconds(1)));

            Thread.Sleep(TimeSpan.FromSeconds(1.1));

            using (this.RedisProvider.AcquireLock(key))
            {
            }
        }

        #endregion Acquire Lock

        #region Scripts

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void ExecuteScriptDisposed()
        {
            this.RedisProvider.Dispose();

            this.RedisProvider.ExecuteLuaScript<string>("dummy");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExecuteNullScript()
        {
            this.RedisProvider.ExecuteLuaScript<string>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExecuteNullParameters()
        {
            this.RedisProvider.ExecuteLuaScript<string>("dummy", null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExecuteEmptyScript()
        {
            this.RedisProvider.ExecuteLuaScript<string>(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(RedisException))]
        public void ExecuteInvalidScript()
        {
            this.RedisProvider.ExecuteLuaScript<string>("dummy");
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException), AllowDerivedTypes = true)]
        public void ExecuteScriptTypeNotSupported()
        {
            this.RedisProvider.ExecuteLuaScript<Version>("return 'Hello World'");
        }

        [TestMethod]
        public void ExecuteScriptAsHash()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetHashValues(key, new Dictionary<string, string>()
            {
                { "A", "1" },
                { "B", "2" }
            });

            var result = this.RedisProvider.ExecuteLuaScript<IReadOnlyDictionary<string, string>>("return redis.call('HGETALL', KEYS[1])", key);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("1", result["A"]);
            Assert.AreEqual("2", result["B"]);
        }

        #endregion Scripts

        #region GetHashTextValue

        [TestMethod]
        public void GetHashTextValue()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetHashValue(key, "Hello", "World");

            Assert.AreEqual("World", this.RedisProvider.GetHashTextValue(key, "Hello"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetHashTextValueNullKey()
        {
            this.RedisProvider.GetHashTextValue(null, "Field");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetHashTextValueEmptyKey()
        {
            this.RedisProvider.GetHashTextValue(string.Empty, "Field");
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void GetHashTextValueDisposed()
        {
            this.RedisProvider.Dispose();

            this.RedisProvider.GetHashTextValue("Key", "Field");
        }

        [TestMethod]
        public void GetHashTextValueNotExistentKey()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            Assert.IsNull(this.RedisProvider.GetHashTextValue(key, "Field"));
        }

        [TestMethod]
        public void GetHashTextValueNotExistentField()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetHashValue(key, "Dummy", "Value");

            Assert.IsNull(this.RedisProvider.GetHashTextValue(key, "Field"));
        }

        [TestMethod]
        [ExpectedException(typeof(RedisException))]
        public void GetHashTextValueWrongKeyType()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetValue(key, string.Empty);

            Assert.IsNull(this.RedisProvider.GetHashTextValue(key, "Field"));
        }

        #endregion GetHashTextValue

        #region GetHashBinaryValue

        [TestMethod]
        public void GetHashBinaryValue()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetHashValue(key, "Hello", Encoding.ASCII.GetBytes("World"));

            Assert.AreEqual("World", Encoding.ASCII.GetString(this.RedisProvider.GetHashBinaryValue(key, "Hello")));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetHashBinaryValueNullKey()
        {
            this.RedisProvider.GetHashBinaryValue(null, "Field");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetHashBinaryValueEmptyKey()
        {
            this.RedisProvider.GetHashBinaryValue(string.Empty, "Field");
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void GetHashBinaryValueDisposed()
        {
            this.RedisProvider.Dispose();

            this.RedisProvider.GetHashBinaryValue("Key", "Field");
        }

        [TestMethod]
        public void GetHashBinaryValueNotExistentKey()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            Assert.IsNull(this.RedisProvider.GetHashBinaryValue(key, "Field"));
        }

        [TestMethod]
        public void GetHashBinaryValueNotExistentField()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetHashValue(key, "Dummy", new byte[0]);

            Assert.IsNull(this.RedisProvider.GetHashBinaryValue(key, "Field"));
        }

        [TestMethod]
        [ExpectedException(typeof(RedisException))]
        public void GetHashBinaryValueWrongKeyType()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetValue(key, string.Empty);

            Assert.IsNull(this.RedisProvider.GetHashBinaryValue(key, "Field"));
        }

        #endregion GetHashBinaryValue

        #region SetHashValue (Text)

        [TestMethod]
        public void SetHashTextValue()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetHashValue(key, "Field", "Hello");

            Assert.AreEqual("Hello", this.RedisProvider.GetHashTextValue(key, "Field"));
        }

        [TestMethod]
        public void SetHashTextValueOverrideValue()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetHashValue(key, "Field", "Hello");
            this.RedisProvider.SetHashValue(key, "Field", "World");

            Assert.AreEqual("World", this.RedisProvider.GetHashTextValue(key, "Field"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetHashTextValueNullKey()
        {
            this.RedisProvider.SetHashValue(null, "Field", "Value");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetHashTextValueEmptyKey()
        {
            this.RedisProvider.SetHashValue(string.Empty, "Field", "Value");
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void SetHashTextValueDisposed()
        {
            this.RedisProvider.Dispose();

            this.RedisProvider.SetHashValue("Key", "Field", "Value");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetHashTextValueNullField()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetHashValue(key, null, "Value");
        }

        [TestMethod]
        public void SetHashTextValueEmptyField()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetHashValue(key, string.Empty, "Value");

            Assert.AreEqual("Value", this.RedisProvider.GetHashTextValue(key, string.Empty));
        }

        [TestMethod]
        public void SetHashTextValueEmptyValue()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetHashValue(key, "Field", string.Empty);

            Assert.AreEqual(string.Empty, this.RedisProvider.GetHashTextValue(key, "Field"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetHashTextValueNullValue()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetHashValue(key, "Field", default(string));
        }

        #endregion SetHashValue (Text)

        #region SetHashValue (Binary)

        [TestMethod]
        public void SetHashBinaryValue()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetHashValue(key, "Field", Encoding.ASCII.GetBytes("Hello"));

            Assert.AreEqual("Hello", Encoding.ASCII.GetString(this.RedisProvider.GetHashBinaryValue(key, "Field")));
        }

        [TestMethod]
        public void SetHashBinaryValueOverrideValue()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetHashValue(key, "Field", "Hello");
            this.RedisProvider.SetHashValue(key, "Field", "World");

            Assert.AreEqual("World", Encoding.ASCII.GetString(this.RedisProvider.GetHashBinaryValue(key, "Field")));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetHashBinaryValueNullKey()
        {
            this.RedisProvider.SetHashValue(null, "Field", new byte[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetHashBinaryValueEmptyKey()
        {
            this.RedisProvider.SetHashValue(string.Empty, "Field", new byte[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void SetHashBinaryValueDisposed()
        {
            this.RedisProvider.Dispose();

            this.RedisProvider.SetHashValue("Key", "Field", new byte[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetHashBinaryValueNullField()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetHashValue(key, null, new byte[0]);
        }

        [TestMethod]
        public void SetHashBinaryValueEmptyField()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetHashValue(key, string.Empty, Encoding.ASCII.GetBytes("Value"));

            Assert.AreEqual("Value", Encoding.ASCII.GetString(this.RedisProvider.GetHashBinaryValue(key, string.Empty)));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetHashBinaryValueNullValue()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetHashValue(key, "Field", default(byte[]));
        }

        #endregion SetHashValue (Binary)

        #region RemoveFromHash

        [TestMethod]
        public void RemoveFromHash()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetHashValues(key, new Dictionary<string, string>()
            {
                { "Key 1", "Value 1" },
                { "Key 2", "Value 2" },
                { "Key 3", "Value 3" },
                { "Key 4", "Value 4" }
            });

            this.RedisProvider.RemoveFromHash(key, "Key 1", "Key 2");

            IReadOnlyDictionary<string, string> values = this.RedisProvider.GetHashAsText(key);

            Assert.AreEqual(2, values.Count);

            Assert.IsFalse(values.ContainsKey("Key 1"));
            Assert.IsFalse(values.ContainsKey("Key 2"));
            Assert.IsTrue(values.ContainsKey("Key 3"));
            Assert.IsTrue(values.ContainsKey("Key 4"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveFromHashNullKey()
        {
            this.RedisProvider.RemoveFromHash(null, "Field");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveFromHashEmptyKey()
        {
            this.RedisProvider.RemoveFromHash(string.Empty, "Field");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveFromHashNullFields()
        {
            this.RedisProvider.RemoveFromHash("Key", null);
        }

        [TestMethod]
        public void RemoveFromHashNoFields()
        {
            this.RedisProvider.RemoveFromHash("Key");
        }

        [TestMethod]
        public void RemoveFromHashEmptyField()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetHashValues(key, new Dictionary<string, string>() { { string.Empty, "Value" } });

            this.RedisProvider.RemoveFromHash(key, string.Empty);

            Assert.AreEqual(0, this.RedisProvider.GetHashAsText(key).Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void RemoveFromHashDisposed()
        {
            this.RedisProvider.Dispose();

            this.RedisProvider.RemoveFromHash("Key", "Field");
        }

        [TestMethod]
        public void RemoveFromHashKeyNotExists()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.RemoveFromHash(key, "Key 1");
        }

        [TestMethod]
        [ExpectedException(typeof(RedisException))]
        public void RemoveFromHashWrongKeyType()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetValue(key, "Value");

            this.RedisProvider.RemoveFromHash(key, "Field");
        }

        #endregion RemoveFromHash

        #region GetListAsText

        [TestMethod]
        public void GetListAsText()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            int count = 3;

            Enumerable.Range(0, count).ForEach(false, i => this.RedisProvider.AddToList(key, i.ToString(CultureInfo.InvariantCulture)));

            string[] list = this.RedisProvider.GetListAsText(key).ToArray();

            Enumerable.Range(0, count).ForEach(false, i => Assert.AreEqual(i.ToString(CultureInfo.InvariantCulture), list[i]));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetListAsTextNullKey()
        {
            this.RedisProvider.GetListAsText(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetListAsTextEmptyKey()
        {
            this.RedisProvider.GetListAsText(string.Empty);
        }

        [TestMethod]
        public void GetListAsTextKeyNotExists()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            Assert.IsFalse(this.RedisProvider.GetListAsText(key).Any());
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void GetListAsTextDisposed()
        {
            this.RedisProvider.Dispose();

            this.RedisProvider.GetListAsText("Key");
        }

        [TestMethod]
        [ExpectedException(typeof(RedisException))]
        public void GetListAsTextWrongKeyType()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetValue(key, "Value");

            this.RedisProvider.GetListAsText(key);
        }

        #endregion GetListAsText

        #region GetListAsBinary

        [TestMethod]
        public void GetListAsBinary()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            int count = 3;

            Enumerable.Range(0, count).ForEach(false, i => this.RedisProvider.AddToList(key, Encoding.ASCII.GetBytes(i.ToString(CultureInfo.InvariantCulture))));

            byte[][] list = this.RedisProvider.GetListAsBinary(key).ToArray();

            Enumerable.Range(0, count).ForEach(false, i => Assert.AreEqual(i.ToString(CultureInfo.InvariantCulture), Encoding.ASCII.GetString(list[i])));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetListAsBinaryNullKey()
        {
            this.RedisProvider.GetListAsBinary(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetListAsBinaryEmptyKey()
        {
            this.RedisProvider.GetListAsBinary(string.Empty);
        }

        [TestMethod]
        public void GetListAsBinaryKeyNotExists()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            Assert.IsFalse(this.RedisProvider.GetListAsBinary(key).Any());
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void GetListAsBinaryDisposed()
        {
            this.RedisProvider.Dispose();

            this.RedisProvider.GetListAsBinary("Key");
        }

        [TestMethod]
        [ExpectedException(typeof(RedisException))]
        public void GetListAsBinaryWrongKeyType()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetValue(key, "Value");

            this.RedisProvider.GetListAsBinary(key);
        }

        #endregion GetListAsBinary

        #region AddToList (Text)

        [TestMethod]
        public void AddToListText()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.AddToList(key, "Element");

            Assert.IsTrue(this.RedisProvider.GetListAsText(key).Any(e => e == "Element"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddToListTextNullKey()
        {
            this.RedisProvider.AddToList(null, "Element");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddToListTextEmptyKey()
        {
            this.RedisProvider.AddToList(string.Empty, "Element");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddToListTextNullElement()
        {
            this.RedisProvider.AddToList("Key", default(string));
        }

        [TestMethod]
        public void AddToListTextEmptyElement()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.AddToList(key, string.Empty);

            Assert.IsTrue(this.RedisProvider.GetListAsText(key).Any(e => string.IsNullOrEmpty(e)));
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void AddToListTextDisposed()
        {
            this.RedisProvider.Dispose();

            this.RedisProvider.AddToList("Key", "Element");
        }

        [TestMethod]
        [ExpectedException(typeof(RedisException))]
        public void AddToListTextWrongKeyType()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetValue(key, "Value");

            this.RedisProvider.AddToList(key, "Element");
        }

        #endregion AddToList (Text)

        #region AddToList (Binary)

        [TestMethod]
        public void AddToListBinary()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.AddToList(key, Encoding.ASCII.GetBytes("Element"));

            IEnumerable<byte[]> list = this.RedisProvider.GetListAsBinary(key);

            Assert.AreEqual("Element", Encoding.ASCII.GetString(list.Single()));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddToListBinaryNullKey()
        {
            this.RedisProvider.AddToList(null, new byte[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddToListBinaryEmptyKey()
        {
            this.RedisProvider.AddToList(string.Empty, new byte[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddToListBinaryNullElement()
        {
            this.RedisProvider.AddToList("Key", default(byte[]));
        }

        [TestMethod]
        public void AddToListBinaryEmptyElement()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.AddToList(key, new byte[0]);

            IEnumerable<byte[]> list = this.RedisProvider.GetListAsBinary(key);

            Assert.AreEqual(0, list.Single().Length);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void AddToListBinaryDisposed()
        {
            this.RedisProvider.Dispose();

            this.RedisProvider.AddToList("Key", new byte[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(RedisException))]
        public void AddToListBinaryWrongKeyType()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetValue(key, "Value");

            this.RedisProvider.AddToList(key, new byte[0]);
        }

        #endregion AddToList (Binary)

        #region PopFirstListElementAsText

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PopFirstListElementAsTextNullKey()
        {
            this.RedisProvider.PopFirstListElementAsText(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void PopFirstListElementAsTextDisposed()
        {
            this.RedisProvider.Dispose();

            this.RedisProvider.PopFirstListElementAsText("Key");
        }

        [TestMethod]
        [ExpectedException(typeof(RedisException))]
        public void PopFirstListElementAsTextWrongKeyType()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetValue(key, "Value");

            this.RedisProvider.PopFirstListElementAsText(key);
        }

        [TestMethod]
        public void PopFirstListElementAsText()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.AddToList(key, "A");
            this.RedisProvider.AddToList(key, "B");
            this.RedisProvider.AddToList(key, "C");

            Assert.AreEqual("A", this.RedisProvider.PopFirstListElementAsText(key));
            Assert.AreEqual("B", this.RedisProvider.PopFirstListElementAsText(key));
            Assert.AreEqual("C", this.RedisProvider.PopFirstListElementAsText(key));

            Assert.IsNull(this.RedisProvider.PopFirstListElementAsText(key));
        }

        #endregion PopFirstListElementAsText

        #region PopFirstListElementAsBinary

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PopFirstListElementAsBinaryNullKey()
        {
            this.RedisProvider.PopFirstListElementAsBinary(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void PopFirstListElementAsBinaryDisposed()
        {
            this.RedisProvider.Dispose();

            this.RedisProvider.PopFirstListElementAsBinary("Key");
        }

        [TestMethod]
        [ExpectedException(typeof(RedisException))]
        public void PopFirstListElementAsBinaryWrongKeyType()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetValue(key, "Value");

            this.RedisProvider.PopFirstListElementAsBinary(key);
        }

        [TestMethod]
        public void PopFirstListElementAsBinary()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.AddToList(key, Encoding.ASCII.GetBytes("A"));
            this.RedisProvider.AddToList(key, Encoding.ASCII.GetBytes("B"));
            this.RedisProvider.AddToList(key, Encoding.ASCII.GetBytes("C"));

            Assert.AreEqual("A", Encoding.ASCII.GetString(this.RedisProvider.PopFirstListElementAsBinary(key)));
            Assert.AreEqual("B", Encoding.ASCII.GetString(this.RedisProvider.PopFirstListElementAsBinary(key)));
            Assert.AreEqual("C", Encoding.ASCII.GetString(this.RedisProvider.PopFirstListElementAsBinary(key)));

            Assert.IsNull(this.RedisProvider.PopFirstListElementAsBinary(key));
        }

        #endregion PopFirstListElementAsBinary

        #region GetHashAsText

        [TestMethod]
        public void GetHashAsText()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetHashValues(key, new Dictionary<string, string>()
            {
                { "Key 1", "Value 1" },
                { "Key 2", "Value 2" }
            });

            IReadOnlyDictionary<string, string> values = this.RedisProvider.GetHashAsText(key);

            Assert.AreEqual(2, values.Count);

            Assert.AreEqual("Value 1", values["Key 1"]);
            Assert.AreEqual("Value 2", values["Key 2"]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetHashAsTextNullKey()
        {
            this.RedisProvider.GetHashAsText(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetHashAsTextEmptyKey()
        {
            this.RedisProvider.GetHashAsText(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void GetHashAsTextDisposed()
        {
            this.RedisProvider.Dispose();

            this.RedisProvider.GetHashAsText("Key");
        }

        [TestMethod]
        public void GetHashAsTextNotExistentKey()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            IReadOnlyDictionary<string, string> values = this.RedisProvider.GetHashAsText(key);

            Assert.AreEqual(0, values.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(RedisException))]
        public void GetHashAsTextWrongKeyType()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetValue(key, string.Empty);

            this.RedisProvider.GetHashAsText(key);
        }

        #endregion GetHashAsText

        #region GetHashAsBinary

        [TestMethod]
        public void GetHashAsBinary()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetHashValue(key, "Key 1", Encoding.ASCII.GetBytes("Value 1"));
            this.RedisProvider.SetHashValue(key, "Key 2", Encoding.ASCII.GetBytes("Value 2"));

            IReadOnlyDictionary<string, byte[]> values = this.RedisProvider.GetHashAsBinary(key);

            Assert.AreEqual(2, values.Count);

            Assert.AreEqual("Value 1", Encoding.ASCII.GetString(values["Key 1"]));
            Assert.AreEqual("Value 2", Encoding.ASCII.GetString(values["Key 2"]));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetHashAsBinaryullKey()
        {
            this.RedisProvider.GetHashAsBinary(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetHashAsBinaryEmptyKey()
        {
            this.RedisProvider.GetHashAsBinary(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void GetHashAsBinaryDisposed()
        {
            this.RedisProvider.Dispose();

            this.RedisProvider.GetHashAsBinary("Key");
        }

        [TestMethod]
        public void GetHashAsBinaryNotExistentKey()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            IReadOnlyDictionary<string, byte[]> values = this.RedisProvider.GetHashAsBinary(key);

            Assert.AreEqual(0, values.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(RedisException))]
        public void GetHashAsBinaryWrongKeyType()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetValue(key, string.Empty);

            this.RedisProvider.GetHashAsBinary(key);
        }

        #endregion GetHashAsBinary

        #region GetHashValuesAsBinary

        [TestMethod]
        public void GetHashValuesAsBinary()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetHashValue(key, "Key 1", Encoding.ASCII.GetBytes("Value 1"));
            this.RedisProvider.SetHashValue(key, "Key 2", Encoding.ASCII.GetBytes("Value 2"));

            IEnumerable<byte[]> values = this.RedisProvider.GetHashValuesAsBinary(key);

            Assert.AreEqual(2, values.Count());

            Assert.AreEqual("Value 1", Encoding.ASCII.GetString(values.First()));
            Assert.AreEqual("Value 2", Encoding.ASCII.GetString(values.Last()));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetHashValuesAsBinaryullKey()
        {
            this.RedisProvider.GetHashValuesAsBinary(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetHashValuesAsBinaryEmptyKey()
        {
            this.RedisProvider.GetHashValuesAsBinary(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void GetHashValuesAsBinaryDisposed()
        {
            this.RedisProvider.Dispose();

            this.RedisProvider.GetHashValuesAsBinary("Key");
        }

        [TestMethod]
        public void GetHashValuesAsBinaryNotExistentKey()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            var values = this.RedisProvider.GetHashValuesAsBinary(key);

            Assert.AreEqual(0, values.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(RedisException))]
        public void GetHashValuesAsBinaryWrongKeyType()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetValue(key, string.Empty);

            this.RedisProvider.GetHashValuesAsBinary(key);
        }

        #endregion GetHashValuesAsBinary

        #region SetHashValues (Text)

        [TestMethod]
        public void SetHashValuesText()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetHashValues(key, new Dictionary<string, string>()
            {
                { "Key 1", "Value 1" },
                { "Key 2", "Value 2" }
            });

            IReadOnlyDictionary<string, string> values = this.RedisProvider.GetHashAsText(key);

            Assert.AreEqual(2, values.Count);

            Assert.AreEqual("Value 1", values["Key 1"]);
            Assert.AreEqual("Value 2", values["Key 2"]);

            this.RedisProvider.SetHashValues(key, new Dictionary<string, string>()
            {
                { "Key 1", "Value 11" },
                { "Key 3", "Value 3" }
            });

            values = this.RedisProvider.GetHashAsText(key);

            Assert.AreEqual(3, values.Count);

            Assert.AreEqual("Value 11", values["Key 1"]);
            Assert.AreEqual("Value 2", values["Key 2"]);
            Assert.AreEqual("Value 3", values["Key 3"]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetHashValuesTextNullKey()
        {
            this.RedisProvider.SetHashValues(null, new Dictionary<string, string>() { { "Key", "Value" } });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetHashValuesTextEmptyKey()
        {
            this.RedisProvider.SetHashValues(string.Empty, new Dictionary<string, string>() { { "Key", "Value" } });
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void SetHashValuesTextDisposed()
        {
            this.RedisProvider.Dispose();

            this.RedisProvider.SetHashValues("Key", new Dictionary<string, string>() { { "Key", "Value" } });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetHashValuesTextNullValues()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetHashValues(key, default(IReadOnlyDictionary<string, string>));
        }

        [TestMethod]
        public void SetHashValuesTextNoValues()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetHashValues(key, new Dictionary<string, string>());
        }

        [TestMethod]
        public void SetHashValuesTextEmptyField()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetHashValues(key, new Dictionary<string, string>() { { string.Empty, "Value" } });
        }

        [TestMethod]
        public void SetHashValuesTextEmptyValue()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetHashValues(key, new Dictionary<string, string>() { { "Field", string.Empty } });
        }

        #endregion SetHashValues (Text)

        #region SetHashValues (Binary)

        [TestMethod]
        public void SetHashValuesBinary()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetHashValues(key, new Dictionary<string, byte[]>()
            {
                { "Key 1", Encoding.ASCII.GetBytes("Value 1") },
                { "Key 2", Encoding.ASCII.GetBytes("Value 2") }
            });

            IReadOnlyDictionary<string, byte[]> values = this.RedisProvider.GetHashAsBinary(key);

            Assert.AreEqual(2, values.Count);

            Assert.AreEqual("Value 1", Encoding.ASCII.GetString(values["Key 1"]));
            Assert.AreEqual("Value 2", Encoding.ASCII.GetString(values["Key 2"]));

            this.RedisProvider.SetHashValues(key, new Dictionary<string, byte[]>()
            {
                { "Key 1", Encoding.ASCII.GetBytes("Value 11") },
                { "Key 3", Encoding.ASCII.GetBytes("Value 3") }
            });

            values = this.RedisProvider.GetHashAsBinary(key);

            Assert.AreEqual(3, values.Count);

            Assert.AreEqual("Value 11", Encoding.ASCII.GetString(values["Key 1"]));
            Assert.AreEqual("Value 2", Encoding.ASCII.GetString(values["Key 2"]));
            Assert.AreEqual("Value 3", Encoding.ASCII.GetString(values["Key 3"]));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetHashValuesBinaryNullKey()
        {
            this.RedisProvider.SetHashValues(null, new Dictionary<string, byte[]>() { { "Field", new byte[0] } });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetHashValuesBinaryEmptyKey()
        {
            this.RedisProvider.SetHashValues(string.Empty, new Dictionary<string, byte[]>() { { "Field", new byte[0] } });
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void SetHashValuesBinaryDisposed()
        {
            this.RedisProvider.Dispose();

            this.RedisProvider.SetHashValues("Key", new Dictionary<string, byte[]>() { { "Field", new byte[0] } });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetHashValuesBinaryNullValues()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetHashValues(key, default(IReadOnlyDictionary<string, byte[]>));
        }

        [TestMethod]
        public void SetHashValuesBinaryNoValues()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetHashValues(key, new Dictionary<string, byte[]>());
        }

        [TestMethod]
        public void SetHashValuesBinaryEmptyField()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetHashValues(key, new Dictionary<string, byte[]>() { { string.Empty, new byte[0] } });
        }

        [TestMethod]
        public void SetHashValuesBinaryEmptyValue()
        {
            string key = RedisProviderUnitTests.GetTempRedisKey();

            this.RedisProvider.SetHashValues(key, new Dictionary<string, byte[]>() { { "Field", new byte[0] } });
        }

        #endregion SetHashValues (Binary)

        internal static string GetTempRedisKey()
        {
            return RedisProviderUnitTests.TempKeyPrefix + Guid.NewGuid();
        }

        protected abstract IRedisProvider CreateRedisProvider();
    }
}