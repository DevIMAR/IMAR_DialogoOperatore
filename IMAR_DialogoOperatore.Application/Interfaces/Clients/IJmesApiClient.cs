
using IMAR_DialogoOperatore.Domain.Models;

namespace IMAR_DialogoOperatore.Application.Interfaces.Clients
{
	public interface IJmesApiClient
	{
        IList<T>? ChiamaQueryGetJmes<T>();
		IList<T>? ChiamaQueryVirtualJmes<T>();
		string? RegistrazioneOperazioneSuDb(Func<HttpResponseMessage> operazione);
        HttpResponseMessage MesAdvanceDeclaration(Operatore operatore, Attivita attivita, int quantitaProdotta, int quantitaScartata);
		HttpResponseMessage MesWorkStart(Operatore operatore, string bolla);
		HttpResponseMessage MesWorkStartIndiretta(string badge, string codiceAttivitaIndiretta);
        HttpResponseMessage MesWorkEnd(string badge, Attivita attivita, int quantitaProdotta, int quantitaScartata);
		HttpResponseMessage MesWorkSuspension(string badge, Attivita attivita, int quantitaProdotta, int quantitaScartata);
		HttpResponseMessage MesWorkResume(string badge, Attivita attivita);
        HttpResponseMessage MesSuspensionStart(string badge, int codiceJmesMacchina);
		HttpResponseMessage MesSuspensionEnd(string badge, int codiceJmesMacchina);
        HttpResponseMessage MesEquipStart(Operatore operatore, string bolla);
		HttpResponseMessage MesEquipEnd(string badge, double? idJmesAttrezzaggio);
		HttpResponseMessage MesEquipSuspension(string badge, double? idJmesAttrezzaggio);

        HttpResponseMessage MesBreakStart(string badge);
		HttpResponseMessage MesBreakEnd(string badge);
		HttpResponseMessage MesAutoClock(string badge, bool isIngresso);
    }
}
