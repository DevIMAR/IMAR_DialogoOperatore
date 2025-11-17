using IMAR_DialogoOperatore.Application.DTOs;
using IMAR_DialogoOperatore.Domain.Entities.Imar_Produzione;
using IMAR_DialogoOperatore.Domain.Models;

namespace IMAR_DialogoOperatore.Application.Interfaces.Clients
{
    public interface IImarApiClient
    {
        Task<CostiArticoloDTO> GetCostiArticolo(string codiceArticolo);
        Task<string> SendTaskAsana(TaskAsana taskAsana, string creatoreTask);
        void RegistraForzature(Forzatura forzatura);
        List<ODPSchedulazione> GetSchedulazioneAttuale(string odc);
    }
}
