namespace MiniDDD.Events.EventUtils
{
    public interface IEventBus
    {
        void Publish<T>(T @event) where T : Event;
    }
}