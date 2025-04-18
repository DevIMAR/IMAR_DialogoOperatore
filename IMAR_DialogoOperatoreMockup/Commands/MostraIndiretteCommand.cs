using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Interfaces.Observers;
using IMAR_DialogoOperatore.Observers;
using IMAR_DialogoOperatore.ViewModels;

namespace IMAR_DialogoOperatore.Commands
{
    public class MostraIndiretteCommand : CommandBase
    {
        private readonly FasiIndirettePopupViewModel _fasiIndirettePopupViewModel;
        private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;
        private readonly IAttivitaIndirettaObserver _attivitaIndirettaObserver;

        public MostraIndiretteCommand(
            FasiIndirettePopupViewModel fasiIndirettePopupViewModel,
            IDialogoOperatoreObserver dialogoOperatoreObserver,
            IAttivitaIndirettaObserver attivitaIndirettaObserver)
        {
            _fasiIndirettePopupViewModel = fasiIndirettePopupViewModel;
            _dialogoOperatoreObserver = dialogoOperatoreObserver;
            _attivitaIndirettaObserver = attivitaIndirettaObserver;
        }

        public override bool CanExecute(object? parameter)
        {
            return _dialogoOperatoreObserver.OperazioneInCorso == Costanti.INIZIO_LAVORO && 
                   base.CanExecute(parameter);
        }

        public override void Execute(object? parameter)
        {
            _attivitaIndirettaObserver.IsAttivitaIndiretta = true;
            _fasiIndirettePopupViewModel.MostraPopup = true;
        }
    }
}
