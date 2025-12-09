using IMAR_DialogoOperatore.Commands;
using IMAR_DialogoOperatore.Interfaces.Observers;
using System.Windows.Input;

namespace IMAR_DialogoOperatore.ViewModels
{
	public class PopupDiConfermaViewModel : PopupViewModelBase
	{
		private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;
		private IPopupObserver _popupStore;
		private string _testo;

		public ICommand RispostaPopupDiConfermaCommand { get; set; }
		public string Testo
		{
			get { return _testo; }
			set 
			{
				_testo = value; 
				OnNotifyStateChanged();
			}
		}

        public PopupDiConfermaViewModel(
			IDialogoOperatoreObserver dialogoOperatoreObserver,
			IPopupObserver popupStore,
			RispostaPopupDiConfermaCommand rispostaPopupDiConfermaCommand)
			: base(dialogoOperatoreObserver)
        {
			_dialogoOperatoreObserver = dialogoOperatoreObserver;
            _popupStore = popupStore;
			RispostaPopupDiConfermaCommand = rispostaPopupDiConfermaCommand;

			IsVisible = false;

            _dialogoOperatoreObserver.OnOperatoreSelezionatoChanged += DialogoOperatoreObserver_OnOperatoreSelezionatoChanged;
			_popupStore.OnIsPopupVisibleChanged += PopupStore_OnIsPopupVisibleChanged;
			_popupStore.OnTestoPopupChanged += PopupStore_OnTestoPopupChanged;
        }

        public override void DialogoOperatoreObserver_OnOperatoreSelezionatoChanged()
        {
			RispostaPopupDiConfermaCommand.Execute(false);
        }

        private void PopupStore_OnTestoPopupChanged()
		{
			Testo = _popupStore.TestoPopup;
		}

		private void PopupStore_OnIsPopupVisibleChanged()
		{
			IsVisible = _popupStore.IsPopupVisible;	
		}
	}
}
