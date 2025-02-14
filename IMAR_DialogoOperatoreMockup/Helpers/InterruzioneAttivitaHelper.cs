using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Application.Interfaces.ViewModels;
using IMAR_DialogoOperatore.Interfaces.Helpers;
using IMAR_DialogoOperatore.Interfaces.Observers;

namespace IMAR_DialogoOperatore.Helpers
{
	public class InterruzioneAttivitaHelper : IInterruzioneAttivitaHelper
	{
		private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;

		private TaskCompletionSource<bool> _tcs;

		public InterruzioneAttivitaHelper(
			IDialogoOperatoreObserver dialogoOperatoreObserver)
		{
			_dialogoOperatoreObserver = dialogoOperatoreObserver;
		}

		public async Task GestisciInterruzioneAttivita(IAttivitaViewModel attivita, bool isUscita)
		{
			try
			{
				string fineLavoroOAvanzamento = isUscita ? Costanti.FINE_LAVORO : Costanti.AVANZAMENTO;

				string causale = attivita.Causale;
				_dialogoOperatoreObserver.OperazioneInCorso = causale == Costanti.IN_LAVORO ? fineLavoroOAvanzamento : Costanti.FINE_ATTREZZAGGIO;
				_dialogoOperatoreObserver.AttivitaSelezionata = attivita;

				SottoscriviAdEventoCorrispondente(causale);

				await AttendiChiusuraAttivita();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Errore durante l'interruzione dell'attività: {ex.Message}");
			}
			finally
			{
				DisiscriviDaEventi();
			}
		}

		private void SottoscriviAdEventoCorrispondente(string causale)
		{
            _dialogoOperatoreObserver.OnIsOperazioneGestitaChanged += DialogoOperatoreObserver_OnIsOperazioneGestitaChanged;
			_dialogoOperatoreObserver.OnIsOperazioneAnnullataChanged += DialogoOperatoreStore_OnIsOperazioneAnnullataChanged;
		}

        private void DisiscriviDaEventi()
        {
            _dialogoOperatoreObserver.OnIsOperazioneGestitaChanged -= DialogoOperatoreObserver_OnIsOperazioneGestitaChanged;
            _dialogoOperatoreObserver.OnIsOperazioneAnnullataChanged -= DialogoOperatoreStore_OnIsOperazioneAnnullataChanged;
		}

        private void DialogoOperatoreObserver_OnIsOperazioneGestitaChanged()
        {
            if (!_dialogoOperatoreObserver.IsOperazioneGestita)
                return;

            _dialogoOperatoreObserver.OnIsOperazioneGestitaChanged -= DialogoOperatoreObserver_OnIsOperazioneGestitaChanged;

            ChiudiAttivita();
        }

		private void DialogoOperatoreStore_OnIsOperazioneAnnullataChanged()
		{
			if (!_dialogoOperatoreObserver.IsOperazioneAnnullata)
				return;

			_dialogoOperatoreObserver.OnIsOperazioneAnnullataChanged -= DialogoOperatoreStore_OnIsOperazioneAnnullataChanged;

			ChiudiAttivita();
		}

		private Task AttendiChiusuraAttivita()
		{
			_tcs = new TaskCompletionSource<bool>();

			return _tcs.Task;
		}

		private void ChiudiAttivita()
		{
			if (!_tcs.Task.IsCompleted)
				_tcs?.SetResult(true);
		}
	}
}
