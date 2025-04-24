using IMAR_DialogoOperatore.Domain.Entities.Imar_Produzione;

namespace IMAR_DialogoOperatore.Application.Interfaces.Services.External
{
    public interface ISegnalazioniDifformitaService
    {
        int InsertSegnalazione(SegnalazioneDifformita segnalazione);
    }
}
