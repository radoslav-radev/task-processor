using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Radoslav.Common.UnitTests
{
    [TestClass]
    public sealed class HelpersTests
    {
        [TestMethod]
        public void GetValueOrDefault1()
        {
            int? value = null;

            Assert.AreEqual(199, value.GetValueOrDefault(() => 199));
        }

        [TestMethod]
        public void GetValueOrDefault2()
        {
            int? value = 12;

            Assert.AreEqual(12, value.GetValueOrDefault(() => 199));
        }
    }
}