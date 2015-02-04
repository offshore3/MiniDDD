using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniDDD.Domain;
using MiniDDD.Events;
using MiniDDD.Storage.Exceptions;

namespace MiniDDD.Storage
{
    public class Repository<T> : IRepository<T> where T : AggregateRoot, new()
    {
        private readonly IEventStorageProvider _eventStorageProvider;

        private static object _lockStorage = new object();

        private IEventStorage _eventStorage { get { return _eventStorageProvider.GetEventStorage(); } }

        public Repository(IEventStorageProvider eventStorageProvider)
        {
            _eventStorageProvider = eventStorageProvider;
        }

        public void Save(AggregateRoot aggregate, int expectedVersion)
        {
            if (aggregate.GetUncommittedChanges().Any())
            {
                lock (_lockStorage)
                {
                    var item = new T();

                    if (expectedVersion != -1)
                    {
                        item = GetById(aggregate.Id);
                        if (item.Version != expectedVersion)
                        {
                            throw new ConcurrencyException(string.Format("Aggregate {0} has been previously modified",
                                                                         item.Id));
                        }
                    }

                    _eventStorage.Save(aggregate);
                }
            }
        }

        public T GetById(Guid id)
        {
            IEnumerable<IAggregateRootEvent> events;

            // BaseMemento is cache
            //var memento = _eventStorage.GetMemento<AggregateRoot>(id);
            //if (memento != null)
            //{
            //    events = _eventStorage.GetEvents(id).Where(e => e.AggregateRootVersion >= memento.Version);
            //}
            //else
            //{
            // events = _eventStorage.GetEvents(id);
            //}
            //var obj = new T();
            //if (memento != null)
            //{
            //    ((IOriginator) obj).SetMemento(memento);


            events = _eventStorage.GetEvents(id);
            var obj = new T();
            obj.LoadsFromHistory(events);
            return obj;
        }


    }
}
