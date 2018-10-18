using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Radoslav.Collections
{
    /// <summary>
    /// A class that represents a thread-safe <see cref="List{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the elements of the <see cref="List{T}"/>.</typeparam>
    /// <threadsafety static="true" instance="true" />
    [DebuggerDisplay("Count = {Count}")]
    public sealed class ConcurrentList<T> : ConcurrentCollection<T, List<T>>, IList<T>, IReadOnlyConcurrentList<T>
    {
        #region Constructor & Properties

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentList{T}"/> class.
        /// </summary>
        public ConcurrentList()
            : base(new List<T>())
        {
        }

        private ConcurrentList(List<T> collection, ReaderWriterLockSlim locker)
            : base(collection, locker)
        {
        }

        /// <summary>
        /// Gets a value indicating whether the collection is empty, i.e. it has no elements.
        /// </summary>
        /// <value>Whether the collection is empty, i.e. it has no elements.</value>
        public bool IsEmpty
        {
            get
            {
                return this.Count == 0;
            }
        }

        #endregion Constructor & Properties

        #region IList<T> Members

        /// <inheritdoc />
        public T this[int index]
        {
            get
            {
                return this.Collection[index];
            }

            set
            {
                this.Collection[index] = value;
            }
        }

        /// <inheritdoc />
        public int IndexOf(T item)
        {
            return this.ExecuteWithReadLock(() => this.Collection.IndexOf(item));
        }

        /// <inheritdoc />
        public void Insert(int index, T item)
        {
            this.ExecuteWithWriteLock(() => this.Collection.Insert(index, item));
        }

        /// <inheritdoc />
        public void RemoveAt(int index)
        {
            this.ExecuteWithWriteLock(() => this.Collection.RemoveAt(index));
        }

        #endregion IList<T> Members

        #region Add/Remove Range

        /// <inheritdoc />
        public override void AddRange(params T[] items)
        {
            this.ExecuteWithWriteLock(() => this.Collection.AddRange(items));
        }

        /// <inheritdoc />
        public override void AddRange(IEnumerable<T> items)
        {
            this.ExecuteWithWriteLock(() => this.Collection.AddRange(items));
        }

        /// <summary>
        /// Removes a range of elements from the <see cref="ConcurrentList{T}"/>.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range of elements to remove.</param>
        /// <param name="count">The number of elements to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0.-or-<paramref name="count"/> is less than 0.</exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> and <paramref name="count"/> do not denote a valid range of elements.</exception>
        /// <exception cref="LockRecursionException">Current thread already holds a read lock on the collection.</exception>
        public void RemoveRange(int index, int count)
        {
            this.ExecuteWithWriteLock(() => this.Collection.RemoveRange(index, count));
        }

        #endregion Add/Remove Range

        /// <summary>
        /// Creates a read-only wrapper around the current <see cref="ConcurrentList{T}" />.
        /// </summary>
        /// <returns>A read-only wrapper around the current <see cref="ConcurrentList{T}" /></returns>
        public IReadOnlyConcurrentList<T> AsReadOnly()
        {
            return new ConcurrentList<T>(this.Collection, this.Locker)
            {
                IsReadOnly = true
            };
        }
    }
}