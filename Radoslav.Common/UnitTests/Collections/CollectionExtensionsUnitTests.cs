using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Radoslav.Common.UnitTests.Collections
{
    [TestClass]
    public sealed class CollectionExtensionsUnitTests
    {
        [TestMethod]
        public void AtLeast()
        {
            int[] values = new int[] { 1, 2, 3 };

            Assert.IsTrue(values.AtLeast(0, v => v < 0));
            Assert.IsTrue(values.AtLeast(1, v => v > 1));
            Assert.IsFalse(values.AtLeast(2, v => v > 2));
            Assert.IsTrue(values.AtLeast(3, v => v > 0));
        }

        [TestMethod]
        public void RemoveWhere()
        {
            ICollection<int> values = new List<int> { 1, 2, 3, 4 };

            values.Remove(v => v > 1 && v < 4);

            Assert.AreEqual(2, values.Count);

            Assert.IsTrue(values.Contains(1));
            Assert.IsFalse(values.Contains(2));
            Assert.IsFalse(values.Contains(3));
            Assert.IsTrue(values.Contains(4));
        }
    }
}