using System;

namespace MiniDDD.Events
{
   public class Event:IEvent
    {
       public Guid EventId { get; set; }
    }
}