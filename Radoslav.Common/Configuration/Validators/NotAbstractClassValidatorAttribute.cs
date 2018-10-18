using System;
using System.Configuration;

namespace Radoslav.Configuration.Validators
{
    /// <summary>
    /// An attribute to validate that a <see cref="Type"/> is not abstract.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class NotAbstractClassValidatorAttribute : ConfigurationValidatorAttribute
    {
        /// <inheritdoc />
        public override ConfigurationValidatorBase ValidatorInstance
        {
            get
            {
                return new NotAbstractClassValidator();
            }
        }
    }
}