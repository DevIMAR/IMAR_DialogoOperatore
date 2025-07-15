using IMAR_DialogoOperatore.Interfaces.Observers;
using IMAR_DialogoOperatore.ViewModels;

namespace IMAR_DialogoOperatore.Commands
{
    public class ShowEntrateUscitePauseCommand : CommandBase
    {
        private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;
        private readonly PopupTimbratureViewModel _popupTimbratureViewModel;

        public ShowEntrateUscitePauseCommand(
            IDialogoOperatoreObserver dialogoOperatoreObserver,
            PopupTimbratureViewModel popupTimbratureViewModel)
        {
            _dialogoOperatoreObserver = dialogoOperatoreObserver;
            _popupTimbratureViewModel = popupTimbratureViewModel;
        }

        public override bool CanExecute(object? parameter)
        {
            return _dialogoOperatoreObserver.OperatoreSelezionato != null;
        }

        public override void Execute(object? parameter)
        {
            _popupTimbratureViewModel.IsVisible = true;
        }
    }
}
