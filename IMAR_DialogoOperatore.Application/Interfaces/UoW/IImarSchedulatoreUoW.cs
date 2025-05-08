using IMAR_DialogoOperatore.Application.Interfaces.Repositories;
using IMAR_DialogoOperatore.Domain.Models;

namespace IMAR_DialogoOperatore.Application.Interfaces.UoW
{
    public interface IImarSchedulatoreUoW : IUnitOfWork
    {
        public IGenericRepository<DATIMONITOR> DatiMonitorRepository { get; }
    }
}
