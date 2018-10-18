using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Redis;
using Radoslav.Timers;

namespace Radoslav.Common.UnitTests
{
    [TestClass]
    public sealed class RedisConverterUnitTests
    {
        [TestMethod]
        public void IsSupported()
        {
            Assert.IsTrue(RedisConverter.IsSupported(typeof(string)));
            Assert.IsTrue(RedisConverter.IsSupported(typeof(bool)));
            Assert.IsTrue(RedisConverter.IsSupported(typeof(bool?)));
            Assert.IsTrue(RedisConverter.IsSupported(typeof(int)));
            Assert.IsTrue(RedisConverter.IsSupported(typeof(int?)));
            Assert.IsTrue(RedisConverter.IsSupported(typeof(long)));
            Assert.IsTrue(RedisConverter.IsSupported(typeof(long?)));
            Assert.IsTrue(RedisConverter.IsSupported(typeof(Guid)));
            Assert.IsTrue(RedisConverter.IsSupported(typeof(Guid?)));
            Assert.IsTrue(RedisConverter.IsSupported(typeof(TimeSpan)));
            Assert.IsTrue(RedisConverter.IsSupported(typeof(TimeSpan?)));
            Assert.IsTrue(RedisConverter.IsSupported(typeof(DateTime)));
            Assert.IsTrue(RedisConverter.IsSupported(typeof(DateTime?)));
            Assert.IsTrue(RedisConverter.IsSupported(typeof(DisposeState)));
            Assert.IsTrue(RedisConverter.IsSupported(typeof(DisposeState?)));
            Assert.IsTrue(RedisConverter.IsSupported(typeof(Type)));
            Assert.IsTrue(RedisConverter.IsSupported(typeof(Version)));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IsSupportedNull()
        {
            RedisConverter.IsSupported(null);
        }

        [TestMethod]
        public void ParseBoolean()
        {
            Assert.AreEqual(true, RedisConverter.ParseBoolean(RedisConverter.ToString(true)));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParseBooleanNull()
        {
            RedisConverter.ParseBoolean(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseBooleanEmpty()
        {
            RedisConverter.ParseBoolean(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseBooleanInvalid()
        {
            RedisConverter.ParseBoolean("Dummy");
        }

        [TestMethod]
        public void ParseDateTime()
        {
            Assert.AreEqual(DateTime.Today, RedisConverter.ParseDateTime(RedisConverter.ToString(DateTime.Today)));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParseDateTimeNull()
        {
            RedisConverter.ParseDateTime(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseDateTimeEmpty()
        {
            RedisConverter.ParseDateTime(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseDateTimeInvalid()
        {
            RedisConverter.ParseDateTime("Dummy");
        }

        [TestMethod]
        public void ParseDateTimeOrNullNull()
        {
            Assert.IsNull(RedisConverter.ParseValue<DateTime?>(null));
        }

        [TestMethod]
        public void ParseDateTimeOrNullValue()
        {
            Assert.AreEqual(DateTime.Today, RedisConverter.ParseDateTimeOrNull(RedisConverter.ToString(DateTime.Today)));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseDateTimeOrNullInvalid()
        {
            RedisConverter.ParseDateTimeOrNull("Dummy");
        }

        [TestMethod]
        public void ParseEnum()
        {
            Assert.AreEqual(DisposeState.None, RedisConverter.ParseEnum<DisposeState>(RedisConverter.ToString(DisposeState.None)));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParseEnumNull()
        {
            RedisConverter.ParseEnum<DisposeState>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseEnumEmpty()
        {
            RedisConverter.ParseEnum<DisposeState>(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseEnumInvalid()
        {
            RedisConverter.ParseEnum<DisposeState>("Dummy");
        }

        [TestMethod]
        public void ParseGuid()
        {
            Assert.AreEqual(Guid.Empty, RedisConverter.ParseGuid(RedisConverter.ToString(Guid.Empty)));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParseGuidNull()
        {
            RedisConverter.ParseGuid(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseGuidEmpty()
        {
            RedisConverter.ParseGuid(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseGuidInvalid()
        {
            RedisConverter.ParseGuid("Dummy");
        }

        [TestMethod]
        public void ParseGuidOrNullNull()
        {
            Assert.IsNull(RedisConverter.ParseValue<Guid?>(null));
        }

        [TestMethod]
        public void ParseGuidOrNullValue()
        {
            Assert.AreEqual(Guid.Empty, RedisConverter.ParseGuidOrNull(RedisConverter.ToString(Guid.Empty)));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseGuideOrNullInvalid()
        {
            RedisConverter.ParseDateTimeOrNull("Dummy");
        }

        [TestMethod]
        public void ParseLong()
        {
            Assert.AreEqual(long.MaxValue, RedisConverter.ParseLong(RedisConverter.ToString(long.MaxValue)));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParseLongNull()
        {
            RedisConverter.ParseLong(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseLongEmpty()
        {
            RedisConverter.ParseLong(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseLongInvalid()
        {
            RedisConverter.ParseLong("Dummy");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseLongOverflow()
        {
            RedisConverter.ParseLong("12345678901234567890");
        }

        [TestMethod]
        public void ParseInt()
        {
            Assert.AreEqual(int.MaxValue, RedisConverter.ParseInteger(RedisConverter.ToString(int.MaxValue)));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParseIntNull()
        {
            RedisConverter.ParseInteger(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseIntEmpty()
        {
            RedisConverter.ParseInteger(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseIntInvalid()
        {
            RedisConverter.ParseInteger("Dummy");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseIntOverflow()
        {
            RedisConverter.ParseInteger("12345678901234567890");
        }

        [TestMethod]
        public void ParseTimeSpan()
        {
            Assert.AreEqual(DateTime.Today.TimeOfDay, RedisConverter.ParseTimeSpan(RedisConverter.ToString(DateTime.Today.TimeOfDay)));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParseTimeSpanNull()
        {
            RedisConverter.ParseTimeSpan(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseTimeSpanEmpty()
        {
            RedisConverter.ParseTimeSpan(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseTimeSpanInvalid()
        {
            RedisConverter.ParseTimeSpan("Dummy");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParseTypeNull()
        {
            RedisConverter.ParseType(null);
        }

        [TestMethod]
        public void ParseType()
        {
            Assert.AreEqual(typeof(FakeEntity), RedisConverter.ParseType(RedisConverter.ToString(typeof(FakeEntity), false)));
            Assert.AreEqual(typeof(FakeEntity), RedisConverter.ParseType(RedisConverter.ToString(typeof(FakeEntity), true)));
        }

        [TestMethod]
        public void ParseValue()
        {
            Assert.IsNull(RedisConverter.ParseValue<string>(null));
            Assert.IsNull(RedisConverter.ParseValue<bool?>(null));
            Assert.IsNull(RedisConverter.ParseValue<int?>(null));
            Assert.IsNull(RedisConverter.ParseValue<long?>(null));
            Assert.IsNull(RedisConverter.ParseValue<DateTime?>(null));
            Assert.IsNull(RedisConverter.ParseValue<TimeSpan?>(null));
            Assert.IsNull(RedisConverter.ParseValue<DisposeState?>(null));

            Assert.AreEqual(true, RedisConverter.ParseValue<bool>(RedisConverter.ToString(true)));
            Assert.AreEqual(false, RedisConverter.ParseValue<bool?>(RedisConverter.ToString(false)));

            Assert.AreEqual(int.MinValue, RedisConverter.ParseValue<int>(RedisConverter.ToString(int.MinValue)));
            Assert.AreEqual(int.MaxValue, RedisConverter.ParseValue<int?>(RedisConverter.ToString(int.MaxValue)));

            Assert.AreEqual(long.MinValue, RedisConverter.ParseValue<long>(RedisConverter.ToString(long.MinValue)));
            Assert.AreEqual(long.MaxValue, RedisConverter.ParseValue<long?>(RedisConverter.ToString(long.MaxValue)));

            Assert.AreEqual(Guid.Empty, RedisConverter.ParseValue<Guid>(RedisConverter.ToString(Guid.Empty)));
            Assert.AreEqual(Guid.Empty, RedisConverter.ParseValue<Guid?>(RedisConverter.ToString(Guid.Empty)));

            Assert.AreEqual(DateTime.MinValue, RedisConverter.ParseValue<DateTime>(RedisConverter.ToString(DateTime.MinValue)));
            Assert.AreEqual(DateTime.MaxValue, RedisConverter.ParseValue<DateTime?>(RedisConverter.ToString(DateTime.MaxValue)));

            Assert.AreEqual(TimeSpan.MinValue, RedisConverter.ParseValue<TimeSpan>(RedisConverter.ToString(TimeSpan.MinValue)));
            Assert.AreEqual(TimeSpan.MaxValue, RedisConverter.ParseValue<TimeSpan?>(RedisConverter.ToString(TimeSpan.MaxValue)));

            Assert.AreEqual(DisposeState.Disposing, RedisConverter.ParseValue<DisposeState>(RedisConverter.ToString(DisposeState.Disposing)));
            Assert.AreEqual(DisposeState.Disposed, RedisConverter.ParseValue<DisposeState?>(RedisConverter.ToString(DisposeState.Disposed)));

            Assert.AreEqual(typeof(FakeEntity), RedisConverter.ParseValue<Type>(RedisConverter.ToString(typeof(FakeEntity), false)));
            Assert.AreEqual(typeof(FakeEntity), RedisConverter.ParseValue<Type>(RedisConverter.ToString(typeof(FakeEntity), true)));

            Assert.AreEqual(new Version(1, 2, 3, 4), RedisConverter.ParseValue<Version>(RedisConverter.ToString(new Version(1, 2, 3, 4))));
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException), AllowDerivedTypes = true)]
        public void ParseValueNotSupported()
        {
            RedisConverter.ParseValue<ITimer>("Hello World");
        }

        [TestMethod]
        public void ToStringNull()
        {
            Assert.AreEqual(string.Empty, RedisConverter.ToString(default(string)));
        }

        [TestMethod]
        public void ParseCollectionInteger()
        {
            RedisConverterUnitTests.ParseCollection(1, 2, 3);
        }

        [TestMethod]
        public void ParseCollectionString()
        {
            RedisConverterUnitTests.ParseCollection("Hello", "World");
        }

        [TestMethod]
        public void ParseCollectionNull()
        {
            Assert.IsFalse(RedisConverter.ParseCollection<string>(null).Any());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseCollectionInvalid()
        {
            RedisConverter.ParseCollection<int>("Dummy").Any();
        }

        [TestMethod]
        public void ToStringCollectionNull()
        {
            Assert.AreEqual(string.Empty, RedisConverter.ToString(default(object[])));
        }

        private static void ParseCollection<T>(params T[] values)
        {
            string value = RedisConverter.ToString(values);

            IEnumerable<T> values2 = RedisConverter.ParseCollection<T>(value);

            CollectionAssert.AreEquivalent(values, values2.ToList());
        }
    }
}