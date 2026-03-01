using IMAR_DialogoOperatore.Domain.Models;

namespace IMAR_DialogoOperatore.Application.Interfaces.Services.Activities
{
    public interface IAttivitaService
    {
        bool ConfrontaCausaliAttivita(IList<Attivita> listaAttivitaDaControllare, string bollaAttivitaDaConfrontare, string operazioneAttivitaDaConfrontare);
        Attivita? CercaAttivitaDaBolla(string bolla);
        Task<string?> AvanzaAttivitaAsync(Operatore operatore, Attivita attivitaDaAvanzare, int quantitaProdotta, int quantitaScartata);
        public IEnumerable<Attivita> GetAttivitaPerOdp(string odp);
        public Task<IList<Attivita>> OttieniAttivitaOperatoreAsync(Operatore operatore);
        Task<IList<mesDiaOpe>?> GetAttivitaAperteAsync();
        public IList<Attivita> GetAttivitaIndirette();
        Task<List<string>> GetIdOperatoriConBollaApertaAsync(string bolla);
        IList<Attivita>? GetAttivitaOperatoreDellUltimaGiornata(int idJmesOperatore);
        Task<string?> ApriAttrezzaggioFaseNonPianificataAsync(Attivita attivita, Operatore operatore);
        Task<string?> ApriLavoroFaseNonPianificataAsync(Attivita attivita, Operatore operatore);
    }
}
