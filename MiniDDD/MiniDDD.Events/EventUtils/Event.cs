using System;

namespace MiniDDD.Events.EventUtils
{
    [Serializable]
    public class Event : IEvent
    {
        public Guid EventId { get; set; }
    }
}