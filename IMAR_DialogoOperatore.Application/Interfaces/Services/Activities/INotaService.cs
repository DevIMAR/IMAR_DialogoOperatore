using IMAR_DialogoOperatore.Domain.Models;

namespace IMAR_DialogoOperatore.Application.Interfaces.Services.Activities
{
    public interface INotaService
    {
        IEnumerable<Nota> GetNoteAttivita(Attivita? attivita);
        IEnumerable<Nota> GetNoteDaBolla(string bolla);
        IEnumerable<Nota> GetNoteDaOdpFase(string odp, string fase);
        void AggiungiNota(Attivita? attivita, string? testo);
    }
}
