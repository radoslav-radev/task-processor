using System;
using System.Configuration;

namespace Radoslav.Configuration.Validators
{
    /// <summary>
    /// An attribute to validate a nullable integer value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class NullableIntegerValidatorAttribute : ConfigurationValidatorAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullableIntegerValidatorAttribute"/> class.
        /// </summary>
        public NullableIntegerValidatorAttribute()
        {
            this.MinValue = int.MinValue;
            this.MaxValue = int.MaxValue;
        }

        /// <summary>
        /// Gets or sets the minimum value allowed for the property.
        /// </summary>
        /// <value>The minimum value allowed for the property.</value>
        public int MinValue { get; set; }

        /// <summary>
        /// Gets or sets the maximum value allowed for the property.
        /// </summary>
        /// <value>The maximum value allowed for the property.</value>
        public int MaxValue { get; set; }

        /// <inheritdoc />
        public override ConfigurationValidatorBase ValidatorInstance
        {
            get
            {
                return new NullableIntegerValidator(this.MinValue, this.MaxValue);
            }
        }
    }
}