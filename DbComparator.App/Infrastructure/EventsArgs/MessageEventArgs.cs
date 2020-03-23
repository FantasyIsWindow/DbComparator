using System;

namespace DbComparator.App.Infrastructure.EventsArgs
{
    public class MessageEventArgs : EventArgs
    {
        public object Message { get; set; }
    }
}
