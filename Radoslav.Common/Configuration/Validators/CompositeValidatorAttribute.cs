using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;

namespace Radoslav.Configuration.Validators
{
    /// <summary>
    /// Provides validation combining various validators.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class CompositeValidatorAttribute : ConfigurationValidatorAttribute
    {
        private readonly ReadOnlyCollection<Type> validatorTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeValidatorAttribute"/> class.
        /// </summary>
        /// <param name="validatorTypes">The validator types to be used for validation. Each one of them must inherit from <seealso cref="ConfigurationValidatorBase"/>.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="validatorTypes"/> is null.</exception>
        /// <exception cref="ArgumentException">Parameter <paramref name="validatorTypes"/> is an empty collection or any of the validator types does not inherit from <seealso cref="ConfigurationValidatorBase"/>.</exception>
        public CompositeValidatorAttribute(params Type[] validatorTypes)
        {
            if (validatorTypes == null)
            {
                throw new ArgumentNullException("validatorTypes");
            }

            if (validatorTypes.Length == 0)
            {
                throw new ArgumentException("{0} should specify at least one configuration validator type.".FormatInvariant(this.GetType().Name));
            }

            foreach (Type validatorType in validatorTypes)
            {
                if (!typeof(ConfigurationValidatorBase).IsAssignableFrom(validatorType))
                {
                    throw new ArgumentException("Specified validator type '{0}' does not inherit '{1}'.".FormatInvariant(validatorType, typeof(ConfigurationValidatorBase)));
                }
            }

            this.validatorTypes = Array.AsReadOnly(validatorTypes);
        }

        /// <summary>Gets the validator types.</summary>
        /// <value>The validator types to be used in validation.</value>
        public IEnumerable<Type> ValidatorTypes
        {
            get { return this.validatorTypes; }
        }

        /// <inheritdoc />
        public override ConfigurationValidatorBase ValidatorInstance
        {
            get
            {
                return new CompositeValidator(this.validatorTypes);
            }
        }
    }
}