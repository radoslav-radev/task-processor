using System;
using System.Configuration;
using System.Reflection;

namespace Radoslav.Configuration.Validators
{
    /// <summary>
    /// Validates if a <see cref="Type"/> has a default (parameterless) constructor.
    /// </summary>
    public sealed class ParameterlessConstructorValidator : ConfigurationValidatorBase
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

            if (((Type)value).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null) == null)
            {
                throw new ConfigurationErrorsException("Type '{0}' has no default (parameterless) constructor.".FormatInvariant(value));
            }
        }
    }
}