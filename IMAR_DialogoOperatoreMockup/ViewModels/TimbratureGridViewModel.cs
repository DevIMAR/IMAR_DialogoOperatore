using IMAR_DialogoOperatore.Application.Interfaces.Services.Activities;
using IMAR_DialogoOperatore.Domain.Models;
using IMAR_DialogoOperatore.Interfaces.Observers;

namespace IMAR_DialogoOperatore.ViewModels
{
    public class TimbratureGridViewModel : ViewModelBase
    {
        private readonly PopupTimbratureViewModel _popupTimbratureViewModel;
        private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;
        private readonly ITimbratureService _timbratureService;

        private IList<Timbratura> _timbratureOperatore;

        public IList<Timbratura> TimbratureOperatore
        { 
            get { return _timbratureOperatore; }
            set 
            {
                _timbratureOperatore = value; 
                OnNotifyStateChanged();
            }
        }

        public TimbratureGridViewModel(
            PopupTimbratureViewModel popupTimbratureViewModel,
            IDialogoOperatoreObserver dialogoOperatoreObserver,
            ITimbratureService timbratureService)
        {
            _popupTimbratureViewModel = popupTimbratureViewModel;
            _dialogoOperatoreObserver = dialogoOperatoreObserver;
            _timbratureService = timbratureService;
            
            if (_popupTimbratureViewModel.IsVisible && _dialogoOperatoreObserver.OperatoreSelezionato != null)
            {
                TimbratureOperatore = _timbratureService.GetTimbratureOperatore(
                    _dialogoOperatoreObserver.OperatoreSelezionato.Badge.ToString());
            }

            _popupTimbratureViewModel.NotifyStateChanged += PopupTimbratureViewModel_NotifyStateChanged; 
        }

        private void PopupTimbratureViewModel_NotifyStateChanged()
        {
            if (_popupTimbratureViewModel.IsVisible)
                TimbratureOperatore = _timbratureService.GetTimbratureOperatore(_dialogoOperatoreObserver.OperatoreSelezionato.Badge.ToString());
        }
    }
}
