using System.Collections.Generic;
using System.Linq;
using Radoslav.TaskProcessor.MessageBus;

namespace Radoslav.TaskProcessor.UnitTests
{
    internal sealed partial class FakeMessageQueue : MockObject
    {
        private readonly List<IUniqueMessage> messages = new List<IUniqueMessage>();

        #region IEnumerable<IUniqueMessage> Members

        public IEnumerator<IUniqueMessage> GetEnumerator()
        {
            return this.messages.ToList().GetEnumerator();
        }

        #endregion IEnumerable<IUniqueMessage> Members

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion IEnumerable Members
    }
}