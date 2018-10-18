using System;
using System.Collections;
using System.Threading;

namespace Radoslav.Collections
{
    internal class LockEnumeratorWrapper : IEnumerator, IDisposable
    {
        private readonly IEnumerator enumerator;
        private readonly ReaderWriterLockSlim locker;

        #region Constructor & Destructor

        internal LockEnumeratorWrapper(ReaderWriterLockSlim locker, IEnumerator enumerator)
        {
            this.locker = locker;
            this.enumerator = enumerator;

            if (!this.locker.IsReadLockHeld)
            {
                this.locker.EnterReadLock();
            }
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="LockEnumeratorWrapper"/> class.
        /// </summary>
        ~LockEnumeratorWrapper()
        {
            this.Dispose(false);
        }

        #endregion Constructor & Destructor

        #region IEnumerator Members

        /// <inheritdoc />
        public object Current
        {
            get
            {
                return this.enumerator.Current;
            }
        }

        /// <inheritdoc />
        public bool MoveNext()
        {
            return this.enumerator.MoveNext();
        }

        /// <inheritdoc />
        public void Reset()
        {
            if (this.locker.IsReadLockHeld)
            {
                this.locker.ExitReadLock();
            }

            this.enumerator.Reset();
        }

        #endregion IEnumerator Members

        #region IDisposable Members

        /// <inheritdoc />
        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.locker.IsReadLockHeld)
            {
                this.locker.ExitReadLock();
            }

            if (this.enumerator is IDisposable)
            {
                ((IDisposable)this.enumerator).Dispose();
            }
        }

        #endregion IDisposable Members
    }
}