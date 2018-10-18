using System;
using System.Threading;
using Radoslav.TaskProcessor.MessageBus;

namespace Radoslav.TaskProcessor.UnitTests
{
    internal partial class FakeMessageQueue : ITaskProcessorMessageQueue
    {
        #region IMessageQueue Members

        public event EventHandler MessageReceived;

        public bool ReceiveMessages { get; set; }

        public void Push(IUniqueMessage message)
        {
            this.RecordMethodCall(message);

            this.messages.Add(message);

            if (this.ReceiveMessages && (this.MessageReceived != null))
            {
                ThreadPool.QueueUserWorkItem(state => this.MessageReceived(this, EventArgs.Empty));
            }
        }

        public IUniqueMessage PopFirst()
        {
            this.RecordMethodCall();

            if (this.messages.Count == 0)
            {
                return null;
            }

            IUniqueMessage result = this.messages[0];

            this.messages.Remove(result);

            return result;
        }

        #endregion IMessageQueue Members
    }
}