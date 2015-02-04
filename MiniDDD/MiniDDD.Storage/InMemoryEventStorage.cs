using System;
using System.Collections.Generic;
using System.Linq;
using MiniDDD.Domain;
using MiniDDD.Events;
using MiniDDD.Storage.Exceptions;

namespace MiniDDD.Storage
{
    public class InMemoryEventStorage : IEventStorage
    {
        private List<IAggregateRootEvent> _events;
        public InMemoryEventStorage()
        {
            _events = new List<IAggregateRootEvent>();
        }

        public IEnumerable<IAggregateRootEvent> GetEvents(Guid aggregateId)
        {
            var events = _events.Where(p => p.AggregateRootId == aggregateId).Select(p => p);
            if (!events.Any())
            {
                throw new AggregateNotFoundException(string.Format("Aggregate with Id: {0} was not found", aggregateId));
            }
            return events;
        }

        public void Save(AggregateRoot aggregate)
        {
            var uncommittedChanges = aggregate.GetUncommittedChanges();
            var version = aggregate.Version;

            foreach (var @event in uncommittedChanges)
            {
                version++;
                
                @event.AggregateRootVersion = version;
                _events.Add(@event);
            }

            foreach (var @event in uncommittedChanges)
            {
                var desEvent = Converter.ChangeTo(@event, @event.GetType());
                //TODO: publish event?
            }
        }

        public void Committing()
        {
            
        }

        public void MarkCommitted()
        {
            
        }


        public void Dispose()
        {
            GC.SuppressFinalize(this);

        }
    }
}
