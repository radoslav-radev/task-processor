using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Configuration.Validators;

namespace Radoslav.Common.UnitTests
{
    [TestClass]
    public sealed class NullableIntegerValidatorTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CanValidateNull()
        {
            NullableIntegerValidatorAttribute attr = new NullableIntegerValidatorAttribute();

            attr.ValidatorInstance.CanValidate(null);
        }

        [TestMethod]
        public void CanValidateNullableInt()
        {
            NullableIntegerValidatorAttribute attr = new NullableIntegerValidatorAttribute();

            Assert.IsTrue(attr.ValidatorInstance.CanValidate(typeof(int?)));
        }

        [TestMethod]
        public void ValidateNull()
        {
            NullableIntegerValidatorAttribute attr = new NullableIntegerValidatorAttribute();

            attr.ValidatorInstance.Validate(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ValidateMin()
        {
            NullableIntegerValidatorAttribute attr = new NullableIntegerValidatorAttribute()
            {
                MinValue = 1
            };

            attr.ValidatorInstance.Validate(0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ValidateMax()
        {
            NullableIntegerValidatorAttribute attr = new NullableIntegerValidatorAttribute()
            {
                MaxValue = 10
            };

            attr.ValidatorInstance.Validate(11);
        }
    }
}