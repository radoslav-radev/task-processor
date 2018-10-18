using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Radoslav
{
    /// <summary>
    /// Class for extensions and helpers for classes in System.Collections and System.Collections.Generic namespaces.
    /// </summary>
    public static class CollectionHelpers
    {
        /// <summary>
        /// Checks if a collection is empty, i.e. has no elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements of collection.</typeparam>
        /// <param name="collection">The collection to be checked.</param>
        /// <returns>True if <paramref name="collection"/> does not have any elements; otherwise false.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="collection"/> is null.</exception>
        public static bool IsEmpty<T>(this IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            return !collection.Any();
        }

        /// <summary>
        /// Executes a method for each element in a collection.
        /// </summary>
        /// <typeparam name="T">The type of the elements of collection.</typeparam>
        /// <param name="collection">The collection on which elements to execute the specified method.</param>
        /// <param name="callback">The method to execute for each element of the collection.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="collection"/> or <paramref name="callback"/> is null.</exception>
        [DebuggerStepThrough]
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> callback)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }

            foreach (T item in collection)
            {
                callback(item);
            }
        }

        /// <summary>
        /// Executes a method for each element in a collection.
        /// </summary>
        /// <typeparam name="T">The type of the elements of collection.</typeparam>
        /// <param name="collection">The collection on which elements to execute the specified method.</param>
        /// <param name="executeCallbackInParallel">Whether iteration should be done in parallel.</param>
        /// <param name="callback">The method to execute for each element of the collection.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="collection"/> or <paramref name="callback"/> is null.</exception>
        public static void ForEach<T>(this IEnumerable<T> collection, bool executeCallbackInParallel, Action<T> callback)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }

            if (executeCallbackInParallel)
            {
                Parallel.ForEach(collection, callback);
            }
            else
            {
                foreach (T item in collection)
                {
                    callback(item);
                }
            }
        }

        /// <summary>
        /// Returns a string that represents the current collection as separator-delimited elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements of collection.</typeparam>
        /// <param name="collection">The collection to represent as string.</param>
        /// <param name="separator">Separator to be used between collection elements.</param>
        /// <returns>A string that represents the current objects as separator-delimited elements.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="collection"/> or <paramref name="separator"/> is null.</exception>
        public static string ToString<T>(this IEnumerable<T> collection, string separator)
        {
            /* Do not check arguments because it is done in the overload method. */

            return collection.ToString(separator, item => item.ToString());
        }

        /// <summary>
        /// Returns a string that represents the current collection as separator-delimited elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements of collection.</typeparam>
        /// <param name="collection">The collection to represent as string.</param>
        /// <param name="separator">Separator to be used between collection elements.</param>
        /// <param name="converter">Function to convert collection element to string for the representation.</param>
        /// <returns>A string that represents the current objects as separator-delimited elements.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="collection"/>, <paramref name="separator"/> or <paramref name="converter"/> is null.</exception>
        public static string ToString<T>(this IEnumerable<T> collection, string separator, Func<T, object> converter)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            if (separator == null)
            {
                throw new ArgumentNullException("separator");
            }

            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }

            return string.Join(separator, collection.Select(item => converter(item)));
        }

        /// <summary>
        /// Checks if two collections are equivalent, i.e. every element in the first collection is equal to the
        /// corresponding element in the second collection, and both collections have the same number of elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="collection">The collection who is extended by the extension method.</param>
        /// <param name="expected">The collection to check equivalence with.</param>
        /// <returns>True if collection are equivalent, i.e. every element in the first collection is equal to the
        /// corresponding element in the second collection, and both collections have the same number of elements.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="collection"/> or <paramref name="expected"/> is null.</exception>
        public static bool IsEquivalentTo<T>(this IEnumerable<T> collection, IEnumerable<T> expected)
        {
            return collection.IsEquivalentTo(expected, (a, b) => object.Equals(a, b));
        }

        /// <summary>
        /// Checks if two collections are equivalent, i.e. every element in the first collection is equal to the
        /// corresponding element in the second collection, and both collections have the same number of elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="collection">The collection who is extended by the extension method.</param>
        /// <param name="expected">The collection to check equivalence with.</param>
        /// <param name="compareCallback">The callback to use when comparing element.</param>
        /// <returns>True if collection are equivalent, i.e. every element in the first collection is equal to the
        /// corresponding element in the second collection, and both collections have the same number of elements.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="collection"/>, <paramref name="expected"/> or <paramref name="compareCallback"/> is null.</exception>
        public static bool IsEquivalentTo<T>(this IEnumerable<T> collection, IEnumerable<T> expected, Func<T, T, bool> compareCallback)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            if (expected == null)
            {
                throw new ArgumentNullException("expected");
            }

            if (compareCallback == null)
            {
                throw new ArgumentNullException("compareCallback");
            }

            IEnumerator<T> first = collection.GetEnumerator();
            IEnumerator<T> second = expected.GetEnumerator();

            while (first.MoveNext())
            {
                if (!second.MoveNext())
                {
                    return false;
                }

                if (!compareCallback(first.Current, second.Current))
                {
                    return false;
                }
            }

            return !second.MoveNext();
        }

        /// <summary>
        ///  Returns a number that represents how many elements in the specified sequence are equal to a specified element.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <param name="collection">A sequence that contains elements to be tested and counted.</param>
        /// <param name="element">The element to check for.</param>
        /// <returns> A number that represents how many elements in the sequence are equal to the specified element.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="collection"/> is null.</exception>
        public static int Count<T>(this IEnumerable<T> collection, T element)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            return collection.Count(item => object.Equals(item, element));
        }

        /// <summary>
        /// Returns a collection without a specified single element.
        /// </summary>
        /// <typeparam name="T">The type of the elements of collection.</typeparam>
        /// <param name="collection">The collection to filter.</param>
        /// <param name="item">The element to exclude from the result.</param>
        /// <returns>A collection without the specified element.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="collection"/> is null.</exception>
        public static IEnumerable<T> Except<T>(this IEnumerable<T> collection, T item)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            return collection.Except(new T[] { item });
        }

        /// <summary>
        /// Checks if a collection contains at least specified number of elements satisfying a specified condition.
        /// </summary>
        /// <remarks>If <paramref name="count"/> is 0, method returns <c>true</c>.</remarks>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="collection">The collection to check.</param>
        /// <param name="count">The number of elements searched.</param>
        /// <returns>True if the collection contains at least the specified number of elements satisfying the specified condition.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="collection"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Parameter <paramref name="count"/> is negative.</exception>
        public static bool AtLeast<T>(this IEnumerable<T> collection, int count)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count", count, "Value must not be negative.");
            }

            if (count == 0)
            {
                return true;
            }

            int found = 0;

            IEnumerator<T> enumerator = collection.GetEnumerator();

            while (enumerator.MoveNext())
            {
                found++;

                if (found == count)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if a collection contains at least specified number of elements satisfying a specified condition.
        /// </summary>
        /// <remarks>If <paramref name="count"/> is 0, method returns <c>true</c>.</remarks>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="collection">The collection to check.</param>
        /// <param name="count">The number of elements searched.</param>
        /// <param name="predicate">The predicate to filter collection elements.</param>
        /// <returns>True if the collection contains at least the specified number of elements satisfying the specified condition.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="collection"/> or <paramref name="predicate"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Parameter <paramref name="count"/> is negative.</exception>
        public static bool AtLeast<T>(this IEnumerable<T> collection, int count, Func<T, bool> predicate)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count", count, "Value must not be negative.");
            }

            if (count == 0)
            {
                return true;
            }

            int found = 0;

            foreach (T item in collection)
            {
                if (predicate(item))
                {
                    found++;

                    if (found == count)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Removes all elements from a collection satisfying a specified condition.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="collection">The collection to check.</param>
        /// <param name="predicate">The condition to determine which elements to remove from the collection.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="collection"/> or <paramref name="predicate"/> is null.</exception>
        public static void Remove<T>(this ICollection<T> collection, Func<T, bool> predicate)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            foreach (T item in collection.Where(v => predicate(v)).ToArray())
            {
                collection.Remove(item);
            }
        }

        /// <summary>
        /// Checks if a collection is fixed, for example an array or list, and is not materialized every time when enumerated.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="collection">The collection to check.</param>
        /// <returns>True if the collection is fixed; false if the collection is materialized every time when enumerated.</returns>
        public static bool IsFixed<T>(this IEnumerable<T> collection)
        {
            return (collection is T[]) || (collection is List<T>) || (collection is Collection<T>);
        }

        /// <summary>
        /// Returns a fixed collection from another.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="collection">The collection to covert to a fixed one.</param>
        /// <returns>If the source collection is fixed, the method returns it (the same instance).
        /// If it is not, makes an array from it and returns it.</returns>
        public static IEnumerable<T> ToFixed<T>(this IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            return collection.IsFixed() ? collection : collection.ToArray();
        }

        /// <summary>
        /// Checks if the collection is empty or all its elements match a condition.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="collection">The collection to check.</param>
        /// <param name="predicate">The condition to check.</param>
        /// <returns>True if the collection is empty or all its elements match a condition.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="collection"/> or <paramref name="predicate"/> is null.</exception>
        public static bool AllOrEmpty<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
        {
            return !collection.Any() || collection.All(predicate);
        }
    }
}