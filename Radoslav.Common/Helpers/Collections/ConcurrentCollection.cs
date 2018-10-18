using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Radoslav.Collections
{
    /// <summary>
    /// A class that represents a thread-safe collection wrapper.
    /// </summary>
    /// <remarks>This class uses <see cref="ReaderWriterLockSlim"/> to ensure thread-safe operations.</remarks>
    /// <typeparam name="T">The type of the elements of the collection.</typeparam>
    /// <typeparam name="TCollection">The type of the collection.</typeparam>
    /// <threadsafety static="true" instance="true" />
    public abstract class ConcurrentCollection<T, TCollection> : ReadOnlyConcurrentCollection<T, TCollection>, ICollection<T>
        where TCollection : ICollection<T>
    {
        #region Constructors

        /// <inheritdoc />
        protected ConcurrentCollection(TCollection collection)
            : base(collection)
        {
        }

        /// <inheritdoc />
        protected ConcurrentCollection(TCollection collection, ReaderWriterLockSlim locker)
            : base(collection, locker)
        {
        }

        #endregion Constructors

        #region ICollection<T> Members

        /// <inheritdoc />
        public bool IsReadOnly { get; protected set; }

        /// <inheritdoc />
        public void Add(T item)
        {
            this.ExecuteWithWriteLock(() => this.Collection.Add(item));
        }

        /// <inheritdoc />
        public bool Remove(T item)
        {
            return this.ExecuteWithWriteLock(() => this.Collection.Remove(item));
        }

        /// <inheritdoc />
        public bool Contains(T item)
        {
            return this.ExecuteWithReadLock(() => this.Collection.Contains(item));
        }

        /// <inheritdoc />
        public void Clear()
        {
            this.ExecuteWithWriteLock(this.Collection.Clear);
        }

        /// <inheritdoc />
        public void CopyTo(T[] array, int arrayIndex)
        {
            this.ExecuteWithReadLock(() => this.Collection.CopyTo(array, arrayIndex));
        }

        #endregion ICollection<T> Members

        #region Add/Remove Range

        /// <summary>
        /// Adds items to the <see cref="ConcurrentCollection{T,TCollection}"/>.
        /// </summary>
        /// <param name="items">The items to be added.</param>
        public virtual void AddRange(params T[] items)
        {
            this.ExecuteWithWriteLock(() =>
            {
                items.ForEach(false, item => this.Collection.Add(item));
            });
        }

        /// <summary>
        /// Adds items to the <see cref="ConcurrentCollection{T,TCollection}"/>.
        /// </summary>
        /// <param name="items">The items to be added.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="items"/> is null.</exception>
        public virtual void AddRange(IEnumerable<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            this.ExecuteWithWriteLock(() =>
                {
                    items.ForEach(false, item => this.Collection.Add(item));
                });
        }

        /// <summary>
        /// Removes items from the <see cref="ConcurrentCollection{T,TCollection}"/>.
        /// </summary>
        /// <param name="items">The items to be removed.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="items"/> is null.</exception>
        public void RemoveRange(IEnumerable<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            this.ExecuteWithWriteLock(() =>
            {
                items.ForEach(false, item => this.Collection.Remove(item));
            });
        }

        /// <summary>
        /// Removes items from the <see cref="ConcurrentCollection{T,TCollection}"/> according to a condition.
        /// </summary>
        /// <param name="condition">The condition to be used in order to determine which items should be removed.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="condition"/> is null.</exception>
        public void RemoveRange(Func<T, bool> condition)
        {
            if (condition == null)
            {
                throw new ArgumentNullException("condition");
            }

            this.ExecuteWithWriteLock(() =>
                {
                    T[] itemsToRemove = this.Collection.Where(condition).ToArray();

                    itemsToRemove.ForEach(false, item => this.Collection.Remove(item));
                });
        }

        #endregion Add/Remove Range

        #region Execute with Write Lock

        /// <summary>
        /// Executes a callback method with a write lock on the collection.
        /// </summary>
        /// <remarks>
        /// This method does the following in the specified order:
        /// <list type="number">
        /// <item><description>If the current thread does not hold a write lock on the collection - acquire it.</description></item>
        /// <item><description>Execute the callback method.</description></item>
        /// <item><description>If a write lock has been acquired in step 1 - release it.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="callback">The callback method to be executed with write lock on the collection.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="callback"/> is null.</exception>
        /// <exception cref="LockRecursionException">Current thread already holds a read lock on the collection.</exception>
        /// <exception cref="InvalidOperationException">Collection is read-only.</exception>
        [DebuggerStepThrough]
        public void ExecuteWithWriteLock(Action callback)
        {
            this.ExecuteWithWriteLock<object>(() => { callback(); return null; });
        }

        /// <summary>
        /// Executes a callback method with a write lock on the collection.
        /// </summary>
        /// <remarks>
        /// This method does the following in the specified order:
        /// <list type="number">
        /// <item><description>If the current thread does not hold a write lock on the collection - acquire it.</description></item>
        /// <item><description>Execute the callback method.</description></item>
        /// <item><description>If a write lock has been acquired in step 1 - release it.</description></item>
        /// </list>
        /// </remarks>
        /// <typeparam name="TResult">The type of the result returned by the method.</typeparam>
        /// <param name="callback">The callback method to be executed with write lock on the collection.</param>
        /// <returns>The result of the execution of the callback method.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="callback"/> is null.</exception>
        /// <exception cref="LockRecursionException">Current thread already holds a read lock on the collection.</exception>
        /// <exception cref="InvalidOperationException">Collection is read-only.</exception>
        [DebuggerStepThrough]
        public TResult ExecuteWithWriteLock<TResult>(Func<TResult> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }

            if (this.IsReadOnly)
            {
                throw new InvalidOperationException("Collection is read-only.");
            }

            bool isWriteLockAcquired = false;

            if (!this.Locker.IsWriteLockHeld)
            {
                this.Locker.EnterWriteLock();

                isWriteLockAcquired = true;
            }

            try
            {
                return callback();
            }
            finally
            {
                if (isWriteLockAcquired && this.Locker.IsWriteLockHeld)
                {
                    this.Locker.ExitWriteLock();
                }
            }
        }

        #endregion Execute with Write Lock
    }
}