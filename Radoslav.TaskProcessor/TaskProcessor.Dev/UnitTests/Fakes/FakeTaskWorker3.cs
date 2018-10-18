using System.Diagnostics.CodeAnalysis;

namespace Radoslav.TaskProcessor.UnitTests.Fakes
{
    public sealed class FakeTaskWorker3
    {
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "dummy", Justification = "Class is used for unit testing the App.config configuration.")]
        public FakeTaskWorker3(int dummy)
        {
        }
    }
}