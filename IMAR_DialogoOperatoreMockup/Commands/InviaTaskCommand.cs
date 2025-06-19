using IMAR_DialogoOperatore.Application.Interfaces.Clients;
using IMAR_DialogoOperatore.Interfaces.Observers;
using IMAR_DialogoOperatore.ViewModels;

namespace IMAR_DialogoOperatore.Commands
{
    public class InviaTaskCommand : CommandBase
    {
        private readonly TaskPopupViewModel _taskPopupViewModel;
        private readonly IImarApiClient _imarApiClient;
        private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;

        public InviaTaskCommand(
            TaskPopupViewModel taskPopupViewModel,
            IImarApiClient imarApiClient,
            IDialogoOperatoreObserver dialogoOperatoreObserver)
        {
            _taskPopupViewModel = taskPopupViewModel;
            _imarApiClient = imarApiClient;
            _dialogoOperatoreObserver = dialogoOperatoreObserver;
        }

        public override bool CanExecute(object? parameter)
        {
            return _taskPopupViewModel.CategoriaErroreSelezionata != null &&
                   !string.IsNullOrWhiteSpace(_taskPopupViewModel.TaskAsana.Html_notes); 
        }

        public override async void Execute(object? parameter)
        {
            string firmaOperatore = _dialogoOperatoreObserver.OperatoreSelezionato.Nome + " " + _dialogoOperatoreObserver.OperatoreSelezionato.Cognome;
            _taskPopupViewModel.TaskAsana.Html_notes += "\n\n" + firmaOperatore;

            string feedback = await _imarApiClient.SendTaskAsana(_taskPopupViewModel.TaskAsana, "server@imarsrl.com");

            _taskPopupViewModel.Visible = false;
        }
    }
}
