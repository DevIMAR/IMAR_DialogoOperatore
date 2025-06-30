using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Application.Interfaces.Clients;
using IMAR_DialogoOperatore.Interfaces.Helpers;
using IMAR_DialogoOperatore.Interfaces.Observers;

namespace IMAR_DialogoOperatore.Commands
{
    public class InviaTaskCommand : CommandBase
    {
        private readonly IImarApiClient _imarApiClient;
        private readonly ITaskCompilerHelper _taskCompilerHelper;
        private readonly ITaskCompilerObserver _taskCompilerObserver;

        public InviaTaskCommand(
            IImarApiClient imarApiClient,
            ITaskCompilerHelper taskCompilerHelper,
            ITaskCompilerObserver taskCompilerObserver)
        {
            _imarApiClient = imarApiClient;
            _taskCompilerHelper = taskCompilerHelper;
            _taskCompilerObserver = taskCompilerObserver;

            _taskCompilerObserver.OnCategoriaErroreSelezionataChanged += CanExecuteEvaluator;
            _taskCompilerObserver.OnNoteChanged += CanExecuteEvaluator;
        }

        private void CanExecuteEvaluator()
        {
            CanExecute(null);
        }

        public override bool CanExecute(object? parameter)
        {
            return _taskCompilerObserver.CategoriaErroreSelezionata != null &&
                   !(_taskCompilerObserver.CategoriaErroreSelezionata == Costanti.TASK_CHIUSURA_A_SALDO_ERRATA &&
                   string.IsNullOrWhiteSpace(_taskCompilerObserver.Note)); 
        }

        public override async void Execute(object? parameter)
        {
            _taskCompilerHelper.CompilaTaskAsana();
            string feedback = await _imarApiClient.SendTaskAsana(_taskCompilerHelper.TaskAsana, "luca.marangoni@imarsrl.com");
        }
    }
}
