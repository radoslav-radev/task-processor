using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Collections;

namespace Radoslav.Common.UnitTests.Collections
{
    [TestClass]
    public sealed class MultiKeyDictionaryTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddNull1Key()
        {
            MultiKeyDictionary<string, string, Version> dictionary = new MultiKeyDictionary<string, string, Version>();

            dictionary.Add(null, string.Empty, new Version());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddNullKey2()
        {
            MultiKeyDictionary<string, string, Version> dictionary = new MultiKeyDictionary<string, string, Version>();

            dictionary.Add(string.Empty, null, new Version());
        }

        [TestMethod]
        public void AddNullValue()
        {
            MultiKeyDictionary<int, int, Version> dictionary = new MultiKeyDictionary<int, int, Version>();

            dictionary.Add(0, 0, null);

            Assert.IsNull(dictionary[0, 0]);
        }

        [TestMethod]
        public void Add1()
        {
            MultiKeyDictionary<int, string, Version> dictionary = new MultiKeyDictionary<int, string, Version>();

            dictionary.Add(1, "A", new Version(1, 2));

            Assert.AreEqual(new Version(1, 2), dictionary[1, "A"]);
        }

        [TestMethod]
        public void Add2()
        {
            MultiKeyDictionary<int, string, Version> dictionary = new MultiKeyDictionary<int, string, Version>();

            dictionary.Add(1, "A", new Version(1, 2));
            dictionary.Add(1, "B", new Version(1, 3));

            Assert.AreEqual(new Version(1, 3), dictionary[1, "B"]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddExisting()
        {
            MultiKeyDictionary<int, string, Version> dictionary = new MultiKeyDictionary<int, string, Version>();

            dictionary.Add(1, "A", new Version(1, 1));
            dictionary.Add(1, "A", new Version(1, 2));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryGetValueNullKey1()
        {
            MultiKeyDictionary<string, string, Version> dictionary = new MultiKeyDictionary<string, string, Version>();

            Version result;

            dictionary.TryGetValue(null, string.Empty, out result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryGetValueNullKey2()
        {
            MultiKeyDictionary<string, string, Version> dictionary = new MultiKeyDictionary<string, string, Version>();

            Version result;

            dictionary.TryGetValue(string.Empty, null, out result);
        }

        [TestMethod]
        public void TryGetValueTrue()
        {
            MultiKeyDictionary<int, int, Version> dictionary = new MultiKeyDictionary<int, int, Version>();

            dictionary.Add(1, 2, new Version(1, 2));

            Version result;

            Assert.IsTrue(dictionary.TryGetValue(1, 2, out result));
            Assert.AreEqual(new Version(1, 2), result);
        }

        [TestMethod]
        public void TryGetValueFalse1()
        {
            MultiKeyDictionary<int, int, Version> dictionary = new MultiKeyDictionary<int, int, Version>();

            Version result;

            Assert.IsFalse(dictionary.TryGetValue(0, 0, out result));
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TryGetValueFalse2()
        {
            MultiKeyDictionary<int, int, Version> dictionary = new MultiKeyDictionary<int, int, Version>();

            dictionary.Add(1, 1, new Version(1, 1));

            Version result;

            Assert.IsFalse(dictionary.TryGetValue(1, 2, out result));
            Assert.IsNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ContainsKey1Null()
        {
            MultiKeyDictionary<string, int, Version> dictionary = new MultiKeyDictionary<string, int, Version>();

            dictionary.ContainsKey(null);
        }

        [TestMethod]
        public void ContainsKey1True()
        {
            MultiKeyDictionary<int, int, Version> dictionary = new MultiKeyDictionary<int, int, Version>();

            dictionary.Add(1, 2, new Version(1, 2));

            Assert.IsTrue(dictionary.ContainsKey(1));
        }

        [TestMethod]
        public void ContainsKey1False()
        {
            MultiKeyDictionary<int, int, Version> dictionary = new MultiKeyDictionary<int, int, Version>();

            Assert.IsFalse(dictionary.ContainsKey(1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetValueNullKey1()
        {
            MultiKeyDictionary<string, string, Version> dictionary = new MultiKeyDictionary<string, string, Version>();

            if (dictionary[null, string.Empty] == null)
            {
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetValueNullKey2()
        {
            MultiKeyDictionary<string, string, Version> dictionary = new MultiKeyDictionary<string, string, Version>();

            if (dictionary[string.Empty, null] == null)
            {
            }
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void GetValueNotFound1()
        {
            MultiKeyDictionary<int, int, Version> dictionary = new MultiKeyDictionary<int, int, Version>();

            if (dictionary[0, 0] == null)
            {
            }
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void GetValueNotFound2()
        {
            MultiKeyDictionary<int, int, Version> dictionary = new MultiKeyDictionary<int, int, Version>();

            dictionary.Add(1, 2, new Version());

            if (dictionary[1, 3] == null)
            {
            }
        }
    }
}