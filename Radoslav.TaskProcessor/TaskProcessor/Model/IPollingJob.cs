using System.Diagnostics.CodeAnalysis;

namespace Radoslav.TaskProcessor.Model
{
    /// <summary>
    /// Basic implementation of task processor polling job.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "This is a task processor polling job.")]
    public interface IPollingJob
    {
        /// <summary>
        /// Initializes the polling job before the first call of the <see cref="Process"/> method.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Processes the polling job.
        /// </summary>
        void Process();
    }
}