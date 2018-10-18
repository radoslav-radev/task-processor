using System;
using System.Configuration;

namespace Radoslav.Configuration.Validators
{
    /// <summary>
    /// Provides validation whether a <see cref="TimeSpan" /> value is within an expected range.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class TimeSpanRangeValidatorAttribute : ConfigurationValidatorAttribute
    {
        private TimeSpan minValue;
        private TimeSpan maxValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeSpanRangeValidatorAttribute"/> class.
        /// </summary>
        public TimeSpanRangeValidatorAttribute()
        {
            this.minValue = TimeSpan.MinValue;
            this.maxValue = TimeSpan.MaxValue;
        }

        /// <inheritdoc />
        public override ConfigurationValidatorBase ValidatorInstance
        {
            get
            {
                return new RangeValidator<TimeSpan>(this.minValue, this.maxValue);
            }
        }

        /// <summary>
        /// Gets or sets the minimum allowed <see cref="TimeSpan"/> value in milliseconds.
        /// </summary>
        /// <value>The minimum allowed <see cref="TimeSpan"/> value in milliseconds.</value>
        public int MinValueInMilliseconds
        {
            get
            {
                return Convert.ToInt32(this.minValue.TotalMilliseconds);
            }

            set
            {
                this.MinValue = TimeSpan.FromMilliseconds(value);
            }
        }

        /// <summary>
        /// Gets or sets the maximum allowed <see cref="TimeSpan"/> value in milliseconds.
        /// </summary>
        /// <value>The maximum allowed <see cref="TimeSpan"/> value in milliseconds.</value>
        public int MaxValueInMilliseconds
        {
            get
            {
                return Convert.ToInt32(this.maxValue.TotalMilliseconds);
            }

            set
            {
                this.MaxValue = TimeSpan.FromMilliseconds(value);
            }
        }

        private TimeSpan MinValue
        {
            get
            {
                return this.minValue;
            }

            set
            {
                if (value > this.maxValue)
                {
                    throw new ArgumentOutOfRangeException("Value {0} is greater than max value {1}.".FormatInvariant(value, this.maxValue));
                }

                this.minValue = value;
            }
        }

        private TimeSpan MaxValue
        {
            get
            {
                return this.maxValue;
            }

            set
            {
                if (value < this.minValue)
                {
                    throw new ArgumentOutOfRangeException("Value {0} is less than min value {1}.".FormatInvariant(value, this.minValue));
                }

                this.maxValue = value;
            }
        }
    }
}