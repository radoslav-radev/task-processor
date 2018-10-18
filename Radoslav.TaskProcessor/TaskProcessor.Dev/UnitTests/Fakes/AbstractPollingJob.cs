using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.UnitTests
{
    internal abstract class AbstractPollingJob : IPollingJob
    {
        #region IPollingJob Members

        public abstract void Initialize();

        public abstract void Process();

        #endregion IPollingJob Members
    }
}