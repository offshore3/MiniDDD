using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Windsor;

namespace MiniDDD.Storage
{
    public interface IUnitOfWork : IDisposable
    {
        void Commit();
    }

    public class SqlServerUnitOfWork : IUnitOfWork
    {
        private readonly IEventStorage _eventStorage;

        public SqlServerUnitOfWork(IEventStorageProvider eventStorageProvider)
        {
            _eventStorage = eventStorageProvider.GetEventStorage();
        }

        public void Commit()
        {
            _eventStorage.Commit();
        }

        public void Dispose()
        {
            if (_eventStorage != null)
            {
                _eventStorage.Dispose();

            }
            GC.SuppressFinalize(this);
        }
    }

    public interface IUnitOfWorkFactory
    {
        IUnitOfWork GetCurrentUnitOfWork();
    }

    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly IWindsorContainer _container;

        public UnitOfWorkFactory(IWindsorContainer container)
        {
            _container = container;
        }

        public IUnitOfWork GetCurrentUnitOfWork()
        {
            return _container.Resolve<IUnitOfWork>();
        }
    }


    public interface IEventStorageProvider
    {
        IEventStorage GetEventStorage();
    }

    public class EventStorageProvider : IEventStorageProvider
    {
        private readonly IWindsorContainer _container;

        public EventStorageProvider(IWindsorContainer container)
        {
            _container = container;
        }

        public IEventStorage GetEventStorage()
        {
            return _container.Resolve<IEventStorage>();
        }
    }
}
