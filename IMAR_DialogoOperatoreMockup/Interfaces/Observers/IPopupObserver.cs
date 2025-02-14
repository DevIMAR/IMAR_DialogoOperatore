namespace IMAR_DialogoOperatore.Interfaces.Observers
{
	public interface IPopupObserver
	{
		bool IsConfermato { get; set; }
		bool IsPopupVisible { get; set; }
		string TestoPopup { get; set; }

		event Action OnIsConfermatoChanged;
		event Action OnIsPopupVisibleChanged;
		event Action OnTestoPopupChanged;
	}
}