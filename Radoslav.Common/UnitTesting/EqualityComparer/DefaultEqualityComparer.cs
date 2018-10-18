using System.Collections.Generic;

namespace Radoslav
{
    internal sealed class DefaultEqualityComparer<T> : IEqualityComparer<T>
    {
        #region IEqualityComparer<T> Members

        public bool Equals(T x, T y)
        {
            return object.Equals(x, y);
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }

        #endregion IEqualityComparer<T> Members
    }
}