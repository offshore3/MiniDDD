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
        private  IUnitOfWork _unitOfWork;

        private IEventStorage _eventStorage;

        private static object _lockStorage = new object();

        public Repository(IEventStorage eventStorage)
        {
            _eventStorage = eventStorage;
        }

        public void Join(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Quit()
        {
            _unitOfWork = null;
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
                }
                if (_unitOfWork != null)
                {
                    _unitOfWork.AddAggregateRoot(aggregate);
                }
                else
                {
                    _eventStorage.Save(aggregate);
                }
            }
        }

        public T GetById(Guid id)
        {
            IEnumerable<IAggregateRootEvent> events;

            AggregateRoot memento;

            if (_unitOfWork != null)
            {
                // BaseMemento is cache
                 memento = _unitOfWork.EventStorage.GetMemento<AggregateRoot>(id);
                if (memento != null)
                {
                    events = _unitOfWork.EventStorage.GetEvents(id).Where(e => e.AggregateRootVersion >= memento.Version);
                }
                else
                {
                    events = _unitOfWork.EventStorage.GetEvents(id);
                }
            }
            else
            {
                // BaseMemento is cache
                memento = _eventStorage.GetMemento<AggregateRoot>(id);
                if (memento != null)
                {
                    events = _eventStorage.GetEvents(id).Where(e => e.AggregateRootVersion >= memento.Version);
                }
                else
                {
                    events = _eventStorage.GetEvents(id);
                }
                
            }

            var obj = new T();
            if (memento != null)
                ((IOriginator)obj).SetMemento(memento);

            obj.LoadsFromHistory(events);
            return obj;
        }
      
    }
}
