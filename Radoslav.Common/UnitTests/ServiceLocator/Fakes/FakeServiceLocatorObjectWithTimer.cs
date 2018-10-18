using Radoslav.Timers;

namespace Radoslav.UnitTests
{
    public sealed class FakeServiceLocatorObjectWithTimer
    {
        public FakeServiceLocatorObjectWithTimer(ITimer timer)
        {
            this.Timer = timer;
        }

        public ITimer Timer { get; private set; }
    }
}