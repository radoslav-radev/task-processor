using System;
using System.Configuration;

namespace Radoslav.Configuration.Validators
{
    /// <summary>
    /// An attribute to validate if a <see cref="Type"/> is assignable from another.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class AssignableFromTypeValidatorAttribute : ConfigurationValidatorAttribute
    {
        private readonly Type assignableFromType;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssignableFromTypeValidatorAttribute"/> class.
        /// </summary>
        /// <param name="assignableFromType">The type the the validated <see cref="Type"/> must be assignable from.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="assignableFromType"/> is null.</exception>
        public AssignableFromTypeValidatorAttribute(Type assignableFromType)
        {
            if (assignableFromType == null)
            {
                throw new ArgumentNullException("assignableFromType");
            }

            this.assignableFromType = assignableFromType;
        }

        /// <summary>
        /// Gets the <see cref="Type"/> that the validated <see cref="Type"/> must be assignable from.
        /// </summary>
        /// <value>The type the the validated <see cref="Type"/> must be assignable from.</value>
        public Type AssignableFromType
        {
            get { return this.assignableFromType; }
        }

        /// <inheritdoc />
        public override ConfigurationValidatorBase ValidatorInstance
        {
            get
            {
                return new AssignableFromTypeValidator(this.assignableFromType);
            }
        }
    }
}