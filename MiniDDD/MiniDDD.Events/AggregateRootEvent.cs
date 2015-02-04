using System;
using MiniDDD.Events.EventUtils;

namespace MiniDDD.Events
{
    public class AggregateRootEvent : Event,IAggregateRootEvent
    {
        public AggregateRootEvent()
        {
            EventId = Guid.NewGuid();
        }

        public int AggregateRootVersion { get; set; }
        public Guid AggregateRootId { get; set; }

        public DateTime TimeStamp
        {
            get { return DateTime.Now; }
            set { }
        }
    }
}