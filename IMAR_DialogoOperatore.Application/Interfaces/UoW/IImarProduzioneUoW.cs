using IMAR_DialogoOperatore.Application.Interfaces.Repositories;
using IMAR_DialogoOperatore.Domain.Entities.Imar_Produzione;

namespace IMAR_DialogoOperatore.Application.Interfaces.UoW
{
    public interface IImarProduzioneUoW : IUnitOfWork
    {
        public IGenericRepository<SegnalazioneDifformita> SegnalazioniDifformitaRepository { get; }
    }
}
