using System;
using System.Transactions;

namespace MiniDDD.Storage.UnitOfWork
{
    public class SqlServerUnitOfWork : IUnitOfWork
    {
        private readonly IEventStorage _eventStorage;
        private TransactionScope _transactionScope;

        public SqlServerUnitOfWork(IEventStorageProvider eventStorageProvider)
        {
            _transactionScope=  new TransactionScope();
            _eventStorage = eventStorageProvider.GetEventStorage();
        }

        public void Commit()
        {
            _eventStorage.Committing();
            _transactionScope.Complete();
            _eventStorage.MarkCommitted();
        }

        public void Dispose()
        {
            if (_eventStorage != null)
            {
                _eventStorage.Dispose();

            }
            if (_transactionScope != null)
            {
                _transactionScope.Dispose();
            }
            GC.SuppressFinalize(this);
        }
    }
}