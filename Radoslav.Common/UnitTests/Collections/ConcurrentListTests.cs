using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Collections;

namespace Radoslav.Common.UnitTests
{
    [TestClass]
    public sealed class ConcurrentListTests
    {
        #region Properties

        public TestContext TestContext { get; set; }

        private ConcurrentList<object> ConcurrentList
        {
            get
            {
                return (ConcurrentList<object>)this.TestContext.Properties[typeof(ConcurrentList<>).Name];
            }

            set
            {
                this.TestContext.Properties.Add(typeof(ConcurrentList<>).Name, value);
            }
        }

        private IReadOnlyConcurrentList<object> ReadOnlyList
        {
            get
            {
                return this.ConcurrentList.AsReadOnly();
            }
        }

        #endregion Properties

        #region Initializes & Cleanup

        [TestInitialize]
        public void InitializeTest()
        {
            this.ConcurrentList = new ConcurrentList<object>();
        }

        #endregion Initializes & Cleanup

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ReadOnlyAdd()
        {
            ((ICollection<object>)this.ReadOnlyList).Add(1);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ReadOnlyRemove()
        {
            ((ICollection<object>)this.ReadOnlyList).Remove(1);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ReadOnlyClear()
        {
            ((ICollection<object>)this.ReadOnlyList).Clear();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ReadOnlyInsert()
        {
            ((IList<object>)this.ReadOnlyList).Insert(0, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ReadOnlyRemoveAt()
        {
            ((IList<object>)this.ReadOnlyList).RemoveAt(0);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ReadOnlyRemoveRange()
        {
            ((ConcurrentList<object>)this.ReadOnlyList).RemoveRange(new object[0]);
        }
    }
}