using IMAR_DialogoOperatore.Interfaces.Observers;
using IMAR_DialogoOperatore.ViewModels;

namespace IMAR_DialogoOperatore.Commands
{
    public class ShowTaskPopupCommand : CommandBase
    {
        private readonly TaskPopupViewModel _taskPopupViewModel;
        private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;

        public ShowTaskPopupCommand(
            TaskPopupViewModel taskPopupViewModel,
            IDialogoOperatoreObserver dialogoOperatoreObserver)
        {
            _taskPopupViewModel = taskPopupViewModel;
            _dialogoOperatoreObserver = dialogoOperatoreObserver;
        }

        public override bool CanExecute(object? parameter)
        {
            return _dialogoOperatoreObserver.OperatoreSelezionato != null;
        }

        public override void Execute(object? parameter)
        {
            if (parameter is bool isVisibile)
                _taskPopupViewModel.Visible = isVisibile;
        }
    }
}
