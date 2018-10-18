using System;

namespace Radoslav.TaskProcessor.Model
{
    /// <summary>
    /// An implementation of the <see cref="ITaskSummary"/> interface that uses a string for summary.
    /// </summary>
    /// <remarks>This class must be serializable to binary, XML, JSON and/or other formats.</remarks>
    [Serializable]
    public struct StringTaskSummary : ITaskSummary, IEquatable<StringTaskSummary>, IEquatable<string>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StringTaskSummary"/> struct.
        /// </summary>
        /// <param name="summary">The summary for the task.</param>
        public StringTaskSummary(string summary)
        {
            this.Summary = summary;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the summary for the task.
        /// </summary>
        /// <value>The summary for the task.</value>
        public string Summary { get; set; }

        #endregion Properties

        #region Operators

        /// <summary>
        /// Converts implicitly a <see cref="StringTaskSummary"/> to a <see cref="string"/>.
        /// </summary>
        /// <param name="summary">The summary to convert.</param>
        public static implicit operator string (StringTaskSummary summary)
        {
            return summary.Summary;
        }

        /// <summary>
        /// Converts implicitly a <see cref="string"/> to a <see cref="StringTaskSummary"/>.
        /// </summary>
        /// <param name="summary">The string to convert.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="summary"/> is null or empty.</exception>
        public static implicit operator StringTaskSummary(string summary)
        {
            return new StringTaskSummary(summary);
        }

        /// <summary>
        /// Checks if two instance of the <see cref="StringTaskSummary"/> class are equal.
        /// </summary>
        /// <param name="summary1">The first <see cref="StringTaskSummary"/> instance to compare.</param>
        /// <param name="summary2">The second <see cref="StringTaskSummary"/> instance to compare.</param>
        /// <returns>True if the specified instances are equal; otherwise false.</returns>
        public static bool operator ==(StringTaskSummary summary1, StringTaskSummary summary2)
        {
            return string.Equals(summary1.Summary, summary2.Summary);
        }

        /// <summary>
        /// Checks if two instance of the <see cref="StringTaskSummary"/> class are not equal.
        /// </summary>
        /// <param name="summary1">The first <see cref="StringTaskSummary"/> instance to compare.</param>
        /// <param name="summary2">The second <see cref="StringTaskSummary"/> instance to compare.</param>
        /// <returns>True if the specified instances are not equal; otherwise false.</returns>
        public static bool operator !=(StringTaskSummary summary1, StringTaskSummary summary2)
        {
            return !string.Equals(summary1.Summary, summary2.Summary);
        }

        /// <summary>
        /// Checks if an instance of the <see cref="StringTaskSummary"/> class is equal to an instance of the <see cref="String"/> class.
        /// </summary>
        /// <param name="summary1">The <see cref="StringTaskSummary"/> instance to compare.</param>
        /// <param name="summary2">The <see cref="string"/> instance to compare.</param>
        /// <returns>True if the specified instances are equal; otherwise false.</returns>
        public static bool operator ==(StringTaskSummary summary1, string summary2)
        {
            return string.Equals(summary1.Summary, summary2);
        }

        /// <summary>
        /// Checks if an instance of the <see cref="StringTaskSummary"/> class is not equal to an instance of the <see cref="String"/> class.
        /// </summary>
        /// <param name="summary1">The <see cref="StringTaskSummary"/> instance to compare.</param>
        /// <param name="summary2">The <see cref="string"/> instance to compare.</param>
        /// <returns>True if the specified instances are not equal; otherwise false.</returns>
        public static bool operator !=(StringTaskSummary summary1, string summary2)
        {
            return !string.Equals(summary1.Summary, summary2);
        }

        /// <summary>
        /// Checks if an instance of the <see cref="string"/> class is equal to an instance of the <see cref="StringTaskSummary"/> class.
        /// </summary>
        /// <param name="summary1">The <see cref="string"/> instance to compare.</param>
        /// <param name="summary2">The <see cref="StringTaskSummary"/> instance to compare.</param>
        /// <returns>True if the specified instances are equal; otherwise false.</returns>
        public static bool operator ==(string summary1, StringTaskSummary summary2)
        {
            return string.Equals(summary1, summary2.Summary);
        }

        /// <summary>
        /// Checks if an instance of the <see cref="string"/> class is not equal to an instance of the <see cref="StringTaskSummary"/> class.
        /// </summary>
        /// <param name="summary1">The <see cref="string"/> instance to compare.</param>
        /// <param name="summary2">The <see cref="StringTaskSummary"/> instance to compare.</param>
        /// <returns>True if the specified instances are not equal; otherwise false.</returns>
        public static bool operator !=(string summary1, StringTaskSummary summary2)
        {
            return !string.Equals(summary1, summary2.Summary);
        }

        #endregion Operators

        #region IEquatable<StringTaskSummary> Members

        /// <inheritdoc />
        public bool Equals(StringTaskSummary other)
        {
            return string.Equals(this.Summary, other.Summary);
        }

        #endregion IEquatable<StringTaskSummary> Members

        #region IEquatable<string> Members

        /// <inheritdoc />
        public bool Equals(string other)
        {
            return string.Equals(this.Summary, other);
        }

        #endregion IEquatable<string> Members

        #region Object Methods

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType() == typeof(StringTaskSummary))
            {
                return string.Equals(this.Summary, ((StringTaskSummary)obj).Summary);
            }

            if (obj is string)
            {
                return string.Equals(this.Summary, (string)obj);
            }

            return false;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return this.Summary.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return this.Summary;
        }

        #endregion Object Methods
    }
}