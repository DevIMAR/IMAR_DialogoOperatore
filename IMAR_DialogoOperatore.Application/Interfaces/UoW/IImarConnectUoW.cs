using IMAR_DialogoOperatore.Application.Interfaces.Repositories;
using IMAR_DialogoOperatore.Domain.Entities.Imar_Connect;

namespace IMAR_DialogoOperatore.Application.Interfaces.UoW
{
    public interface IImarConnectUoW : IUnitOfWork
    {
        public IGenericRepository<Interfaccia> InterfacciaRepository { get; }
    }
}
