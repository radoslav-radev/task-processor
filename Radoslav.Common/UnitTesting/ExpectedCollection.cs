using System;
using System.Collections.Generic;

namespace Radoslav
{
    public sealed class ExpectedCollection<T> : IEnumerable<T>, IExpectedCollection
    {
        private readonly IEnumerable<T> collection;

        #region Constructor

        public ExpectedCollection(params T[] collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            this.collection = collection;
        }

        public ExpectedCollection(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            this.collection = collection;
        }

        #endregion Constructor

        #region IMockEnumerable Members

        public bool IsOrdered { get; internal set; }

        #endregion IMockEnumerable Members

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return this.collection.GetEnumerator();
        }

        #endregion IEnumerable<T> Members

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.collection.GetEnumerator();
        }

        #endregion IEnumerable Members
    }
}