using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Radoslav.Configuration.Validators
{
    /// <summary>
    /// Performs validation combining several other validators.
    /// </summary>
    public sealed class CompositeValidator : ConfigurationValidatorBase
    {
        private readonly ConfigurationValidatorBase[] validators;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeValidator"/> class.
        /// </summary>
        /// <param name="validators">The validators to use.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="validators"/> is null.</exception>
        public CompositeValidator(params ConfigurationValidatorBase[] validators)
        {
            if (validators == null)
            {
                throw new ArgumentNullException(nameof(validators));
            }

            this.validators = validators;
        }

        internal CompositeValidator(IEnumerable<Type> validatorTypes)
        {
            this.validators = validatorTypes
                .Select(t => t.CreateInstance<ConfigurationValidatorBase>())
                .ToArray();
        }

        /// <inheritdoc />
        public override bool CanValidate(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return this.validators.All(v => v.CanValidate(type));
        }

        /// <inheritdoc />
        public override void Validate(object value)
        {
            foreach (ConfigurationValidatorBase validator in this.validators)
            {
                validator.Validate(value);
            }
        }
    }
}