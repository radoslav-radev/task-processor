using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Radoslav.Common.UnitTests
{
    [TestClass]
    public class DateTimeTests
    {
        [TestMethod]
        public void GetRandomDateTime()
        {
            int count = Enumerable.Range(0, 10)
                .Select(i => Helpers.GetRandomDateTime())
                .Distinct()
                .Count();

            Assert.AreEqual(10, count);
        }

        [TestMethod]
        public void GetBeginningOfTimeInterval()
        {
            Assert.AreEqual(DateTime.Today, DateTime.Today.GetBeginningOfTimeInterval(TimeSpan.FromMinutes(15)));
            Assert.AreEqual(DateTime.Today, DateTime.Today.AddMinutes(5).GetBeginningOfTimeInterval(TimeSpan.FromMinutes(15)));
            Assert.AreEqual(DateTime.Today, DateTime.Today.AddMinutes(10).GetBeginningOfTimeInterval(TimeSpan.FromMinutes(15)));
            Assert.AreEqual(DateTime.Today.AddMinutes(15), DateTime.Today.AddMinutes(15).GetBeginningOfTimeInterval(TimeSpan.FromMinutes(15)));
            Assert.AreEqual(DateTime.Today.AddMinutes(15), DateTime.Today.AddMinutes(20).GetBeginningOfTimeInterval(TimeSpan.FromMinutes(15)));
            Assert.AreEqual(DateTime.Today.AddMinutes(30), DateTime.Today.AddMinutes(30).GetBeginningOfTimeInterval(TimeSpan.FromMinutes(15)));
            Assert.AreEqual(DateTime.Today.AddMinutes(30), DateTime.Today.AddMinutes(40).GetBeginningOfTimeInterval(TimeSpan.FromMinutes(15)));
            Assert.AreEqual(DateTime.Today.AddMinutes(45), DateTime.Today.AddMinutes(45).GetBeginningOfTimeInterval(TimeSpan.FromMinutes(15)));
            Assert.AreEqual(DateTime.Today.AddMinutes(45), DateTime.Today.AddMinutes(50).GetBeginningOfTimeInterval(TimeSpan.FromMinutes(15)));
        }

        [TestMethod]
        public void IsHourDivider()
        {
            Assert.IsFalse(TimeSpan.FromMinutes(-1).IsHourDivider());
            Assert.IsFalse(TimeSpan.Zero.IsHourDivider());
            Assert.IsTrue(TimeSpan.FromMinutes(1).IsHourDivider());
            Assert.IsTrue(TimeSpan.FromMinutes(2).IsHourDivider());
            Assert.IsTrue(TimeSpan.FromMinutes(3).IsHourDivider());
            Assert.IsTrue(TimeSpan.FromMinutes(4).IsHourDivider());
            Assert.IsTrue(TimeSpan.FromMinutes(5).IsHourDivider());
            Assert.IsTrue(TimeSpan.FromMinutes(6).IsHourDivider());
            Assert.IsFalse(TimeSpan.FromMinutes(7).IsHourDivider());
            Assert.IsFalse(TimeSpan.FromMinutes(8).IsHourDivider());
            Assert.IsFalse(TimeSpan.FromMinutes(9).IsHourDivider());
            Assert.IsTrue(TimeSpan.FromMinutes(10).IsHourDivider());
            Assert.IsTrue(TimeSpan.FromMinutes(12).IsHourDivider());
            Assert.IsTrue(TimeSpan.FromMinutes(15).IsHourDivider());
            Assert.IsTrue(TimeSpan.FromMinutes(20).IsHourDivider());
            Assert.IsFalse(TimeSpan.FromMinutes(25).IsHourDivider());
            Assert.IsTrue(TimeSpan.FromMinutes(30).IsHourDivider());
            Assert.IsFalse(TimeSpan.FromMinutes(40).IsHourDivider());
            Assert.IsFalse(TimeSpan.FromMinutes(45).IsHourDivider());
            Assert.IsFalse(TimeSpan.FromMinutes(60).IsHourDivider());
        }
    }
}