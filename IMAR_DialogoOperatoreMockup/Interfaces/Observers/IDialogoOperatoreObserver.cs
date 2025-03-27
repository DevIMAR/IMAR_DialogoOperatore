using IMAR_DialogoOperatore.Interfaces.ViewModels;

namespace IMAR_DialogoOperatore.Interfaces.Observers
{
    public interface IDialogoOperatoreObserver
	{
		bool IsUscita { get; set; }
		bool IsLoaderVisibile { get; set; }
		IAttivitaViewModel? AttivitaSelezionata { get; set; }
		bool IsDettaglioAttivitaOpen { get; set; }
		bool IsOperazioneAnnullata { get; set; }
		IEnumerable<IAttivitaViewModel>? ListaAttivita { get; set; }
		IOperatoreViewModel? OperatoreSelezionato { get; set; }
		string? OperazioneInCorso { get; set; }
		bool IsRiaperturaAttiva { get; set; }
		bool IsAperturaLavoroAutomaticaAttiva { get; set; }
		bool IsOperazioneGestita { get; set; }

		event Action? OnIsUscitaChanged;
		event Action? OnIsLoaderVisibileChanged;
		event Action? OnAttivitaSelezionataChanged;
		event Action? OnIsDettaglioAttivitaOpenChanged;
		event Action? OnIsOperazioneAnnullataChanged;
		event Action? OnListaAttivitaChanged;
		event Action? OnOperatoreSelezionatoChanged;
		event Action? OnOperazioneInCorsoChanged;
		event Action? OnIsRiaperturaAttivaChanged;
		event Action? OnIsAperturaLavoroAutomaticaAttivaChanged;
		event Action? OnIsOperazioneGestitaChanged;
	}
}