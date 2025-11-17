using IMAR_DialogoOperatore.Application.Interfaces.Repositories;
using IMAR_DialogoOperatore.Domain.Entities.Imar_Schedulazione;

namespace IMAR_DialogoOperatore.Application.Interfaces.UoW
{
    public interface IImarSchedulatoreUoW : IUnitOfWork
    {
        public IGenericRepository<CAL_FL_ODP> CalFlOdpRepository { get; }
        public IGenericRepository<DESCRIZIONE_FASI> DescrizioneFasiRepository { get; }
        public IGenericRepository<FASI> FasiRepository { get; }
        public IGenericRepository<ODC_ODP> OdcOdpRepository { get; }
        public IGenericRepository<ORDINE_CLIENTE> OrdineClienteRepository { get; }
        public IGenericRepository<ORDINE_PRODUZIONE> OrdineProduzioneRepository { get; }
    }
}
