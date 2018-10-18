using System;
using System.Configuration;

namespace Radoslav.Configuration.Validators
{
    /// <summary>
    /// Validates that a <see cref="TimeSpan" /> value divides an hour into equal parts.
    /// </summary>
    public sealed class HourDividerValidator : ConfigurationValidatorBase
    {
        /// <inheritdoc />
        public override bool CanValidate(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return type == typeof(TimeSpan);
        }

        /// <inheritdoc />
        public override void Validate(object value)
        {
            if (value == null)
            {
                // May be value is not required - in this case we assume it is correct. If value is mandatory, use IsRequired=true in System.Configuration.ConfigurationProperty attribute.
                return;
            }

            if (!((TimeSpan)value).IsHourDivider())
            {
                throw new ConfigurationErrorsException("{0} '{1}' does not divide hour into equal parts.".FormatInvariant(value.GetType().Name, value));
            }
        }
    }
}