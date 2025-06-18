using IMAR_DialogoOperatore.Application.Interfaces.Clients;
using IMAR_DialogoOperatore.ViewModels;

namespace IMAR_DialogoOperatore.Commands
{
    public class InviaTaskCommand : CommandBase
    {
        private readonly TaskPopupViewModel _taskPopupViewModel;
        private readonly IImarApiClient _imarApiClient;

        public InviaTaskCommand(
            TaskPopupViewModel taskPopupViewModel,
            IImarApiClient imarApiClient)
        {
            _taskPopupViewModel = taskPopupViewModel;
            _imarApiClient = imarApiClient;
        }

        public override bool CanExecute(object? parameter)
        {
            return _taskPopupViewModel.CategoriaErroreSelezionata != null &&
                   !string.IsNullOrWhiteSpace(_taskPopupViewModel.TaskAsana.Html_notes); 
        }

        public override void Execute(object? parameter)
        {
            _imarApiClient.SendTaskAsana(_taskPopupViewModel.TaskAsana, "server@imarsrl.com");

            _taskPopupViewModel.Visible = false;
        }
    }
}
