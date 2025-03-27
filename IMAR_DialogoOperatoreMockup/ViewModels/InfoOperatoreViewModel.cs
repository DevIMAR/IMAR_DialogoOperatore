using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Application.Interfaces.Services.Activities;
using IMAR_DialogoOperatore.Application.Interfaces.Utilities;
using IMAR_DialogoOperatore.Domain.Models;
using IMAR_DialogoOperatore.Interfaces.Observers;
using IMAR_DialogoOperatore.Interfaces.ViewModels;

namespace IMAR_DialogoOperatore.ViewModels
{
    public class InfoOperatoreViewModel : ViewModelBase
    {
        private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;
        private readonly IOperatoriService _operatoriService;
        private readonly IAutoLogoutUtility _autoLogoutUtility;

        private int? _badge;
        private IOperatoreViewModel? _operatoreSelezionato;

        public string NomeCognome => OperatoreSelezionato == null ? "" : OperatoreSelezionato.Nome + " " + OperatoreSelezionato.Cognome;
        public string Stato => OperatoreSelezionato == null ? "" : OperatoreSelezionato.Stato;
        public bool IsInfoVisible => OperatoreSelezionato != null && OperatoreSelezionato.Badge != null;
        public bool IsBadgeEditable => OperatoreSelezionato == null;


        public int? Badge
        {
            get { return _badge; }
            set
            {
                _badge = value;

                SelezionaOperatore();

                if (value != null)
                    _autoLogoutUtility.StartLogoutTimer(30);
            }
        }

        public IOperatoreViewModel? OperatoreSelezionato
        {
            get { return _operatoreSelezionato; }
            set
            {
                if (_operatoreSelezionato == value)
                    return;

                UpdateOperatoreSelezionato(value);

                OnNotifyStateChanged();
            }
        }

        public InfoOperatoreViewModel(
            IDialogoOperatoreObserver dialogoOperatoreStore,
            IOperatoriService operatoriService,
            IAutoLogoutUtility autoLogoutUtility)
        {
            _dialogoOperatoreObserver = dialogoOperatoreStore;
            _operatoriService = operatoriService;
            _autoLogoutUtility = autoLogoutUtility;

            _autoLogoutUtility.OnLogoutTriggered += AutoLogoutUtility_OnLogoutTriggered;
            _dialogoOperatoreObserver.OnIsDettaglioAttivitaOpenChanged += DialogoOperatoreObserver_OnIsDettaglioAttivitaOpenChanged;
            _dialogoOperatoreObserver.OnOperatoreSelezionatoChanged += DialogoOperatoreObserver_OnOperatoreSelezionatoChanged;
        }

        private void DialogoOperatoreObserver_OnIsDettaglioAttivitaOpenChanged()
        {
            OnNotifyStateChanged();
        }

        private void DialogoOperatoreObserver_OnOperatoreSelezionatoChanged()
        {
            OperatoreSelezionato = _dialogoOperatoreObserver.OperatoreSelezionato;
        }

        private void AutoLogoutUtility_OnLogoutTriggered()
        {
            if (Badge == null)
                return;

            Badge = null;
            _autoLogoutUtility.IsActive = false;
        }

        private void OperatoreSelezionato_NotifyStateChanged()
        {
            OnNotifyStateChanged();
        }

        private async void SelezionaOperatore()
        {
            _dialogoOperatoreObserver.IsLoaderVisibile = true;
            await Task.Delay(1);
            Operatore? operatore = _operatoriService.OttieniOperatore(Badge);
            _dialogoOperatoreObserver.IsLoaderVisibile = false;

            OperatoreSelezionato = operatore != null ? new OperatoreViewModel(operatore) : null;
        }

        private void UpdateOperatoreSelezionato(IOperatoreViewModel? value)
        {
            UnsubscribeEventoOperatore();

            _operatoreSelezionato = value;
            _dialogoOperatoreObserver.OperatoreSelezionato = OperatoreSelezionato;

            if (OperatoreSelezionato != null)
            {
                ((OperatoreViewModel)OperatoreSelezionato).NotifyStateChanged += OperatoreSelezionato_NotifyStateChanged;
            }

            _dialogoOperatoreObserver.AttivitaSelezionata = null;
            _dialogoOperatoreObserver.OperazioneInCorso = Costanti.NESSUNA;
        }

        private void UnsubscribeEventoOperatore()
        {
            if (OperatoreSelezionato != null)
                ((OperatoreViewModel)OperatoreSelezionato).NotifyStateChanged -= OperatoreSelezionato_NotifyStateChanged;
        }

        public override void Dispose()
        {
            _dialogoOperatoreObserver.OnIsDettaglioAttivitaOpenChanged -= DialogoOperatoreObserver_OnIsDettaglioAttivitaOpenChanged;
        }
    }
}
