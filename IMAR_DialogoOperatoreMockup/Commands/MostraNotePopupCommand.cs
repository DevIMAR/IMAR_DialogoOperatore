using IMAR_DialogoOperatore.Interfaces.Observers;
using IMAR_DialogoOperatore.ViewModels;

namespace IMAR_DialogoOperatore.Commands
{
    public class MostraNotePopupCommand : CommandBase
    {
        private readonly NotePopupViewModel _notePopupViewModel;
        private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;

        public MostraNotePopupCommand(
            NotePopupViewModel notePopupViewModel,
            IDialogoOperatoreObserver dialogoOperatoreObserver)
        {
            _notePopupViewModel = notePopupViewModel;
            _dialogoOperatoreObserver = dialogoOperatoreObserver;
        }

        public override bool CanExecute(object? parameter)
        {
            return _dialogoOperatoreObserver.AttivitaSelezionata != null &&
                    (_dialogoOperatoreObserver.AttivitaSelezionata.Bolla.Length == 5 
                        && _dialogoOperatoreObserver.AttivitaSelezionata.Bolla.Contains("AI"));
        }

        public override void Execute(object? parameter)
        {
            _notePopupViewModel.IsVisible = true;
        }
    }
}
