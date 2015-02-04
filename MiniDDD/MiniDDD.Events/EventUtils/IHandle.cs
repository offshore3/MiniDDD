namespace MiniDDD.Events.EventUtils
{
    public interface IHandle<TEvent> where TEvent : IAggregateRootEvent
    {
        void Handle(TEvent e);
    }
}