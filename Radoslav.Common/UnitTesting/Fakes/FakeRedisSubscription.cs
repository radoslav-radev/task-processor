using System;
using System.Collections.Generic;
using Radoslav.Redis;

namespace Radoslav
{
    internal sealed class FakeRedisSubscription : MockObject, IRedisMessageSubscription
    {
        #region IRedisMessageSubscription Members

        public event EventHandler<RedisMessageEventArgs> MessageReceived
        {
            add { }
            remove { }
        }

        public IEnumerable<string> ActiveChannels
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void SubscribeToChannels(TimeSpan timeout, IEnumerable<string> channels)
        {
            throw new NotImplementedException();
        }

        public bool UnsubscribeFromChannels(TimeSpan timeout, IEnumerable<string> channels)
        {
            throw new NotImplementedException();
        }

        #endregion IRedisMessageSubscription Members

        #region IDisposable Members

        public void Dispose()
        {
            this.RecordMethodCall();
        }

        #endregion IDisposable Members
    }
}