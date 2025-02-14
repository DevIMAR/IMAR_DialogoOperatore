using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Application.Interfaces.Services.Activities;
using IMAR_DialogoOperatore.Domain.Models;
using IMAR_DialogoOperatore.Helpers;
using IMAR_DialogoOperatore.Interfaces.Helpers;
using IMAR_DialogoOperatore.Interfaces.Observers;
using IMAR_DialogoOperatore.Mappers;
using IMAR_DialogoOperatore.ViewModels;
using NSubstitute;

namespace IMAR_DialogoOperatore.Test.Utilities
{
    public class PopupConfermaUtilityTest
	{
		private IPopupConfermaHelper _popupConfermaHelper;
		private IAttivitaService _attivitaService;
		private IDialogoOperatoreObserver _dialogoOperatoreObserver;
		private IAvanzamentoObserver _avanzamentoObserver;
		private ICercaAttivitaObserver _cercaAttivitaObserver;
		private AttivitaMapper _attivitaMapper;

		public PopupConfermaUtilityTest()
		{
			_attivitaService = Substitute.For<IAttivitaService>();

			_dialogoOperatoreObserver = Substitute.For<IDialogoOperatoreObserver>();
			_avanzamentoObserver = Substitute.For<IAvanzamentoObserver>();
			_cercaAttivitaObserver = Substitute.For<ICercaAttivitaObserver>();

			_attivitaMapper = Substitute.For<AttivitaMapper>();

			_popupConfermaHelper = new PopupConfermaHelper(
				_attivitaService,
				_dialogoOperatoreObserver,
				_cercaAttivitaObserver,
				_avanzamentoObserver
			);
		}

		[Fact]
		public void GetTestoPopup_RestituisceVuoto_WhenOperazioneInCorsoNulla()
		{
			// Arrange
			_dialogoOperatoreObserver.OperazioneInCorso = null;

			// Act
			var result = _popupConfermaHelper.GetTestoPopup();

			// Assert
			Assert.Empty(result);
		}

		[Fact]
		public void GetTestoPopup_GestisciInizioLavoro_RestituisceMessaggioCorretto()
		{
			// Arrange
			_dialogoOperatoreObserver.OperazioneInCorso = Costanti.INIZIO_LAVORO;
			_dialogoOperatoreObserver.AttivitaSelezionata = new AttivitaViewModel(new Attivita { Fase = "Fase1", QuantitaProdotta = 0, Causale = Costanti.IN_LAVORO });
			_cercaAttivitaObserver.FaseCercata = "Fase2";

			var attivitaPrecedente = new AttivitaViewModel(new Attivita { Odp = "ODP123", Fase = "Fase1", QuantitaProdotta = 8 });
			_attivitaService.Attivita.Returns(_attivitaMapper.ListaAttivitaViewModelToListaAttivita([attivitaPrecedente, _dialogoOperatoreObserver.AttivitaSelezionata]));

			// Act
			var result = _popupConfermaHelper.GetTestoPopup();

			// Assert
			Assert.Contains("La fase cercata inizialmente era la Fase2", result);
			Assert.Contains("Sei sicuro di voler iniziare questo lavoro?", result);
		}

		[Fact]
		public void GetTestoPopup_GestisciAvanzamentoOChiusuraLavoro_RestituisceMessaggioCorretto()
		{
			// Arrange
			_dialogoOperatoreObserver.OperazioneInCorso = Costanti.AVANZAMENTO;
			_dialogoOperatoreObserver.AttivitaSelezionata = new AttivitaViewModel(new Attivita { Fase = "Fase1", QuantitaProdotta = 5, QuantitaOrdine = 20, SaldoAcconto = Costanti.SALDO });
			_avanzamentoObserver.QuantitaProdotta = 3;

			var attivitaPrecedente = new AttivitaViewModel (new Attivita { Odp = "ODP123", Fase = "Fase1", QuantitaProdotta = 8 });
			_attivitaService.Attivita.Returns(_attivitaMapper.ListaAttivitaViewModelToListaAttivita([attivitaPrecedente, _dialogoOperatoreObserver.AttivitaSelezionata]));

			// Act
			var result = _popupConfermaHelper.GetTestoPopup();

			// Assert
			Assert.Contains("La fase è già stata chiusa a saldo.", result);
			Assert.Contains("Stai dichiarando 3 pezzi.", result);
			Assert.Contains("La quantità totale prodotta per questa fase è 8/20.", result);
			Assert.Contains("Sei sicuro di voler continuare?", result);
		}

		[Fact]
		public void GetTestoPopup_RestituisceMessaggio_WhenFasePrecedenteConQuantitaProdottaZero()
		{
			// Arrange
			var attivitaSelezionata = new AttivitaViewModel (new Attivita { Odp = "ODP123", Fase = "Fase2", QuantitaProdotta = 0 });
			_dialogoOperatoreObserver.OperazioneInCorso = Costanti.INIZIO_LAVORO;
			_dialogoOperatoreObserver.AttivitaSelezionata = attivitaSelezionata;

			// Mock attivita con fase precedente a quantità prodotta zero
			var attivitaPrecedente = new AttivitaViewModel (new Attivita { Odp = "ODP123", Fase = "Fase1", QuantitaProdotta = 0 });
			_attivitaService.Attivita.Returns(_attivitaMapper.ListaAttivitaViewModelToListaAttivita([attivitaPrecedente, attivitaSelezionata]));

			// Act
			var result = _popupConfermaHelper.GetTestoPopup();

			// Assert
			Assert.Contains("La fase precedente a quella in lavorazione ha prodotto 0 pezzi.", result);
		}

		[Fact]
		public void GetTestoPopup_RestituisceMessaggio_WhenFasePrecedenteQuantitaMinoreDiAttuale()
		{
			// Arrange
			_dialogoOperatoreObserver.OperazioneInCorso = Costanti.AVANZAMENTO;
			_dialogoOperatoreObserver.AttivitaSelezionata = new AttivitaViewModel (new Attivita { Odp = "ODP123", Fase = "Fase2", QuantitaProdotta = 10 });
			_avanzamentoObserver.QuantitaProdotta = 5;

			// Mock attivita con fase precedente a quantità prodotta minore dell'attuale
			var attivitaPrecedente = new AttivitaViewModel (new Attivita { Odp = "ODP123", Fase = "Fase1", QuantitaProdotta = 8 });
			_attivitaService.Attivita.Returns(_attivitaMapper.ListaAttivitaViewModelToListaAttivita([attivitaPrecedente, _dialogoOperatoreObserver.AttivitaSelezionata]));

			// Act
			var result = _popupConfermaHelper.GetTestoPopup();

			// Assert
			Assert.Contains("La fase precedente a quella in lavorazione ha prodotto 8 pezzi.", result);
		}
	}

}
