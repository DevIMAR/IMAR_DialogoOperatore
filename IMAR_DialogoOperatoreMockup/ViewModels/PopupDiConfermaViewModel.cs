using IMAR_DialogoOperatore.Commands;
using IMAR_DialogoOperatore.Interfaces.Observers;
using System.Windows.Input;

namespace IMAR_DialogoOperatore.ViewModels
{
	public class PopupDiConfermaViewModel : ViewModelBase
	{
		private IPopupObserver _popupStore;
		private bool _isVisibile;
		private string _testo;

		public ICommand RispostaPopupDiConfermaCommand { get; set; }

		public bool IsVisible
		{
			get { return _isVisibile; }
			set
			{
				_isVisibile = value;
				OnNotifyStateChanged();
			}
		}
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
			IPopupObserver popupStore,
			RispostaPopupDiConfermaCommand rispostaPopupDiConfermaCommand)
        {
            _popupStore = popupStore;
			RispostaPopupDiConfermaCommand = rispostaPopupDiConfermaCommand;

			IsVisible = false;

			_popupStore.OnIsPopupVisibleChanged += PopupStore_OnIsPopupVisibleChanged;
			_popupStore.OnTestoPopupChanged += PopupStore_OnTestoPopupChanged;
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
