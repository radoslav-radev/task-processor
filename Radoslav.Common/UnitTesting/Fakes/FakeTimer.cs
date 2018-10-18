using System;
using System.Collections.Generic;
using System.Threading;

namespace Radoslav
{
    public partial class FakeTimer : MockObject
    {
        private readonly List<EventHandler> tickHandlers = new List<EventHandler>();

        public IEnumerable<EventHandler> TickHandlers
        {
            get { return this.tickHandlers; }
        }

        public void RaiseTick()
        {
            foreach (EventHandler handler in this.tickHandlers)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public void RaiseTickInAnotherThread()
        {
            foreach (EventHandler handler in this.tickHandlers)
            {
                ThreadPool.QueueUserWorkItem(state => handler(this, EventArgs.Empty));
            }
        }
    }
}