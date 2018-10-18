using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Radoslav.Collections
{
    /// <summary>
    /// A class for a dictionary with two keys.
    /// </summary>
    /// <typeparam name="TKey1">The type of the first key.</typeparam>
    /// <typeparam name="TKey2">The type of the second key.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Justification = "This is a multi-key dictionary.")]
    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "This is a multi-key dictionary.")]
    [SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes", Justification = "This is a dictionary with two keys.")]
    public sealed class MultiKeyDictionary<TKey1, TKey2, TValue>
    {
        private readonly Dictionary<TKey1, Dictionary<TKey2, TValue>> values = new Dictionary<TKey1, Dictionary<TKey2, TValue>>();

        /// <summary>
        ///  Gets or sets the value associated with the specified keys.
        /// </summary>
        /// <param name="key1">The value of the first key.</param>
        /// <param name="key2">The value of the second key.</param>
        /// <returns>The value associated with the specified keys.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key1"/> or <paramref name="key2"/> is null.</exception>
        /// <exception cref="KeyNotFoundException">The property is retrieved and keys does not exist in the collection.</exception>
        [SuppressMessage("Microsoft.Design", "CA1023:IndexersShouldNotBeMultidimensional", Justification = "This is a dictionary with two keys.")]
        public TValue this[TKey1 key1, TKey2 key2]
        {
            get
            {
                if (key1 == null)
                {
                    throw new ArgumentNullException("key1");
                }

                if (key2 == null)
                {
                    throw new ArgumentNullException("key2");
                }

                return this.values[key1][key2];
            }

            set
            {
                if (key1 == null)
                {
                    throw new ArgumentNullException("key1");
                }

                if (key2 == null)
                {
                    throw new ArgumentNullException("key2");
                }

                Dictionary<TKey2, TValue> temp;

                if (this.values.TryGetValue(key1, out temp))
                {
                    temp[key2] = value;
                }
                else
                {
                    this.values.Add(key1, new Dictionary<TKey2, TValue>() { { key2, value } });
                }
            }
        }

        /// <summary>
        /// Adds the specified keys and value to the dictionary.
        /// </summary>
        /// <param name="key1">The value of the first key.</param>
        /// <param name="key2">The value of the second key.</param>
        /// <param name="value">The value associated with the specified keys.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key1"/> or <paramref name="key2"/> is null.</exception>
        /// <exception cref="ArgumentException">An item with the specified keys already exists in dictionary.</exception>
        public void Add(TKey1 key1, TKey2 key2, TValue value)
        {
            if (key1 == null)
            {
                throw new ArgumentNullException("key1");
            }

            if (key2 == null)
            {
                throw new ArgumentNullException("key2");
            }

            Dictionary<TKey2, TValue> temp;

            if (this.values.TryGetValue(key1, out temp))
            {
                temp.Add(key2, value);
            }
            else
            {
                this.values.Add(key1, new Dictionary<TKey2, TValue>() { { key2, value } });
            }
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key1">The value of the first key.</param>
        /// <param name="key2">The value of the second key.</param>
        /// <param name="result">The value associated with the specified keys.</param>
        /// <returns>True if the dictionary contains an element with the specified key; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key1"/> or <paramref name="key2"/> is null.</exception>
        public bool TryGetValue(TKey1 key1, TKey2 key2, out TValue result)
        {
            if (key1 == null)
            {
                throw new ArgumentNullException("key1");
            }

            if (key2 == null)
            {
                throw new ArgumentNullException("key2");
            }

            Dictionary<TKey2, TValue> temp;

            if (this.values.TryGetValue(key1, out temp))
            {
                return temp.TryGetValue(key2, out result);
            }
            else
            {
                result = default(TValue);

                return false;
            }
        }

        /// <summary>
        /// Determines whether the dictionary contains at least one element with the specified first key.
        /// </summary>
        /// <param name="key1">The key to locate in the dictionary.</param>
        /// <returns>True if the dictionary contains at least one element with the specified first key; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key1"/> is null.</exception>
        public bool ContainsKey(TKey1 key1)
        {
            return this.values.ContainsKey(key1);
        }
    }
}