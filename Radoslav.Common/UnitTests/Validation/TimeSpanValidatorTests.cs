using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Configuration.Validators;

namespace Radoslav.Common.UnitTests.Configuration
{
    [TestClass]
    public sealed class TimeSpanValidatorTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CanValidateNull()
        {
            TimeSpanRangeValidatorAttribute attr = new TimeSpanRangeValidatorAttribute();

            attr.ValidatorInstance.CanValidate(null);
        }

        [TestMethod]
        public void CanValidateInt()
        {
            TimeSpanRangeValidatorAttribute attr = new TimeSpanRangeValidatorAttribute();

            Assert.IsFalse(attr.ValidatorInstance.CanValidate(typeof(int)));
        }

        [TestMethod]
        public void CanValidateTimeSpan()
        {
            TimeSpanRangeValidatorAttribute attr = new TimeSpanRangeValidatorAttribute();

            Assert.IsTrue(attr.ValidatorInstance.CanValidate(typeof(TimeSpan)));
        }

        [TestMethod]
        public void ValidateNull()
        {
            TimeSpanRangeValidatorAttribute attr = new TimeSpanRangeValidatorAttribute();

            attr.ValidatorInstance.Validate(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MinGreaterThanMax()
        {
            new TimeSpanRangeValidatorAttribute()
            {
                MinValueInMilliseconds = (int)TimeSpan.FromMinutes(2).TotalMilliseconds,
                MaxValueInMilliseconds = (int)TimeSpan.FromMinutes(1).TotalMilliseconds
            };
        }

        [TestMethod]
        public void MinEqualToMax()
        {
            TimeSpanRangeValidatorAttribute attr = new TimeSpanRangeValidatorAttribute()
            {
                MinValueInMilliseconds = (int)TimeSpan.FromMinutes(1).TotalMilliseconds,
                MaxValueInMilliseconds = (int)TimeSpan.FromMinutes(1).TotalMilliseconds
            };

            attr.ValidatorInstance.Validate(TimeSpan.FromMinutes(1));
        }

        [TestMethod]
        public void MinLessThanMax()
        {
            new TimeSpanRangeValidatorAttribute()
            {
                MinValueInMilliseconds = (int)TimeSpan.FromMinutes(1).TotalMilliseconds,
                MaxValueInMilliseconds = (int)TimeSpan.FromMinutes(2).TotalMilliseconds
            };
        }

        [TestMethod]
        public void ValueIsLessThanMin()
        {
            new TimeSpanRangeValidatorAttribute()
            {
                MinValueInMilliseconds = (int)TimeSpan.FromMinutes(1).TotalMilliseconds,
                MaxValueInMilliseconds = (int)TimeSpan.FromMinutes(2).TotalMilliseconds
            };
        }

        [TestMethod]
        public void ValueIsGreaterThanMax()
        {
            new TimeSpanRangeValidatorAttribute()
            {
                MinValueInMilliseconds = (int)TimeSpan.FromMinutes(1).TotalMilliseconds,
                MaxValueInMilliseconds = (int)TimeSpan.FromMinutes(2).TotalMilliseconds
            };
        }

        [TestMethod]
        public void ValueInRange()
        {
            new TimeSpanRangeValidatorAttribute()
            {
                MinValueInMilliseconds = (int)TimeSpan.FromMinutes(1).TotalMilliseconds,
                MaxValueInMilliseconds = (int)TimeSpan.FromMinutes(2).TotalMilliseconds
            };
        }
    }
}