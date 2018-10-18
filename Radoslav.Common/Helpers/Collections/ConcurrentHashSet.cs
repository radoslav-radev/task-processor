using System.Collections.Generic;
using System.Diagnostics;

namespace Radoslav.Collections
{
    /// <summary>
    /// A class that represents a thread-safe hash set.
    /// </summary>
    /// <typeparam name="T">The type of the elements of the hash set.</typeparam>
    /// <threadsafety static="true" instance="true" />
    [DebuggerDisplay("Count = {Count}")]
    public sealed class ConcurrentHashSet<T> : ConcurrentCollection<T, HashSet<T>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentHashSet{T}"/> class.
        /// </summary>
        public ConcurrentHashSet()
            : base(new HashSet<T>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentHashSet{T}"/> class.
        /// </summary>
        /// <param name="collection">Elements to add to the collection initially.</param>
        public ConcurrentHashSet(IEnumerable<T> collection)
            : base(new HashSet<T>(collection))
        {
        }

        /// <summary>
        /// Adds the specified element to a set.
        /// </summary>
        /// <param name="item">The element to add to the set.</param>
        /// <returns>True if the element is added to the set; false if the element is already present.</returns>
        public new bool Add(T item)
        {
            return this.ExecuteWithWriteLock(() => this.Collection.Add(item));
        }
    }
}