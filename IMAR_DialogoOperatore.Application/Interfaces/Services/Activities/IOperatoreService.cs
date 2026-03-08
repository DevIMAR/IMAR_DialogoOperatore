using IMAR_DialogoOperatore.Domain.Models;

namespace IMAR_DialogoOperatore.Application.Interfaces.Services.Activities
{
    public interface IOperatoreService
    {
        Operatore Operatore { get; set; }

        Task<Operatore?> OttieniOperatoreAsync(int? badge);
        Task<string?> RimuoviAttivitaDaOperatoreAsync(Operatore operatore, Attivita attivitaDaRimuovere, int? quantitaProdotta, int? quantitaScartata, bool isSospeso = false, bool? isAttrezzaggio = null);
        Task<string?> AggiungiAttivitaAdOperatoreAsync(bool isAttrezzaggio, Operatore operatore, Attivita attivita, bool isAttivitaIndiretta);
        Operatore GetOperatoreDaIdJMes(string idJMesOperatore);
    }
}
