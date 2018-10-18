using System.Collections.Generic;
using System.Threading;

namespace Radoslav.Collections
{
    internal sealed class LockEnumeratorWrapper<T> : LockEnumeratorWrapper, IEnumerator<T>
    {
        internal LockEnumeratorWrapper(ReaderWriterLockSlim locker, IEnumerator<T> enumerator)
            : base(locker, enumerator)
        {
        }

        #region IEnumerator<T> Members

        /// <inheritdoc />
        public new T Current
        {
            get { return (T)base.Current; }
        }

        #endregion IEnumerator<T> Members
    }
}