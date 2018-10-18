using System;
using System.Configuration;

namespace Radoslav.Configuration.Validators
{
    /// <summary>
    /// Validates that a <see cref="Type"/> is not abstract.
    /// </summary>
    public sealed class NotAbstractClassValidator : ConfigurationValidatorBase
    {
        /// <inheritdoc />
        public override bool CanValidate(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return type == typeof(Type);
        }

        /// <inheritdoc />
        public override void Validate(object value)
        {
            if (value == null)
            {
                // May be value is not required - in this case we assume it is correct. If value is mandatory, use IsRequired=true in System.Configuration.ConfigurationProperty attribute.
                return;
            }

            if (((Type)value).IsAbstract)
            {
                throw new ConfigurationErrorsException("Type '{0}' is abstract.".FormatInvariant(value));
            }
        }
    }
}