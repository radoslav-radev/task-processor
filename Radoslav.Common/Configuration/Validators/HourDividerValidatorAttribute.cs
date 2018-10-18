using System;
using System.Configuration;

namespace Radoslav.Configuration.Validators
{
    /// <summary>Provides validates that a <see cref="TimeSpan" /> value divides an hour into equal parts.</summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class HourDividerValidatorAttribute : ConfigurationValidatorAttribute
    {
        /// <inheritdoc />
        public override ConfigurationValidatorBase ValidatorInstance
        {
            get
            {
                return new HourDividerValidator();
            }
        }
    }
}