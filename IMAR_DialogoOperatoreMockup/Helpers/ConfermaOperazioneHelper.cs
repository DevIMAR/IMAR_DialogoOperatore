using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Application.Interfaces.Services.Activities;
using IMAR_DialogoOperatore.Domain.Models;
using IMAR_DialogoOperatore.Interfaces.Helpers;
using IMAR_DialogoOperatore.Interfaces.Mappers;
using IMAR_DialogoOperatore.Interfaces.Observers;
using IMAR_DialogoOperatore.Observers;
using IMAR_DialogoOperatore.ViewModels;

namespace IMAR_DialogoOperatore.Helpers
{
    public class ConfermaOperazioneHelper : IConfermaOperazioneHelper
	{
		private readonly AttivitaGridViewModel _attivitaGridViewModel;
		private readonly IOperatoreService _operatoriService;
		private readonly IAttivitaService _attivitaService;
		private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;
		private readonly IAvanzamentoObserver _avanzamentoObserver;
		private readonly IAttivitaIndirettaObserver _attivitaIndirettaObserver;
		private readonly IOperatoreMapper _operatoreMapper;
		private readonly IAttivitaMapper _attivitaMapper;

		public ConfermaOperazioneHelper(
			AttivitaGridViewModel attivitaGridViewModel,
			IOperatoreService operatoriService,
			IAttivitaService attivitaService,
			IDialogoOperatoreObserver dialogoOperatoreObserver,
			IAvanzamentoObserver avanzamentoObserver,
			IAttivitaIndirettaObserver attivitaIndirettaObserver,
			IOperatoreMapper operatoreMapper,
			IAttivitaMapper AttivitaMapper)
		{						
			_attivitaGridViewModel = attivitaGridViewModel;
			_operatoriService = operatoriService;
			_attivitaService = attivitaService;
			_dialogoOperatoreObserver = dialogoOperatoreObserver;
			_avanzamentoObserver = avanzamentoObserver;
			_attivitaIndirettaObserver = attivitaIndirettaObserver;
			_operatoreMapper = operatoreMapper;
			_attivitaMapper = AttivitaMapper;
		}

		public string? EseguiOperazione()
		{
			string? result = null;

			string? operazioneInCorso = _dialogoOperatoreObserver.OperazioneInCorso;
			if (operazioneInCorso == null)
				return null;

			switch (operazioneInCorso)
			{
				case Costanti.INIZIO_LAVORO:
                    result = AggiungiAttivitaAdOperatore(false);
                    AggiornaOperatoreSelezionato();
                    break;

				case Costanti.INIZIO_ATTREZZAGGIO:
					result = AggiungiAttivitaAdOperatore(true);
                    AggiornaOperatoreSelezionato();
                    break;

				case Costanti.AVANZAMENTO:
					result = AggiornaAttivitaAvanzata();
					break;

				case Costanti.FINE_LAVORO:
                    result = RimuoviAttivitaDaOperatore();
                    AggiornaOperatoreSelezionato();
                    break;

				case Costanti.FINE_ATTREZZAGGIO:
					result = GestisciFineAttrezzaggio();
                    AggiornaOperatoreSelezionato();
                    break;

				default:
					break;
            }
            _attivitaGridViewModel.AttivitaSelezionata = null;

            return result;
        }

        private string? GestisciFineAttrezzaggio()
        {
			string? result = null;

			if (_dialogoOperatoreObserver.IsAperturaLavoroAutomaticaAttiva)
			{
				_attivitaIndirettaObserver.IsAttivitaIndiretta = false;
                result = AggiungiAttivitaAdOperatore(false);
			}
			else
				result = RimuoviAttivitaDaOperatore();

			return result;
        }

        private string? RimuoviAttivitaDaOperatore()
		{
            string? result = _operatoriService.RimuoviAttivitaDaOperatore(
                _operatoreMapper.OperatoreViewModelToOperatore(_dialogoOperatoreObserver.OperatoreSelezionato),
				_attivitaMapper.AttivitaViewModelToAttivita(_dialogoOperatoreObserver.AttivitaSelezionata),
                _avanzamentoObserver.QuantitaProdotta != null ? (int)_avanzamentoObserver.QuantitaProdotta : 0,
                _avanzamentoObserver.QuantitaScartata != null ? (int)_avanzamentoObserver.QuantitaScartata : 0,
                _dialogoOperatoreObserver.IsRiaperturaAttiva);

            AzzeraValoriAvanzamento();

            return result;
        }

		private string? AggiornaAttivitaAvanzata()
        {
			_dialogoOperatoreObserver.AttivitaSelezionata.QuantitaProdottaNonContabilizzata += _avanzamentoObserver.QuantitaProdotta != null ? (int)_avanzamentoObserver.QuantitaProdotta : 0;
			_dialogoOperatoreObserver.AttivitaSelezionata.QuantitaScartataNonContabilizzata += _avanzamentoObserver.QuantitaScartata != null ? (int)_avanzamentoObserver.QuantitaScartata : 0;

            string? result = _attivitaService.AvanzaAttivita(
                _operatoreMapper.OperatoreViewModelToOperatore(_dialogoOperatoreObserver.OperatoreSelezionato),
                _attivitaMapper.AttivitaViewModelToAttivita(_dialogoOperatoreObserver.AttivitaSelezionata),
                _avanzamentoObserver.QuantitaProdotta != null ? (int)_avanzamentoObserver.QuantitaProdotta : 0,
                _avanzamentoObserver.QuantitaScartata != null ? (int)_avanzamentoObserver.QuantitaScartata : 0);

            AzzeraValoriAvanzamento();

            return result;
        }

        private void AzzeraValoriAvanzamento()
        {
            _avanzamentoObserver.QuantitaProdotta = 0;
            _avanzamentoObserver.QuantitaScartata = 0;

            _dialogoOperatoreObserver.IsRiaperturaAttiva = false;
        }

        private string? AggiungiAttivitaAdOperatore(bool isAttrezzaggio)
        {
            _dialogoOperatoreObserver.AttivitaSelezionata.Causale = isAttrezzaggio ? Costanti.IN_ATTREZZAGGIO : Costanti.IN_LAVORO;

            string? result = _operatoriService.AggiungiAttivitaAdOperatore(
                isAttrezzaggio,
                _operatoreMapper.OperatoreViewModelToOperatore(_dialogoOperatoreObserver.OperatoreSelezionato),
                _attivitaMapper.AttivitaViewModelToAttivita(_dialogoOperatoreObserver.AttivitaSelezionata),
                _attivitaIndirettaObserver.IsAttivitaIndiretta
            );

			return result;
        }

        private void AggiornaOperatoreSelezionato()
        {
            Operatore? operatore = _operatoriService.OttieniOperatore(_dialogoOperatoreObserver.OperatoreSelezionato.Badge);

            _dialogoOperatoreObserver.OperatoreSelezionato = operatore != null ? new OperatoreViewModel(operatore) : null;
        }
    }
}
