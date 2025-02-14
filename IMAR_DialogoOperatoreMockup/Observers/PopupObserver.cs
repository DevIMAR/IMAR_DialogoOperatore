using IMAR_DialogoOperatore.Interfaces.Observers;

namespace IMAR_DialogoOperatore.Observers
{
	public class PopupObserver : ObserverBase, IPopupObserver
	{
		private bool _isPopupVisible;
		private string _testoPopup;
		private bool _isConfermato;

		public bool IsPopupVisible
		{
			get { return _isPopupVisible; }
			set
			{
				_isPopupVisible = value;
				CallAction(OnIsPopupVisibleChanged);
			}
		}
		public string TestoPopup
		{
			get { return _testoPopup; }
			set
			{
				_testoPopup = value;
				InvokeAsync(() => CallAction(OnTestoPopupChanged));
			}
		}
		public bool IsConfermato
		{
			get { return _isConfermato; }
			set
			{
				_isConfermato = value;
				CallAction(OnIsConfermatoChanged);
			}
		}

		public event Action? OnIsPopupVisibleChanged;
		public event Action? OnTestoPopupChanged;
		public event Action? OnIsConfermatoChanged;
	}
}
