namespace Radoslav.Common.UnitTests
{
    internal sealed class FakeMockObject : MockObject
    {
        internal int Method1(int a, int b)
        {
            return this.GetPredefinedResult<int>(a, b);
        }
    }
}