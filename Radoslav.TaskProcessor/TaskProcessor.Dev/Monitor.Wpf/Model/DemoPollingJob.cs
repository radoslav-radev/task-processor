using System.Diagnostics;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor
{
    /// <summary>
    /// A demo implementation of <see cref="IPollingJob"/>.
    /// </summary>
    public sealed class DemoPollingJob : IPollingJob
    {
        #region IPollingJob Members

        /// <inheritdoc />
        public void Initialize()
        {
        }

        /// <inheritdoc />
        public void Process()
        {
            Trace.WriteLine("ENTER: Processing demo polling job ...");
            Trace.WriteLine("EXIT: Demo polling job processed.");
        }

        #endregion IPollingJob Members
    }
}