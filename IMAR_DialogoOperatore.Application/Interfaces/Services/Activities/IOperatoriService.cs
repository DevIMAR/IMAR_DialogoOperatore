using IMAR_DialogoOperatore.Domain.Models;

namespace IMAR_DialogoOperatore.Application.Interfaces.Services.Activities
{
    public interface IOperatoriService
    {
        Operatore Operatore { get; set; }

        Operatore? OttieniOperatore(int? badge);
        string? RimuoviAttivitaDaOperatore(Operatore operatore, Attivita attivitaDaRimuovere, int? quantitaProdotta, int? quantitaScartata, bool isSospeso = false, bool? isAttrezzaggio = null);
        string? AggiungiAttivitaAdOperatore(bool isAttrezzaggio, Operatore operatore, Attivita attivita, bool isAttivitaIndiretta);
    }
}