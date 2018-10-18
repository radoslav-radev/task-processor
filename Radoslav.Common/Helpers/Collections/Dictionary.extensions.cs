using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Radoslav
{
    public static partial class Helpers
    {
        /// <summary>
        /// Gets the value associated with the specified key, or the default value for the type if not found.
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
        /// <param name="dictionary">A dictionary to get value from.</param>
        /// <param name="key">The key whose value to get.</param>
        /// <returns>The element with the specified key, or the default value for <typeparamref name="TValue"/> if an element with the specified key is not found in the dictionary.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="dictionary"/> or <paramref name="key"/> is null.</exception>
        public static TValue GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key)
        {
            /* Do not check arguments because it is done in the overload method. */

            return dictionary.GetValueOrDefault(key, default(TValue));
        }

        /// <summary>
        /// Gets the value associated with the specified key, or the default value for the type if not found.
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
        /// <param name="dictionary">A dictionary to get value from.</param>
        /// <param name="key">The key whose value to get.</param>
        /// <returns>The element with the specified key, or the default value for <typeparamref name="TValue"/> if an element with the specified key is not found in the dictionary.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="dictionary"/> or <paramref name="key"/> is null.</exception>
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            /* Do not check arguments because it is done in the overload method. */

            return dictionary.GetValueOrDefault(key, default(TValue));
        }

        /// <summary>
        /// Gets the value associated with the specified key, or a default value if not found.
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
        /// <param name="dictionary">A dictionary to get value from.</param>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="defaultValue">The default value to be returned if an element with the specified key is not found in the dictionary.</param>
        /// <returns>The element with the specified key, or the specified default value if if an element with the specified key is not found in the dictionary.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="dictionary"/> or <paramref name="key"/> is null.</exception>
        public static TValue GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            /* Do not check arguments because it is done in the overload method. */

            return dictionary.GetValueOrDefault(key, () => defaultValue);
        }

        /// <summary>
        /// Gets the value associated with the specified key, or a default value if not found.
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
        /// <param name="dictionary">A dictionary to get value from.</param>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="defaultValue">The default value to be returned if an element with the specified key is not found in the dictionary.</param>
        /// <returns>The element with the specified key, or the specified default value if if an element with the specified key is not found in the dictionary.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="dictionary"/> or <paramref name="key"/> is null.</exception>
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            /* Do not check arguments because it is done in the overload method. */

            return dictionary.GetValueOrDefault(key, () => defaultValue);
        }

        /// <summary>
        /// Gets the value associated with the specified key, or a default value provided by a callback
        /// method if an element with the specified key is not found in the dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
        /// <param name="dictionary">A dictionary to get value from.</param>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="defaultValueCallback">A callback method to provide default value to be returned if an element with the specified key is not found in the dictionary.</param>
        /// <returns>The element with the specified key, or the default value provided by callback method if an element with such a key is not found in the dictionary.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="dictionary"/>, <paramref name="key"/>, or <paramref name="defaultValueCallback"/> is null.</exception>
        public static TValue GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> defaultValueCallback)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }

            if (defaultValueCallback == null)
            {
                throw new ArgumentNullException("defaultValueCallback");
            }

            TValue result;

            if (!dictionary.TryGetValue(key, out result))
            {
                result = defaultValueCallback();
            }

            return result;
        }

        /// <summary>
        /// Gets the value associated with the specified key, or a default value provided by a callback
        /// method if an element with the specified key is not found in the dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
        /// <param name="dictionary">A dictionary to get value from.</param>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="defaultValueCallback">A callback method to provide default value to be returned if an element with the specified key is not found in the dictionary.</param>
        /// <returns>The element with the specified key, or the default value provided by callback method if an element with such a key is not found in the dictionary.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="dictionary"/>, <paramref name="key"/>, or <paramref name="defaultValueCallback"/> is null.</exception>
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> defaultValueCallback)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }

            if (defaultValueCallback == null)
            {
                throw new ArgumentNullException("defaultValueCallback");
            }

            TValue result;

            if (!dictionary.TryGetValue(key, out result))
            {
                result = defaultValueCallback();
            }

            return result;
        }

        /// <summary>
        /// Attempts to remove the value with the specified key from the <see cref="ConcurrentDictionary{TKey,TValue}"/>.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
        /// <param name="dictionary">The dictionary to extend with this method.</param>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>True if an object was removed successfully; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null.</exception>
        public static bool TryRemove<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }

            TValue temp;

            return dictionary.TryRemove(key, out temp);
        }
    }
}