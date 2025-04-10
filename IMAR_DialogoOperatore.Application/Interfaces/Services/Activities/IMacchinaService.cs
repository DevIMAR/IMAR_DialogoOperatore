using IMAR_DialogoOperatore.Domain.Models;

namespace IMAR_DialogoOperatore.Application.Interfaces.Services.Activities
{
    public interface IMacchinaService
    {
        Macchina GetMacchinaRealeByAttivita(Attivita attivita);
        int GetCodiceJmesByCodice(string codiceMacchinaCompleto);
        Macchina? GetMacchinaFittiziaByFirstAttivitaAperta(Attivita attivitaAperta, int idJMesOperatore);
        Macchina? GetPrimaMacchinaFittiziaNonUtilizzata();
    }
}