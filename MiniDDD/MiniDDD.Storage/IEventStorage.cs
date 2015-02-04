using System;
using System.Collections.Generic;
using MiniDDD.Domain;
using MiniDDD.Events;

namespace MiniDDD.Storage
{
    public interface IEventStorage:IDisposable
    {
        IEnumerable<IAggregateRootEvent> GetEvents(Guid aggregateId);
        void Save(AggregateRoot aggregate);
        void Committing();
        void MarkCommitted();

    }
}