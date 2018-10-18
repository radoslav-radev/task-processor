using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Radoslav.Collections
{
    /// <summary>
    /// A base type for collection wrappers that provide only read operations.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the collections.</typeparam>
    /// <typeparam name="TCollection">The type of the wrapped collection for which thread-safe operations must be ensured.</typeparam>
    /// <threadsafety static="true" instance="true"/>
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = SuppressMessages.ResourceNotExpensiveAndDisposedInDestructor)]
    public abstract class ReadOnlyConcurrentCollection<T, TCollection> : IReadOnlyCollection<T>
        where TCollection : ICollection<T>
    {
        #region Fields

        /// <summary>
        /// The wrapped collection for which thread-safe operations must be ensured.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = SuppressMessages.PerformanceOptimization)]
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = SuppressMessages.PerformanceOptimization)]
        protected readonly TCollection Collection;

        /// <summary>
        /// The instance of <see cref="ReaderWriterLockSlim"/> used to ensure read and write locks.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = SuppressMessages.PerformanceOptimization)]
        protected readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim();

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyConcurrentCollection{T, TCollection}"/> class.
        /// </summary>
        /// <param name="collection">The collection to wrap in a thread-safe manner.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="collection"/> is null.</exception>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = SuppressMessages.ResourceNotExpensiveAndDisposedInDestructor)]
        protected ReadOnlyConcurrentCollection(TCollection collection)
            : this(collection, new ReaderWriterLockSlim())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyConcurrentCollection{T, TCollection}"/> class.
        /// </summary>
        /// <param name="collection">The collection to wrap in a thread-safe manner.</param>
        /// <param name="locker">The <see cref="ReaderWriterLockSlim"/> instance to use for read and write locks.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="collection"/> or <paramref name="locker" /> is null.</exception>
        protected ReadOnlyConcurrentCollection(TCollection collection, ReaderWriterLockSlim locker)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            if (locker == null)
            {
                throw new ArgumentNullException("locker");
            }

            this.Collection = collection;
            this.Locker = locker;
        }

        #endregion Constructor

        #region IReadOnlyCollection<T> Members

        /// <inheritdoc />
        public int Count
        {
            get
            {
                return this.ExecuteWithReadLock(() => this.Collection.Count);
            }
        }

        #endregion IReadOnlyCollection<T> Members

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new LockEnumeratorWrapper(this.Locker, this.Collection.GetEnumerator());
        }

        #endregion IEnumerable Members

        #region IEnumerable<T> Members

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator()
        {
            return new LockEnumeratorWrapper<T>(this.Locker, this.Collection.GetEnumerator());
        }

        #endregion IEnumerable<T> Members

        #region Execute with Read Lock

        /// <summary>
        /// Executes a callback method with a read lock on the collection.
        /// </summary>
        /// <remarks>
        /// This method does the following in the specified order:
        /// <list type="number">
        /// <item><description>If the current thread does not hold a read nor write lock on the collection - acquire read lock.</description></item>
        /// <item><description>Execute the callback method.</description></item>
        /// <item><description>If a read lock has been acquired in step 1 - release it.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="callback">The callback method to be executed with read lock on the collection.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="callback"/> is null.</exception>
        [DebuggerStepThrough]
        public void ExecuteWithReadLock(Action callback)
        {
            this.ExecuteWithReadLock<object>(() => { callback(); return null; });
        }

        /// <summary>
        /// Executes a callback method with a read lock on the collection.
        /// </summary>
        /// <remarks>
        /// This method does the following in the specified order:
        /// <list type="number">
        /// <item><description>If the current thread does not hold a read nor write lock on the collection - acquire read lock.</description></item>
        /// <item><description>Execute the callback method.</description></item>
        /// <item><description>If a read lock has been acquired in step 1 - release it.</description></item>
        /// </list>
        /// </remarks>
        /// <typeparam name="TResult">The type of the result returned by the method.</typeparam>
        /// <param name="callback">The callback method to be executed with read lock on the collection.</param>
        /// <returns>The result of the execution of the callback method.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="callback"/> is null.</exception>
        [DebuggerStepThrough]
        public TResult ExecuteWithReadLock<TResult>(Func<TResult> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }

            bool isReadLockAcquired = false;

            if (!this.Locker.IsReadLockHeld && !this.Locker.IsWriteLockHeld)
            {
                this.Locker.EnterReadLock();

                isReadLockAcquired = true;
            }

            try
            {
                return callback();
            }
            finally
            {
                if (isReadLockAcquired && this.Locker.IsReadLockHeld)
                {
                    this.Locker.ExitReadLock();
                }
            }
        }

        #endregion Execute with Read Lock
    }
}