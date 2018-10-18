using System;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Configuration.Validators;

namespace Radoslav.Common.UnitTests.Configuration
{
    [TestClass]
    public sealed class HourDividerValidatorTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CanValidateNull()
        {
            HourDividerValidator validator = new HourDividerValidator();

            validator.CanValidate(null);
        }

        [TestMethod]
        public void CanValidateInt()
        {
            HourDividerValidator validator = new HourDividerValidator();

            Assert.IsFalse(validator.CanValidate(typeof(int)));
        }

        [TestMethod]
        public void CanValidateTimeSpan()
        {
            HourDividerValidator validator = new HourDividerValidator();

            Assert.IsTrue(validator.CanValidate(typeof(TimeSpan)));
        }

        [TestMethod]
        public void Validate1Min()
        {
            HourDividerValidator validator = new HourDividerValidator();

            validator.Validate(TimeSpan.FromMinutes(1));
        }

        [TestMethod]
        public void Validate2Min()
        {
            HourDividerValidator validator = new HourDividerValidator();

            validator.Validate(TimeSpan.FromMinutes(2));
        }

        [TestMethod]
        public void Validate3Min()
        {
            HourDividerValidator validator = new HourDividerValidator();

            validator.Validate(TimeSpan.FromMinutes(3));
        }

        [TestMethod]
        public void Validate4Min()
        {
            HourDividerValidator validator = new HourDividerValidator();

            validator.Validate(TimeSpan.FromMinutes(4));
        }

        [TestMethod]
        public void Validate5Min()
        {
            HourDividerValidator validator = new HourDividerValidator();

            validator.Validate(TimeSpan.FromMinutes(5));
        }

        [TestMethod]
        public void Validate6Min()
        {
            HourDividerValidator validator = new HourDividerValidator();

            validator.Validate(TimeSpan.FromMinutes(6));
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void Validate7Min()
        {
            HourDividerValidator validator = new HourDividerValidator();

            validator.Validate(TimeSpan.FromMinutes(7));
        }

        [TestMethod]
        public void Validate10Min()
        {
            HourDividerValidator validator = new HourDividerValidator();

            validator.Validate(TimeSpan.FromMinutes(10));
        }

        [TestMethod]
        public void Validate15Min()
        {
            HourDividerValidator validator = new HourDividerValidator();

            validator.Validate(TimeSpan.FromMinutes(15));
        }

        [TestMethod]
        public void Validate20Min()
        {
            HourDividerValidator validator = new HourDividerValidator();

            validator.Validate(TimeSpan.FromMinutes(20));
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void Validate25Min()
        {
            HourDividerValidator validator = new HourDividerValidator();

            validator.Validate(TimeSpan.FromMinutes(25));
        }

        [TestMethod]
        public void Validate30Min()
        {
            HourDividerValidator validator = new HourDividerValidator();

            validator.Validate(TimeSpan.FromMinutes(30));
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void Validate45Min()
        {
            HourDividerValidator validator = new HourDividerValidator();

            validator.Validate(TimeSpan.FromMinutes(45));
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void Validate60Min()
        {
            HourDividerValidator validator = new HourDividerValidator();

            validator.Validate(TimeSpan.FromMinutes(60));
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void Validate70Min()
        {
            HourDividerValidator validator = new HourDividerValidator();

            validator.Validate(TimeSpan.FromMinutes(70));
        }
    }
}