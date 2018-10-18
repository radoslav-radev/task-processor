using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public sealed class StringTaskSummaryUnitTests
    {
        [TestMethod]
        public void Constructor()
        {
            StringTaskSummary summary1 = new StringTaskSummary("Hello World");

            Assert.AreEqual("Hello World", summary1.Summary);
        }

        [TestMethod]
        public void Equals()
        {
            StringTaskSummary summary = new StringTaskSummary("Hello World");

            Assert.IsTrue(summary.Equals(summary));
            Assert.IsFalse(summary.Equals(default(StringTaskSummary)));
            Assert.IsTrue(summary.Equals(new StringTaskSummary("Hello World")));
            Assert.IsFalse(summary.Equals(new StringTaskSummary("Hello Summary")));

            Assert.IsFalse(summary.Equals(default(string)));
            Assert.IsFalse(summary.Equals(string.Empty));
            Assert.IsTrue(summary.Equals("Hello World"));
            Assert.IsFalse(summary.Equals("Hello Summary"));

            Assert.IsTrue(summary.Equals((object)summary));
            Assert.IsFalse(summary.Equals((object)default(StringTaskSummary)));
            Assert.IsTrue(summary.Equals((object)new StringTaskSummary("Hello World")));
            Assert.IsFalse(summary.Equals((object)new StringTaskSummary("Hello Summary")));

            Assert.IsFalse(summary.Equals((object)default(string)));
            Assert.IsFalse(summary.Equals((object)string.Empty));
            Assert.IsTrue(summary.Equals((object)"Hello World"));
            Assert.IsFalse(summary.Equals((object)"Hello Summary"));

            Assert.IsFalse(summary.Equals(1));
        }

        [TestMethod]
        public void ImplicitNullStringToSummary()
        {
            StringTaskSummary summary = default(string);

            Assert.IsNull(summary.Summary);
        }

        [TestMethod]
        public void ImplicitEmptyStringToSummary()
        {
            StringTaskSummary summary = string.Empty;

            Assert.IsTrue(string.IsNullOrEmpty(summary.Summary));
        }

        [TestMethod]
        public void ImplicitStringToSummary()
        {
            StringTaskSummary summary = "Hello World";

            Assert.AreEqual("Hello World", summary.Summary);
        }

        [TestMethod]
        public void ImplicitSummaryToString()
        {
            Assert.IsNull((string)default(StringTaskSummary));

            string s = new StringTaskSummary("Hello World");

            Assert.AreEqual("Hello World", s);
        }

        [TestMethod]
        public void EqualityOperators()
        {
            Assert.IsTrue(new StringTaskSummary("A") == new StringTaskSummary("A"));
            Assert.IsFalse(new StringTaskSummary("A") == new StringTaskSummary("B"));
            Assert.IsFalse(new StringTaskSummary("A") != new StringTaskSummary("A"));
            Assert.IsTrue(new StringTaskSummary("A") != new StringTaskSummary("B"));

            Assert.IsTrue(new StringTaskSummary("A") == "A");
            Assert.IsFalse(new StringTaskSummary("A") == "B");
            Assert.IsFalse(new StringTaskSummary("A") != "A");
            Assert.IsTrue(new StringTaskSummary("A") != "B");

            Assert.IsTrue("A" == new StringTaskSummary("A"));
            Assert.IsFalse("A" == new StringTaskSummary("B"));
            Assert.IsFalse("A" != new StringTaskSummary("A"));
            Assert.IsTrue("A" != new StringTaskSummary("B"));
        }
    }
}