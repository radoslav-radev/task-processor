using System;
using System.Configuration;

namespace Radoslav.Configuration.Validators
{
    /// <summary>
    /// Provides validation of nullable integer value.
    /// </summary>
    internal sealed class NullableIntegerValidator : IntegerValidator
    {
        internal NullableIntegerValidator(int minValue, int maxValue)
            : base(minValue, maxValue)
        {
        }

        /// <inheritdoc />
        public override bool CanValidate(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return type == typeof(int?);
        }

        /// <inheritdoc />
        public override void Validate(object value)
        {
            if (value != null)
            {
                base.Validate(value);
            }
        }
    }
}