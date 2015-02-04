using System;

namespace MiniDDD.Events.EventUtils
{
    public interface IEvent
    {
        Guid EventId { get; set; }
    }
}
