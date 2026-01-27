using System.Threading.Tasks;
using IMAR_DialogoOperatore.Application.DTOs;
using IMAR_DialogoOperatore.Domain.DTO;
using IMAR_DialogoOperatore.Domain.Entities.Imar_Produzione;
using IMAR_DialogoOperatore.Domain.Models;

namespace IMAR_DialogoOperatore.Application.Interfaces.Clients
{
    public interface IImarApiClient
    {
		Task<CostiArticoloDTO> GetCostiArticolo(string codiceArticolo);
		Task<string> SendTaskAsana(TaskAsana taskAsana, string creatoreTask);
		Task RegistraForzature(Forzatura forzatura);
		Task RimuoviSchedulazioneAttuale(string chiamante, List<ODPSchedulazione> schedulazioneAttuale);
		Task<ForzaturaDTO> GetPreviewForzatura(string odc, string giornoForza, decimal allocazione);
		Task<string> InserisciNuovaSchedulazione(List<GiornoSchedulazione> forzatura, string riga, DateTime fineSchedulazione);
		Task<List<ODPSchedulazione>> GetSchedulazioneAttuale(string odc);
	}
}
