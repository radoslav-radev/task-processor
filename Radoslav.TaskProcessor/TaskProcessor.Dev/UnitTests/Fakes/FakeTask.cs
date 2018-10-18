using System;

namespace Radoslav.TaskProcessor.UnitTests
{
    [Serializable]
    public sealed class FakeTask : IFakeTask
    {
        public string StringValue { get; set; }

        public int NumberValue { get; set; }
    }
}