using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.UnitTests
{
    public sealed class PollingJobWithoutParameterlessConstructor : IPollingJob
    {
        public PollingJobWithoutParameterlessConstructor(int dummy)
        {
            if (dummy == 0)
            {
            }
        }

        #region IPollingJob Members

        public void Initialize()
        {
        }

        public void Process()
        {
        }

        #endregion IPollingJob Members
    }
}