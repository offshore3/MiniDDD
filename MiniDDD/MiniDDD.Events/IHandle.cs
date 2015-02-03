namespace MiniDDD.Events
{
    public interface IHandle<TEvent> where TEvent : IAggregateRootEvent
    {
        void Handle(TEvent e);
    }
}