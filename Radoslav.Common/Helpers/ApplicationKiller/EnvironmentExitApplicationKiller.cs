using System;
using System.Diagnostics;

namespace Radoslav
{
    /// <summary>
    /// An implementation of <see cref="IApplicationKiller"/> that uses <see cref="Environment"/>.<see cref="Environment.Exit(Int32)"/>.
    /// </summary>
    public sealed class EnvironmentExitApplicationKiller : IApplicationKiller
    {
        #region IApplicationKiller Members

        /// <inheritdoc />
        public void Kill()
        {
            Trace.WriteLine("Killing application ...");

            Environment.Exit(0);
        }

        #endregion IApplicationKiller Members
    }
}