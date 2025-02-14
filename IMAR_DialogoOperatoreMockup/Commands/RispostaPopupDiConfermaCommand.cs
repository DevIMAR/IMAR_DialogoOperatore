using IMAR_DialogoOperatore.Interfaces.Observers;

namespace IMAR_DialogoOperatore.Commands
{
	public class RispostaPopupDiConfermaCommand : CommandBase
	{
		IPopupObserver _popupStore;

        public RispostaPopupDiConfermaCommand(IPopupObserver popupStore)
        {
            _popupStore = popupStore;
        }

        public override async void Execute(object? parameter)
        {
            if (parameter is bool isConfermato)
            {
                _popupStore.IsPopupVisible = false;
                _popupStore.IsConfermato = isConfermato;
            }
        }
	}
}
