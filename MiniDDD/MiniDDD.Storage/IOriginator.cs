using MiniDDD.Domain;

namespace MiniDDD.Storage
{
    public interface IOriginator
    {
        AggregateRoot GetMemento();
        void SetMemento(AggregateRoot memento);
    }
}