using System.Collections.Generic;
using MiniDDD.Events;

namespace MiniDDD.Domain
{
    public interface IEventProvider
    {
        void LoadsFromHistory(IEnumerable<Event> history);
        IEnumerable<Event> GetUncommittedChanges();
    }
}