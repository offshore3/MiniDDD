using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniDDD.Events
{
    public interface IEvent
    {
        Guid EventId { get; set; }
    }

    public interface IAggregateRootEvent : IEvent
    {
        int AggregateRootVersion { get; set; }
        Guid AggregateRootId { get; set; }
        DateTime TimeStamp { get; set; }
    }
}
