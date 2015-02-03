using System;
using System.Collections.Generic;
using System.Transactions;
using MiniDDD.Domain;

namespace MiniDDD.Storage
{
    public interface IUnitOfWork
    {
        void AddAggregateRoot(AggregateRoot aggregateRoot);
        void Commit();
        void RollBack();

        IEventStorage EventStorage { get;  }
        
    }

    public class SqlServerUnitOfWork:IUnitOfWork, IDisposable
    {
        private IEventStorage _storage;
        private List<AggregateRoot> AggregateRoots { get; set; }

        private static object _lockStorage = new object();

        List<IUnitOfWorkParticipants> participantses=new List<IUnitOfWorkParticipants>();

        public SqlServerUnitOfWork(IEventStorage storage)
        {
            _storage = storage;

            AggregateRoots=new List<AggregateRoot>();
          
        }

        public void AddParticipants(IUnitOfWorkParticipants repository)
        {
            lock (_lockStorage)
            {
                participantses.Add(repository);
            }

            repository.Join(this);
        }


        public void AddAggregateRoot(AggregateRoot aggregateRoot)
        {
            AggregateRoots.Add(aggregateRoot);
        }

        public void Commit()
        {
            using (var transaction = new TransactionScope())
            {

                foreach (var aggregate in AggregateRoots)
                {
                    _storage.Save(aggregate);
                }

                transaction.Complete();
            }

            foreach (var aggregateRoot in AggregateRoots)
            {
                aggregateRoot.MarkChangesAsCommitted();
            }
        }

        public void RollBack()
        {
            // TODO: Do nothing now
        }

        public IEventStorage EventStorage
        {
            get { return _storage; }
        }

        public void Dispose()
        {
            if (_storage != null)
            {
                _storage = null;
               
            }

            foreach (var unitOfWorkParticipantse in participantses)
            {
               unitOfWorkParticipantse.Quit();
            }

            participantses = null;

            GC.SuppressFinalize(this);
        }
    }
}
