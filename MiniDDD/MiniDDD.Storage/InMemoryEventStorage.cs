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
        private List<AggregateRoot> _mementos;



        public InMemoryEventStorage()
        {
            _events = new List<IAggregateRootEvent>();
            _mementos = new List<AggregateRoot>();

        }

        public IEnumerable<IAggregateRootEvent> GetEvents(Guid aggregateId)
        {
            var events = _events.Where(p => p.AggregateRootId == aggregateId).Select(p => p);
            if (events.Count() == 0)
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
                if (version > 2)
                {
                    if (version%3 == 0)
                    {
                        var originator = (IOriginator)aggregate;
                        var memento = originator.GetMemento();
                        memento.Version = version;
                        SaveMemento(memento);
                    }
                }
                @event.AggregateRootVersion = version;
                _events.Add(@event);
            }
            foreach (var @event in uncommittedChanges)
            {
                var desEvent = Converter.ChangeTo(@event, @event.GetType());
                //TODO: publish event?
            }
        }

        public T GetMemento<T>(Guid aggregateId) where T : AggregateRoot
        {
            var memento = _mementos.Where(m => m.Id == aggregateId).Select(m => m).LastOrDefault();
            if (memento != null)
                return (T) memento;
            return null;
        }

        public void SaveMemento(AggregateRoot memento)
        {
            _mementos.Add(memento);
        }
    }
}
