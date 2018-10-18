namespace Radoslav.TaskProcessor.UnitTests
{
    internal sealed class FakeApplicationKiller : MockObject, IApplicationKiller
    {
        #region IApplicationKiller Members

        public void Kill()
        {
            this.RecordMethodCall();
        }

        #endregion IApplicationKiller Members
    }
}