using System;
using System.Collections.Generic;
using System.Threading;
using Radoslav.Collections;

namespace Radoslav.Redis.ServiceStack
{
    internal class SubscribeOperationState : IDisposable
    {
        private readonly SubscribeOperation operation;
        private readonly ConcurrentHashSet<string> channels;
        private readonly ManualResetEventSlim blocker = new ManualResetEventSlim();

        internal SubscribeOperationState(SubscribeOperation operation, IEnumerable<string> channels)
        {
            this.channels = new ConcurrentHashSet<string>(channels);

            this.operation = operation;
        }

        ~SubscribeOperationState()
        {
            this.Dispose();
        }

        internal SubscribeOperation Operation
        {
            get { return this.operation; }
        }

        internal ManualResetEventSlim Blocker
        {
            get { return this.blocker; }
        }

        internal ICollection<string> Channels
        {
            get { return this.channels; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.blocker.Dispose();

            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Members
    }
}