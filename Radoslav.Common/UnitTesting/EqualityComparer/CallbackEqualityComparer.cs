using System;
using System.Collections.Generic;

namespace Radoslav
{
    internal sealed class CallbackEqualityComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> callback;
        private readonly bool callCallbackForNullValues;

        internal CallbackEqualityComparer(Func<T, T, bool> callback, bool callCallbackForNullValues)
        {
            this.callback = callback;
            this.callCallbackForNullValues = callCallbackForNullValues;
        }

        #region IEqualityComparer<T> Members

        public bool Equals(T x, T y)
        {
            if (!this.callCallbackForNullValues)
            {
                if (x == null)
                {
                    return y == null;
                }

                if (y == null)
                {
                    return false;
                }
            }

            return this.callback(x, y);
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }

        #endregion IEqualityComparer<T> Members
    }
}