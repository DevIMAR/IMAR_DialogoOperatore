
using IMAR_DialogoOperatore.Domain.Models;

namespace IMAR_DialogoOperatore.Application.Interfaces.Clients
{
	public interface IJmesApiClient
	{
        Task<IList<T>?> ChiamaQueryGetJmesAsync<T>();
		Task<IList<T>?> ChiamaQueryVirtualJmesAsync<T>();
		Task<string?> RegistrazioneOperazioneSuDbAsync(Func<Task<HttpResponseMessage>> operazione);
        Task<HttpResponseMessage> MesAdvanceDeclarationAsync(Operatore operatore, Attivita attivita, int quantitaProdotta, int quantitaScartata);
		Task<HttpResponseMessage> MesWorkStartAsync(Operatore operatore, Attivita attivita);
		Task<HttpResponseMessage> MesWorkStartNotPlnAsync(Operatore operatore, Attivita attivita, string codiceFase);
		Task<HttpResponseMessage> MesWorkStartIndirettaAsync(string badge, string codiceAttivitaIndiretta);
        Task<HttpResponseMessage> MesWorkEndAsync(string badge, Attivita attivita, int quantitaProdotta, int quantitaScartata);
		Task<HttpResponseMessage> MesWorkSuspensionAsync(string badge, Attivita attivita, int quantitaProdotta, int quantitaScartata);
		Task<HttpResponseMessage> MesWorkResumeAsync(string badge, Attivita attivita);
        Task<HttpResponseMessage> MesSuspensionStartAsync(string badge, int codiceJmesMacchina);
		Task<HttpResponseMessage> MesSuspensionEndAsync(string badge, int codiceJmesMacchina);
        Task<HttpResponseMessage> MesEquipStartAsync(Operatore operatore, string bolla, Macchina? macchina = null);
		Task<HttpResponseMessage> MesEquipStartNotPlnAsync(Operatore operatore, string bolla, string codiceFase);
        Task<HttpResponseMessage> MesEquipEndAsync(string badge, double? idJmesAttrezzaggio);
        Task<HttpResponseMessage> MesEquipRemoveAsync(string badge, double? idJmesAttrezzaggio);
		Task<HttpResponseMessage> MesEquipSuspensionAsync(string badge, double? idJmesAttrezzaggio);
		Task<HttpResponseMessage> MesEquipResumeAsync(string badge, Attivita attivita);

        Task<HttpResponseMessage> MesBreakStartAsync(string badge);
		Task<HttpResponseMessage> MesBreakEndAsync(string badge);
		Task<HttpResponseMessage> MesAutoClockAsync(string badge, bool isIngresso);
    }
}
