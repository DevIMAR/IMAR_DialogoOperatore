using IMAR_DialogoOperatore.Application.DTOs;
using IMAR_DialogoOperatore.Domain.Entities.Imar_Produzione;

namespace IMAR_DialogoOperatore.Application.Interfaces.Services.External
{
    public interface ISegnalazioniDifformitaService
    {
        int InsertSegnalazione(SegnalazioneDifformita segnalazione);
        Task<CostiArticoloDTO> GetCostiArticolo(string codiceArticolo);
        string? GetFlussoByOdpFase(string odp, string fase);
        List<string> GetCategorie();
	}
}
