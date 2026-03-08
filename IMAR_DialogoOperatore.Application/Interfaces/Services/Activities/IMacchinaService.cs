using IMAR_DialogoOperatore.Domain.Models;

namespace IMAR_DialogoOperatore.Application.Interfaces.Services.Activities
{
    public interface IMacchinaService
    {
        Macchina GetMacchinaRealeByAttivita(Attivita attivita);
        int GetCodiceJmesByCodice(string codiceMacchinaCompleto);
        Task<Macchina?> GetMacchinaFittiziaByFirstAttivitaApertaAsync(Attivita attivitaAperta, int idJMesOperatore);
        Task<Macchina?> GetPrimaMacchinaFittiziaNonUtilizzataAsync();
        Task<Macchina?> GetMacchinaFittiziaDaAttivitaAttrezzataAsync(Attivita attivitaDaAggiungere);
    }
}
