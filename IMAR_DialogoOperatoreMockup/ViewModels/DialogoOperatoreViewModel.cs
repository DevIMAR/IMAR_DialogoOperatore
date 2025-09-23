using IMAR_DialogoOperatore.Interfaces.Observers;
using IMAR_DialogoOperatore.Managers;

namespace IMAR_DialogoOperatore.ViewModels
{
	public class DialogoOperatoreViewModel : ViewModelBase
	{
		private readonly IDialogoOperatoreObserver _dialogoOperatoreStore;

		public bool IsLoaderVisibile => _dialogoOperatoreStore.IsLoaderVisibile;

		public PulsantieraGeneraleViewModel PulsantieraGeneraleViewModel { get; set; }


        public DialogoOperatoreViewModel(
			IDialogoOperatoreObserver dialogoOperatoreStore,
			PulsantieraGeneraleViewModel pulsantieraGeneraleViewModel,
            LogoutTimerManager logoutTimerManager)
        {
			_dialogoOperatoreStore = dialogoOperatoreStore;

			PulsantieraGeneraleViewModel = pulsantieraGeneraleViewModel;

			dialogoOperatoreStore.OnIsDettaglioAttivitaOpenChanged += DialogoOperatoreStore_OnPropertyChanged;
			dialogoOperatoreStore.OnIsLoaderVisibileChanged += DialogoOperatoreStore_OnPropertyChanged;
		}

        private void DialogoOperatoreStore_OnPropertyChanged()
		{
			OnNotifyStateChanged();
		}
	}
}
