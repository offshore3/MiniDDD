using Castle.Windsor;

namespace MiniDDD.Storage.UnitOfWork
{
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