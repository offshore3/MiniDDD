using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MiniDDD.Events;

namespace MiniDDD.Domain
{
    public abstract class AggregateRoot : IEventProvider
    {
        private readonly List<IAggregateRootEvent> _changes;

        public Guid Id { get; protected set; }
        public int Version { get; set; }
        public int EventVersion { get; protected set; }

        protected AggregateRoot()
        {
            _changes = new List<IAggregateRootEvent>();
        }

        public IEnumerable<IAggregateRootEvent> GetUncommittedChanges()
        {
            return _changes;
        }

        public void MarkChangesAsCommitted()
        {
            _changes.Clear();
        }

        public void LoadsFromHistory(IEnumerable<IAggregateRootEvent> history)
        {
            foreach (var e in history) ApplyChange(e, false);
            Version = history.Last().AggregateRootVersion;
            EventVersion = Version;
        }

        protected void ApplyChange(IAggregateRootEvent @event)
        {
            @event.AggregateRootId = this.Id;
            ApplyChange(@event, true);
        }

        private void ApplyChange(IAggregateRootEvent @event, bool isNew)
        {
            dynamic d = this;

            d.Handle(Converter.ChangeTo(@event, @event.GetType()));
            if (isNew)
            {
                _changes.Add(@event);
            }
        }
    }
}
