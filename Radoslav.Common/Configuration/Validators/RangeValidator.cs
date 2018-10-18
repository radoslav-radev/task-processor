using System;
using System.Configuration;

namespace Radoslav.Configuration.Validators
{
    internal sealed class RangeValidator<T> : ConfigurationValidatorBase
        where T : IComparable<T>
    {
        private readonly T minValue;
        private readonly T maxValue;

        internal RangeValidator(T minValue, T maxValue)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        public override bool CanValidate(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return typeof(T).IsAssignableFrom(type);
        }

        public override void Validate(object value)
        {
            if (value == null)
            {
                // May be value is not required - in this case we assume it is correct. If value is mandatory, use IsRequired=true in System.Configuration.ConfigurationProperty attribute.
                return;
            }

            if ((this.minValue.CompareTo((T)value) > 0) || (this.maxValue.CompareTo((T)value) < 0))
            {
                throw new ConfigurationErrorsException("{0} {1} should be between {2} and {3}.".FormatInvariant(typeof(T), value, this.minValue, this.maxValue));
            }
        }
    }
}