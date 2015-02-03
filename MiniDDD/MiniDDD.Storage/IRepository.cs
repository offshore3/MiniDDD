using System;
using MiniDDD.Domain;

namespace MiniDDD.Storage
{
    public interface IRepository<T> :IUnitOfWorkParticipants where T : AggregateRoot, new()
    {
        void Save(AggregateRoot aggregate, int expectedVersion);
        T GetById(Guid id);


    }

    public interface IUnitOfWorkParticipants
    {
        void Join(IUnitOfWork unitOfWork);

        void Quit();

        void Commit();

        void MarkAsCommited();

    }
}