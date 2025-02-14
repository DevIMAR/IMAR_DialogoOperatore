﻿
using IMAR_DialogoOperatore.Domain.Models;

namespace IMAR_DialogoOperatore.Application.Interfaces.Clients
{
	public interface IJmesApiClient
	{
        IList<T>? ChiamaQueryGetJmes<T>();
		IList<T>? ChiamaQueryVirtualJmes<T>();
		string? RegistrazioneOperazioneSuDb(Func<HttpResponseMessage> operazione);
        HttpResponseMessage MesAdvanceDeclaration(string badge, Attivita attivita, int quantitaProdotta, int quantitaScartata);
		HttpResponseMessage MesWorkStart(string badge, string bolla);
		HttpResponseMessage MesWorkStartIndiretta(string badge, string codiceAttivitaIndiretta);
        HttpResponseMessage MesWorkEnd(string badge, Attivita attivita, int quantitaProdotta, int quantitaScartata);
        HttpResponseMessage MesSuspensionStart(string badge, int codiceJmesMacchina);
		HttpResponseMessage MesSuspensionEnd(string badge, int codiceJmesMacchina);
        HttpResponseMessage MesEquipStart(string badge, string bolla, int codiceMacchinaGalileo);
		HttpResponseMessage MesEquipEnd(string badge, double? idJmesAttrezzaggio);
		HttpResponseMessage MesBreakStart(string badge);
		HttpResponseMessage MesBreakEnd(string badge);
		HttpResponseMessage MesAutoClock(string badge, bool isIngresso);
    }
}
