using IMAR_DialogoOperatore.Application.Interfaces.Repositories;
using IMAR_DialogoOperatore.Domain.Entities.Imar_Produzione;

namespace IMAR_DialogoOperatore.Application.Interfaces.UoW
{
    public interface IImarProduzioneUoW : IUnitOfWork
    {
        public IGenericRepository<Forzatura> ForzaturaRepository { get; }
        public IGenericRepository<OrdineProduzioneForzato> OrdineProduzioneForzatoRepository { get; }
        public IGenericRepository<SegnalazioneDifformita> SegnalazioniDifformitaRepository { get; }
    }
}
