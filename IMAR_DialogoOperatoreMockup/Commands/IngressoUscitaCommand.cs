﻿using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Application.Interfaces.Clients;
using IMAR_DialogoOperatore.Application.Interfaces.Utilities;
using IMAR_DialogoOperatore.Domain.Models;
using IMAR_DialogoOperatore.Interfaces.Helpers;
using IMAR_DialogoOperatore.Interfaces.Observers;
using IMAR_DialogoOperatore.Interfaces.ViewModels;
using IMAR_DialogoOperatore.Utilities;
using IMAR_DialogoOperatore.ViewModels;

namespace IMAR_DialogoOperatore.Commands
{
    public class IngressoUscitaCommand : CommandBase
    {
        private readonly InfoOperatoreViewModel _infoOperatoreViewModel;
        private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;
        private readonly IInterruzioneAttivitaHelper _interruzioneAttivitaHelper;
		private readonly IJmesApiClient _jmesApiClient;
        private readonly IAutoLogoutUtility _autoLogoutUtility;
        private readonly ToastDisplayerUtility _toastDisplayerUtility;

        public IngressoUscitaCommand(
			InfoOperatoreViewModel infoOperatoreViewModel,
			IDialogoOperatoreObserver dialogoOperatoreObserver,
			IInterruzioneAttivitaHelper interruzioneLavoroHelper,
			IJmesApiClient jmesApiClient,
            IAutoLogoutUtility autoLogoutUtility,
            ToastDisplayerUtility toastDisplayerUtility)
        {
			_infoOperatoreViewModel = infoOperatoreViewModel;
            _dialogoOperatoreObserver = dialogoOperatoreObserver;
			_interruzioneAttivitaHelper = interruzioneLavoroHelper;
			_jmesApiClient = jmesApiClient;
            _autoLogoutUtility = autoLogoutUtility;
            _toastDisplayerUtility = toastDisplayerUtility;

            _autoLogoutUtility.OnLogoutTriggered += AutoLogoutUtility_OnLogoutTriggered;
        }

        private void AutoLogoutUtility_OnLogoutTriggered()
        {
            _infoOperatoreViewModel.Badge = null;
            _dialogoOperatoreObserver.IsUscita = false;
        }

        public override bool CanExecute(object? parameter)
		{
			return _dialogoOperatoreObserver.OperatoreSelezionato != null
					&& _dialogoOperatoreObserver.OperatoreSelezionato.Badge != null
                    && !_dialogoOperatoreObserver.IsDettaglioAttivitaOpen
                    && !_dialogoOperatoreObserver.IsUscita
					&& _dialogoOperatoreObserver.OperatoreSelezionato.Stato != Costanti.IN_PAUSA;
		}

		public override async void Execute(object? parameter)
        {
            if (_dialogoOperatoreObserver.OperatoreSelezionato.Stato == Costanti.ASSENTE)
                await EffettuaIngressoOperatore();
            else
                await EffettuaUscitaOperatore();
        }

        private async Task EffettuaIngressoOperatore()
        {
            _dialogoOperatoreObserver.IsLoaderVisibile = true;
            await Task.Delay(1);

            //RiaperturaAttivitaSospese();

            _jmesApiClient.MesAutoClock(_dialogoOperatoreObserver.OperatoreSelezionato.Badge.ToString(), true);

            _dialogoOperatoreObserver.IsLoaderVisibile = false;

            _dialogoOperatoreObserver.OperatoreSelezionato.Stato = Costanti.PRESENTE;

            _toastDisplayerUtility.ShowGreenToast("Entrata", $"Benvenuto {_dialogoOperatoreObserver.OperatoreSelezionato.Nome}!");
        }

        private void RiaperturaAttivitaSospese()
        {
            IOperatoreViewModel operatore = _dialogoOperatoreObserver.OperatoreSelezionato;
            foreach (Attivita attivita in operatore.AttivitaAperte)
                _jmesApiClient.MesWorkResume(operatore.Badge.ToString(), attivita);
        }

        private async Task EffettuaUscitaOperatore()
        {
            _dialogoOperatoreObserver.IsUscita = true;

            await AvanzaAttivitaOperatore();

            if (_dialogoOperatoreObserver.IsOperazioneAnnullata)
            {
                _dialogoOperatoreObserver.IsUscita = false;
                return;
            }

            _dialogoOperatoreObserver.IsLoaderVisibile = true;
            await Task.Delay(1);

            _jmesApiClient.MesAutoClock(_dialogoOperatoreObserver.OperatoreSelezionato.Badge.ToString(), false);

            _dialogoOperatoreObserver.IsLoaderVisibile = false;

            _dialogoOperatoreObserver.OperatoreSelezionato.Stato = Costanti.ASSENTE;

            _toastDisplayerUtility.ShowRedToast("Uscita", $"Arrivederci {_dialogoOperatoreObserver.OperatoreSelezionato.Nome}!");

            _autoLogoutUtility.StartLogoutTimer(3);
        }

        private async Task ChiudiAttivitaOperatore()
        {
            try
            {
                IAttivitaViewModel attivita;
                IOperatoreViewModel operatore = _dialogoOperatoreObserver.OperatoreSelezionato;

                for (int i = 0; i < operatore.AttivitaAperte.Count; i++)
                {
                    attivita = new AttivitaViewModel(operatore.AttivitaAperte[i]);

                    if (attivita.Causale == Costanti.LAVORO_SOSPESO || attivita.Causale == Costanti.ATTREZZAGGIO_SOSPESO)
                        continue;

                    await _interruzioneAttivitaHelper.GestisciInterruzioneAttivita(attivita, true);

                    if (_dialogoOperatoreObserver.IsOperazioneAnnullata)
                        break;
                }
            }
            catch (Exception ex)
            {
                // Log dell'errore
                Console.WriteLine($"Errore durante la chiusura delle attività: {ex.Message}");
            }
        }

        private async Task AvanzaAttivitaOperatore()
        {
            IAttivitaViewModel attivita;
            IOperatoreViewModel operatore = _dialogoOperatoreObserver.OperatoreSelezionato;

            for (int i = 0; i < operatore.AttivitaAperte.Count; i++)
            {
                attivita = new AttivitaViewModel(operatore.AttivitaAperte[i]);

                if (attivita.Causale == Costanti.IN_ATTREZZAGGIO)
                    continue;

                await _interruzioneAttivitaHelper.GestisciInterruzioneAttivita(attivita, false);

                if (_dialogoOperatoreObserver.IsOperazioneAnnullata)
                    break;
            }
        }

        public override void Dispose()
        {
            _autoLogoutUtility.OnLogoutTriggered -= AutoLogoutUtility_OnLogoutTriggered;
        }
    }
}
