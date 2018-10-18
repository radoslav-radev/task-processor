using System;
using System.Configuration;

namespace Radoslav.Configuration.Validators
{
    /// <summary>
    /// An attribute to validate if a <see cref="Type"/> has a default (parameterless) constructor.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ParameterlessConstructorValidatorAttribute : ConfigurationValidatorAttribute
    {
        /// <inheritdoc />
        public override ConfigurationValidatorBase ValidatorInstance
        {
            get
            {
                return new ParameterlessConstructorValidator();
            }
        }
    }
}