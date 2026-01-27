using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Application.Interfaces.Services.Activities;
using IMAR_DialogoOperatore.Domain.Models;
using IMAR_DialogoOperatore.Interfaces.Helpers;
using IMAR_DialogoOperatore.Interfaces.Mappers;
using IMAR_DialogoOperatore.Interfaces.Observers;
using IMAR_DialogoOperatore.Interfaces.ViewModels;
using IMAR_DialogoOperatore.ViewModels;

namespace IMAR_DialogoOperatore.Helpers
{
    public class CreaFaseNonPianificataHelper : ICreaFaseNonPianificataHelper
    {
        private readonly IAttivitaMapper _attivitaMapper;
        private readonly IOperatoreMapper _operatoreMapper;
        private readonly IAttivitaService _attivitaService;
        private readonly IForzaturaService _forzaturaService;
        private readonly IOperatoreService _operatoreService;
        private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;

        public CreaFaseNonPianificataHelper(
            IAttivitaMapper attivitaMapper,
            IOperatoreMapper operatoreMapper,
            IAttivitaService attivitaService,
            IForzaturaService forzaturaService,
            IOperatoreService operatoreService,
            IDialogoOperatoreObserver dialogoOperatoreObserver)
        {
            _attivitaMapper = attivitaMapper;
            _operatoreMapper = operatoreMapper;
            _attivitaService = attivitaService;
            _forzaturaService = forzaturaService;
            _operatoreService = operatoreService;
            _dialogoOperatoreObserver = dialogoOperatoreObserver;
        }

        public async Task<string?> ApriFaseNonPianificata(IAttivitaViewModel attivita)
        {
            string? result = null;

            switch (_dialogoOperatoreObserver.OperazioneInCorso)
            {
                case Costanti.INIZIO_ATTREZZAGGIO:
                    result = _attivitaService.ApriAttrezzaggioFaseNonPianificata(_attivitaMapper.AttivitaViewModelToAttivita(_dialogoOperatoreObserver.AttivitaSelezionata),
                                                                                 _operatoreMapper.OperatoreViewModelToOperatore(_dialogoOperatoreObserver.OperatoreSelezionato));
                    break;
                case Costanti.INIZIO_LAVORO:
                    result = _attivitaService.ApriLavoroFaseNonPianificata(_attivitaMapper.AttivitaViewModelToAttivita(_dialogoOperatoreObserver.AttivitaSelezionata),
                                                                           _operatoreMapper.OperatoreViewModelToOperatore(_dialogoOperatoreObserver.OperatoreSelezionato));
                    break;
                default:
                    break;
            }

            if (Int32.TryParse(result, out int evtUid))
                await _forzaturaService.ForzaRigaOrdineDaIdEvento(evtUid);

            _dialogoOperatoreObserver.OperazioneInCorso = Costanti.NESSUNA;
            _dialogoOperatoreObserver.AttivitaSelezionata = null;
            AggiornaOperatoreSelezionato();

            return result;
        }

        private void AggiornaOperatoreSelezionato()
        {
            Operatore? operatore = _operatoreService.OttieniOperatore(_dialogoOperatoreObserver.OperatoreSelezionato.Badge);

            _dialogoOperatoreObserver.OperatoreSelezionato = operatore != null ? new OperatoreViewModel(operatore) : null;
        }
    }
}
