using System.Collections;
using System.Collections.Generic;

namespace Radoslav.UnitTests
{
    public sealed class FakeCompositeServiceLocatorObject : ICollection<IFakeServiceLocatorObject>, IFakeServiceLocatorObject
    {
        private readonly List<IFakeServiceLocatorObject> items = new List<IFakeServiceLocatorObject>();

        #region ICollection<IFakeCompositeServiceLocatorObject> Members

        public int Count
        {
            get { return this.items.Count; }
        }

        bool ICollection<IFakeServiceLocatorObject>.IsReadOnly
        {
            get { return false; }
        }

        void ICollection<IFakeServiceLocatorObject>.Add(IFakeServiceLocatorObject item)
        {
            this.items.Add(item);
        }

        void ICollection<IFakeServiceLocatorObject>.Clear()
        {
            this.items.Clear();
        }

        bool ICollection<IFakeServiceLocatorObject>.Contains(IFakeServiceLocatorObject item)
        {
            return this.items.Contains(item);
        }

        void ICollection<IFakeServiceLocatorObject>.CopyTo(IFakeServiceLocatorObject[] array, int arrayIndex)
        {
            this.items.CopyTo(array, arrayIndex);
        }

        bool ICollection<IFakeServiceLocatorObject>.Remove(IFakeServiceLocatorObject item)
        {
            return this.items.Remove(item);
        }

        #endregion ICollection<IFakeCompositeServiceLocatorObject> Members

        #region IEnumerable<IFakeCompositeServiceLocatorObject> Members

        IEnumerator<IFakeServiceLocatorObject> IEnumerable<IFakeServiceLocatorObject>.GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        #endregion IEnumerable<IFakeCompositeServiceLocatorObject> Members

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        #endregion IEnumerable Members
    }
}