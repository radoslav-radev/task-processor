using System;
using System.Configuration;

namespace Radoslav.Configuration.Validators
{
    /// <summary>
    /// Validates if a <see cref="Type"/> is assignable from another.
    /// </summary>
    public sealed class AssignableFromTypeValidator : ConfigurationValidatorBase
    {
        private readonly Type assignableFromType;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssignableFromTypeValidator"/> class.
        /// </summary>
        /// <param name="assignableFromType">The type the the validated type must be assignable from.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="assignableFromType"/> is null.</exception>
        public AssignableFromTypeValidator(Type assignableFromType)
        {
            if (assignableFromType == null)
            {
                throw new ArgumentNullException("assignableFromType");
            }

            this.assignableFromType = assignableFromType;
        }

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

            if (!this.assignableFromType.IsAssignableFrom((Type)value))
            {
                throw new ConfigurationErrorsException("Type '{0}' is not assignable from '{1}'.".FormatInvariant(this.assignableFromType, value));
            }
        }
    }
}