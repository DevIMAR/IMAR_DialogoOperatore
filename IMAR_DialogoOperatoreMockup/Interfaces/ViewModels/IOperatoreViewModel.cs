using IMAR_DialogoOperatore.Domain.Models;

namespace IMAR_DialogoOperatore.Interfaces.ViewModels
{
    public interface IOperatoreViewModel
    {
        int? Badge { get; }
        string Nome { get; }
        string Cognome { get; }
        Macchina? MacchinaAssegnata { get; set; }
        string Stato { get; set; }
        IList<Attivita> AttivitaAperte { get; set; }
    }
}