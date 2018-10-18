using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Radoslav.Common.UnitTests
{
    [TestClass]
    public sealed class MockObjectTests
    {
        [TestMethod]
        public void PredefineResult()
        {
            FakeMockObject fake = new FakeMockObject();

            fake.PredefineResult(3, f => f.Method1(1, 2));

            int result = fake.Method1(1, 2);

            Assert.AreEqual(3, result);
        }
    }
}