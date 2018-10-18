using System.Diagnostics.CodeAnalysis;

namespace Radoslav.Common.UnitTests
{
    internal sealed class FakeEntityWithoutDefaultConstructor : IFakeEntity
    {
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "dummy", Justification = "It is needed in unit tests to have a class without parameterless constructor.")]
        public FakeEntityWithoutDefaultConstructor(object dummy)
        {
        }

        #region IFakeEntity Members

        public string StringValue { get; set; }

        public int IntValue { get; set; }

        #endregion IFakeEntity Members
    }
}