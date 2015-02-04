using System.Collections.Generic;

namespace MiniDDD.Events.EventUtils
{
    public interface IEventHandlerFactory
    {
        IEnumerable<IEventHandler<T>> GetHandlers<T>() where T : Event;
    }
}