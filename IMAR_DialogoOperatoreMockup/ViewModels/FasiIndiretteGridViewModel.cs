using IMAR_DialogoOperatore.Application.Interfaces.Services.Activities;
using IMAR_DialogoOperatore.Interfaces.Mappers;
using IMAR_DialogoOperatore.Interfaces.Observers;
using IMAR_DialogoOperatore.Interfaces.ViewModels;

namespace IMAR_DialogoOperatore.ViewModels
{
    public class FasiIndiretteGridViewModel : ViewModelBase
    {
        private readonly IAttivitaMapper _attivitaMapper;
        private readonly IAttivitaService _attivitaService;
        private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;

        private object _attivitaSelezionata;

        public IEnumerable<IAttivitaViewModel> FasiIndirette { get; set; }


        public object AttivitaSelezionata
        {
            get { return _attivitaSelezionata; }
            set
            {
                _attivitaSelezionata = value;
                _dialogoOperatoreObserver.AttivitaSelezionata = (IAttivitaViewModel)value;

                OnNotifyStateChanged();
            }
        }

        public FasiIndiretteGridViewModel(
            IAttivitaMapper attivitaMapper,
            IAttivitaService attivitaService,
            IDialogoOperatoreObserver dialogoOperatoreObserver)
        {
            _attivitaMapper = attivitaMapper;
            _attivitaService = attivitaService;

            _dialogoOperatoreObserver = dialogoOperatoreObserver;

            FasiIndirette = _attivitaMapper.ListaAttivitaToListaAttivitaViewModel(_attivitaService.GetAttivitaIndirette());
        }
    }
}
