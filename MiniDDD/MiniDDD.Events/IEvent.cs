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

    public class AggregateRootEvent : IAggregateRootEvent
    {
        public AggregateRootEvent()
        {
            EventId = Guid.NewGuid();
        }

        public Guid EventId { get;  set; }
        public int AggregateRootVersion { get; set; }
        public Guid AggregateRootId { get; set; }

        public DateTime TimeStamp
        {
            get { return DateTime.Now; }
            set { }
        }
    }
}
