using IMAR_DialogoOperatore.Application.Interfaces.Services.Activities;
using IMAR_DialogoOperatore.Domain.Models;
using IMAR_DialogoOperatore.Interfaces.Mappers;
using IMAR_DialogoOperatore.Interfaces.Observers;
using IMAR_DialogoOperatore.Interfaces.ViewModels;

namespace IMAR_DialogoOperatore.ViewModels
{
    public class NoteGridViewModel : ViewModelBase
    {
        private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;
        private readonly INotaService _notaService;
        private readonly IAttivitaMapper _attivitaMapper;
        private readonly INotaMapper _notaMapper;

        public IEnumerable<INotaViewModel>? Note { get; set; }

        public NoteGridViewModel(
            IDialogoOperatoreObserver dialogoOperatoreObserver,
            INotaService notaService,
            IAttivitaMapper attivitaMapper,
            INotaMapper notaMapper)
        {
            _dialogoOperatoreObserver = dialogoOperatoreObserver;
            _notaService = notaService;
            _attivitaMapper = attivitaMapper;

            _dialogoOperatoreObserver.OnAttivitaSelezionataChanged += DialogoOperatoreObserver_OnAttivitaSelezionataChanged;
            _notaMapper = notaMapper;
        }

        private void DialogoOperatoreObserver_OnAttivitaSelezionataChanged()
        {
            Note = _notaMapper.ListNotaToListNotaViewModel(_dialogoOperatoreObserver.AttivitaSelezionata?.Note);
        }

        public IEnumerable<INotaViewModel> GetNoteAttivita()
        {
            return _notaMapper.ListNotaToListNotaViewModel(
                        _notaService.GetNoteAttivita(
                            _attivitaMapper.AttivitaViewModelToAttivita(
                                _dialogoOperatoreObserver.AttivitaSelezionata)));
        }

        public void InsertNuovaNota(INotaViewModel nota)
        {
            Attivita? attivita = _attivitaMapper.AttivitaViewModelToAttivita(_dialogoOperatoreObserver.AttivitaSelezionata);
            _notaService.AggiungiNota(attivita, nota?.Testo);
            _dialogoOperatoreObserver.AttivitaSelezionata.Note = _notaService.GetNoteAttivita(attivita);
        }
    }
}
