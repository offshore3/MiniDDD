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
        T GetMemento<T>(Guid aggregateId) where T : AggregateRoot;
        void SaveMemento(AggregateRoot memento);

        void Commit();

    }
}