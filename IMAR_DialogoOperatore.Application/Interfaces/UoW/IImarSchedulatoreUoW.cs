using IMAR_DialogoOperatore.Application.Interfaces.Repositories;
using IMAR_DialogoOperatore.Domain.Entities.Imar_Schdulazione;

namespace IMAR_DialogoOperatore.Application.Interfaces.UoW
{
    public interface IImarSchedulatoreUoW : IUnitOfWork
    {
        public IGenericRepository<CAL_FL_ODP> CalFlOdpRepository { get; }
    }
}
