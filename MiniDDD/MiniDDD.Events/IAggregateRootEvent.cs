using System;
using MiniDDD.Events.EventUtils;

namespace MiniDDD.Events
{
    public interface IAggregateRootEvent : IEvent
    {
        int AggregateRootVersion { get; set; }
        Guid AggregateRootId { get; set; }
        DateTime TimeStamp { get; set; }
    }
}