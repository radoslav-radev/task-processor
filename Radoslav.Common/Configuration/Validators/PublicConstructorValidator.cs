using System;
using System.Configuration;

namespace Radoslav.Configuration.Validators
{
    /// <summary>
    /// Validates if a <see cref="Type"/> has a public constructor.
    /// </summary>
    public sealed class PublicConstructorValidator : ConfigurationValidatorBase
    {
        /// <inheritdoc />
        public override bool CanValidate(Type type)
        {
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

            if (((Type)value).GetConstructors().IsEmpty())
            {
                throw new ConfigurationErrorsException("Type '{0}' has no public constructor.".FormatInvariant(value));
            }
        }
    }
}