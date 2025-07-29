using IMAR_DialogoOperatore.Domain.Models;

namespace IMAR_DialogoOperatore.Application.Interfaces.Services.Activities
{
    public interface IAttivitaService
    {
        bool ConfrontaCausaliAttivita(IList<Attivita> listaAttivitaDaControllare, string bollaAttivitaDaConfrontare, string operazioneAttivitaDaConfrontare);
        Attivita? CercaAttivitaDaBolla(string bolla);
        string? AvanzaAttivita(Operatore operatore, Attivita attivitaDaAvanzare, int quantitaProdotta, int quantitaScartata);
        public IEnumerable<Attivita> GetAttivitaPerOdp(string odp);
        public IList<Attivita> OttieniAttivitaOperatore(Operatore operatore);
        IList<mesDiaOpe>? GetAttivitaAperte();
        public IList<Attivita> GetAttivitaIndirette();
        List<string> GetIdOperatoriConBollaAperta(string bolla);
        IList<Attivita>? GetAttivitaOperatoreDellUltimaGiornata(int idJmesOperatore);
    }
}