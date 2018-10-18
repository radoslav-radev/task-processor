using System.Diagnostics;

namespace Radoslav.Diagnostics
{
    /// <summary>
    /// An implementation of the <see cref="IChildProcessKiller"/> that does nothing.
    /// </summary>
    public sealed class EmptyChildProcessKiller : IChildProcessKiller
    {
        #region IChildProcessKiller Members

        /// <inheritdoc />
        public void AddProcess(Process process)
        {
            /* Do nothing. */
        }

        #endregion IChildProcessKiller Members
    }
}