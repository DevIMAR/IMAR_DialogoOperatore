using IMAR_DialogoOperatore.Domain.Models;

namespace IMAR_DialogoOperatore.Application.Interfaces.Services.Activities
{
    public interface IAttivitaService
    {
        Attivita Attivita { get; }
        IList<Attivita> AttivitaTrovate { get; }

        bool ConfrontaCausaliAttivita(IList<Attivita> listaAttivitaDaControllare, string bollaAttivitaDaConfrontare, string operazioneAttivitaDaConfrontare);
        Attivita? CercaAttivitaDaBolla(string bolla);
        string? AvanzaAttivita(Operatore operatore, Attivita attivitaDaAvanzare, int quantitaProdotta, int quantitaScartata);
        public IList<Attivita> GetAttivitaPerOdp(string odp);
        public IList<Attivita> OttieniAttivitaOperatore(string badgeOperatore);
        public IList<Attivita> GetAttivitaIndirette();
    }
}