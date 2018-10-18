using System;

namespace Radoslav.Common.UnitTests
{
    [Serializable]
    public sealed class FakeEntity : IFakeEntity, IEquatable<FakeEntity>
    {
        #region IFakeEntity Members

        public string StringValue { get; set; }

        public int IntValue { get; set; }

        #endregion IFakeEntity Members

        #region IEquatable<FakeEntity> Members

        public bool Equals(FakeEntity other)
        {
            return (other != null) && (this.StringValue == other.StringValue) && (this.IntValue == other.IntValue);
        }

        #endregion IEquatable<FakeEntity> Members

        public override bool Equals(object obj)
        {
            return this.Equals(obj as FakeEntity);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}