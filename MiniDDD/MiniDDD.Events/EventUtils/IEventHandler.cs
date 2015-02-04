namespace MiniDDD.Events.EventUtils
{
    public interface IEventHandler<TEvent> where TEvent : Event
    {
        void Handle(TEvent e);
    }
}