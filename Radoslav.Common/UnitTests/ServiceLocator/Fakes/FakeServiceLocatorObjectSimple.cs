using Radoslav.Timers;

namespace Radoslav.UnitTests
{
    public sealed class FakeServiceLocatorObjectSimple : IFakeServiceLocatorObject
    {
        public ITimer Timer { get; set; }
    }
}