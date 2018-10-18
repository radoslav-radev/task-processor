using System.Diagnostics;
using Radoslav.Diagnostics;

namespace Radoslav
{
    public sealed class FakeChildProcessKiller : MockObject, IChildProcessKiller
    {
        #region IChildProcessKiller Members

        public void AddProcess(Process process)
        {
            this.RecordMethodCall(process);
        }

        #endregion IChildProcessKiller Members
    }
}