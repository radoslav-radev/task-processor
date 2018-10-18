using System.Diagnostics;

namespace Radoslav.Diagnostics
{
    /// <summary>
    /// Basic functionality of a class that is responsible for automatically killing child processes when the current process exits or crashes.
    /// </summary>
    public interface IChildProcessKiller
    {
        /// <summary>
        /// Tries to adds a process that will be killed when the current process exits or crashes.
        /// </summary>
        /// <param name="process">The process to be added.</param>
        /// <exception cref="System.ArgumentNullException">Parameter <paramref name="process"/> is null.</exception>
        void AddProcess(Process process);
    }
}