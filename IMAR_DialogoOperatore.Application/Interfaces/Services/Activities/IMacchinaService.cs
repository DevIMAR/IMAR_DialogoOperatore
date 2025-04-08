using IMAR_DialogoOperatore.Domain.Models;

namespace IMAR_DialogoOperatore.Application.Interfaces.Services.Activities
{
    public interface IMacchinaService
    {
        Macchina GetMacchinaByAttivita(Attivita attivita);
        int GetCodiceJmesByCodice(string codiceMacchinaCompleto);
        Macchina? GetPrimaMacchinaFittiziaNonUtilizzata();
    }
}