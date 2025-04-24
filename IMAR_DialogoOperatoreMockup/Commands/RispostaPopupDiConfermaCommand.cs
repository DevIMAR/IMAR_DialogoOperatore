using IMAR_DialogoOperatore.Interfaces.Observers;
using IMAR_DialogoOperatore.Observers;

namespace IMAR_DialogoOperatore.Commands
{
	public class RispostaPopupDiConfermaCommand : CommandBase
	{
		IPopupObserver _popupObserver;
        IAvanzamentoObserver _avanzamentoObserver;
        ISegnalazioneObserver _segnalazioneObserver;

        public RispostaPopupDiConfermaCommand(
            IPopupObserver popupObserver,
            IAvanzamentoObserver avanzamentoObserver,
            ISegnalazioneObserver segnalazioneObserver)
        {
            _popupObserver = popupObserver;
            _avanzamentoObserver = avanzamentoObserver;
            _segnalazioneObserver = segnalazioneObserver;
        }

        public override bool CanExecute(object? parameter)
        {
            return (_avanzamentoObserver.QuantitaScartata <= 0 ||
                        _avanzamentoObserver.QuantitaScartata == null ||
                        !string.IsNullOrWhiteSpace(_segnalazioneObserver.DescrizioneDifetto)
                  ) &&
                  base.CanExecute(parameter);
        }

        public override async void Execute(object? parameter)
        {
            if (parameter is bool isConfermato)
            {
                _popupObserver.IsPopupVisible = false;
                _popupObserver.IsConfermato = isConfermato;
            }
        }
	}
}
