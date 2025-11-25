using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Interfaces.Observers;
using IMAR_DialogoOperatore.Interfaces.ViewModels;
using IMAR_DialogoOperatore.ViewModels;

namespace IMAR_DialogoOperatore.Commands
{
    public class MostraFasiNonPianificatePopupCommand : CommandBase
    {
        private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;
        private readonly ICercaAttivitaObserver _cercaAttivitaObserver;
        private readonly FasiNonPianificatePopupViewModel _fasiNonPianificatePopupViewModel;

        public MostraFasiNonPianificatePopupCommand(
            IDialogoOperatoreObserver dialogoOperatoreObserver,
            ICercaAttivitaObserver cercaAttivitaObserver,
            FasiNonPianificatePopupViewModel fasiNonPianificatePopupViewModel)
        {
            _dialogoOperatoreObserver = dialogoOperatoreObserver;
            _cercaAttivitaObserver = cercaAttivitaObserver;

            _fasiNonPianificatePopupViewModel = fasiNonPianificatePopupViewModel;
        }

        public override bool CanExecute(object? parameter)
        {
            IAttivitaViewModel? primaFase = _cercaAttivitaObserver.AttivitaTrovate?.OrderBy(a => a.CodiceFase).FirstOrDefault();

            return _dialogoOperatoreObserver.AttivitaSelezionata != null &&
                   !string.IsNullOrWhiteSpace(_dialogoOperatoreObserver.OperazioneInCorso) &&
                   (_dialogoOperatoreObserver.OperazioneInCorso.Equals(Costanti.INIZIO_ATTREZZAGGIO) ||
                        _dialogoOperatoreObserver.OperazioneInCorso.Equals(Costanti.INIZIO_LAVORO)) &&
                   primaFase != null &&
                   (primaFase.DescrizioneFase.Contains(Costanti.TAGLIO) ||
                        primaFase.DescrizioneFase.Contains(Costanti.SPELATURA)) &&
                   primaFase.QuantitaProdotta + primaFase.QuantitaScartata > 0;
        }

        public override void Execute(object? parameter)
        {
            _fasiNonPianificatePopupViewModel.MostraPopup = true;
        }
    }
}
