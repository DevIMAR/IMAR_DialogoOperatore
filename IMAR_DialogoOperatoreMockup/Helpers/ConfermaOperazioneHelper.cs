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

		public async Task<string?> EseguiOperazioneAsync()
		{
			string? result = null;

			string? operazioneInCorso = _dialogoOperatoreObserver.OperazioneInCorso;
			if (operazioneInCorso == null)
				return null;

			switch (operazioneInCorso)
			{
				case Costanti.INIZIO_LAVORO:
                    result = await AggiungiAttivitaAdOperatoreAsync(false);
                    await AggiornaOperatoreSelezionatoAsync();
                    break;

				case Costanti.INIZIO_ATTREZZAGGIO:
					result = await AggiungiAttivitaAdOperatoreAsync(true);
                    await AggiornaOperatoreSelezionatoAsync();
                    break;

				case Costanti.AVANZAMENTO:
					result = await AggiornaAttivitaAvanzataAsync();
					break;

				case Costanti.FINE_LAVORO:
                    result = await RimuoviAttivitaDaOperatoreAsync();
                    await AggiornaOperatoreSelezionatoAsync();
                    break;

				case Costanti.FINE_ATTREZZAGGIO:
					result = await GestisciFineAttrezzaggioAsync();
                    await AggiornaOperatoreSelezionatoAsync();
                    break;

				default:
					break;
            }
            _attivitaGridViewModel.AttivitaSelezionata = null;

            return result;
        }

        private async Task<string?> GestisciFineAttrezzaggioAsync()
        {
			string? result = null;

			if (_dialogoOperatoreObserver.IsAperturaLavoroAutomaticaAttiva)
			{
				_attivitaIndirettaObserver.IsAttivitaIndiretta = false;
                result = await AggiungiAttivitaAdOperatoreAsync(false);
			}
			else
				result = await RimuoviAttivitaDaOperatoreAsync();

			return result;
        }

        private async Task<string?> RimuoviAttivitaDaOperatoreAsync()
		{
            string? result = await _operatoriService.RimuoviAttivitaDaOperatoreAsync(
                _operatoreMapper.OperatoreViewModelToOperatore(_dialogoOperatoreObserver.OperatoreSelezionato),
				_attivitaMapper.AttivitaViewModelToAttivita(_dialogoOperatoreObserver.AttivitaSelezionata),
                _avanzamentoObserver.QuantitaProdotta != null ? (int)_avanzamentoObserver.QuantitaProdotta : 0,
                _avanzamentoObserver.QuantitaScartata != null ? (int)_avanzamentoObserver.QuantitaScartata : 0,
                _dialogoOperatoreObserver.IsRiaperturaAttiva);

            AzzeraValoriAvanzamento();

            return result;
        }

		private async Task<string?> AggiornaAttivitaAvanzataAsync()
        {
			_dialogoOperatoreObserver.AttivitaSelezionata.QuantitaProdottaNonContabilizzata += _avanzamentoObserver.QuantitaProdotta != null ? (int)_avanzamentoObserver.QuantitaProdotta : 0;
			_dialogoOperatoreObserver.AttivitaSelezionata.QuantitaScartataNonContabilizzata += _avanzamentoObserver.QuantitaScartata != null ? (int)_avanzamentoObserver.QuantitaScartata : 0;

            string? result = await _attivitaService.AvanzaAttivitaAsync(
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

        private async Task<string?> AggiungiAttivitaAdOperatoreAsync(bool isAttrezzaggio)
        {
            _dialogoOperatoreObserver.AttivitaSelezionata.Causale = isAttrezzaggio ? Costanti.IN_ATTREZZAGGIO : Costanti.IN_LAVORO;

            string? result = await _operatoriService.AggiungiAttivitaAdOperatoreAsync(
                isAttrezzaggio,
                _operatoreMapper.OperatoreViewModelToOperatore(_dialogoOperatoreObserver.OperatoreSelezionato),
                _attivitaMapper.AttivitaViewModelToAttivita(_dialogoOperatoreObserver.AttivitaSelezionata),
                _attivitaIndirettaObserver.IsAttivitaIndiretta
            );

			return result;
        }

        private async Task AggiornaOperatoreSelezionatoAsync()
        {
            Operatore? operatore = await _operatoriService.OttieniOperatoreAsync(_dialogoOperatoreObserver.OperatoreSelezionato.Badge);

            _dialogoOperatoreObserver.OperatoreSelezionato = operatore != null ? new OperatoreViewModel(operatore) : null;
        }
    }
}
