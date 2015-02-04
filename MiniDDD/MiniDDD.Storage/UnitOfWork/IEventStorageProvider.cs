namespace MiniDDD.Storage.UnitOfWork
{
    public interface IEventStorageProvider
    {
        IEventStorage GetEventStorage();
    }
}