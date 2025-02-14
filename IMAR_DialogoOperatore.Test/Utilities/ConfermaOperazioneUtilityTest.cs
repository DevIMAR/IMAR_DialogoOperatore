using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Application.Interfaces.Services.Activities;
using IMAR_DialogoOperatore.Application.Interfaces.ViewModels;
using IMAR_DialogoOperatore.Domain.Models;
using IMAR_DialogoOperatore.Helpers;
using IMAR_DialogoOperatore.Infrastructure.Services;
using IMAR_DialogoOperatore.Interfaces.Helpers;
using IMAR_DialogoOperatore.Interfaces.Mappers;
using IMAR_DialogoOperatore.Interfaces.Observers;
using IMAR_DialogoOperatore.Mappers;
using IMAR_DialogoOperatore.ViewModels;
using NSubstitute;

namespace IMAR_DialogoOperatore.Test.Utilities
{
    public class ConfermaOperazioneUtilityTest
	{
		private IConfermaOperazioneHelper _confermaOperazioneHelper;
		private IDialogoOperatoreObserver _dialogoOperatoreObserver;
		private IAvanzamentoObserver _avanzamentoObserver;
		private IOperatoreViewModel _mockOperatore;
		private IAttivitaViewModel _mockAttivita;
		private IOperatoriService _operatoriService;
		private IAttivitaService _attivitaService;
		private IOperatoreMapper _operatoreMapper;
		private IAttivitaMapper _attivitaMapper;

		public ConfermaOperazioneUtilityTest()
		{
			// Mock delle dipendenze
			_dialogoOperatoreObserver = Substitute.For<IDialogoOperatoreObserver>();
			_avanzamentoObserver = Substitute.For<IAvanzamentoObserver>();
			_operatoriService = Substitute.For<IOperatoriService>();
			_attivitaService = Substitute.For<IAttivitaService>();
			_operatoreMapper = Substitute.For<IOperatoreMapper>();
			_attivitaMapper = Substitute.For<IAttivitaMapper>();

			// Mock dei dati
			_mockOperatore = Substitute.For<IOperatoreViewModel>();

			_mockAttivita = new AttivitaViewModel(new Attivita
			{
				QuantitaProdotta = 10,
				QuantitaScartata = 2,
				SaldoAcconto = "A"
			});

			_dialogoOperatoreObserver.OperatoreSelezionato = _mockOperatore;
			_dialogoOperatoreObserver.AttivitaSelezionata = _mockAttivita;

			// Inizializzazione della classe sotto test
			_confermaOperazioneHelper = new ConfermaOperazioneHelper(
				_operatoriService, 
				_attivitaService, 
				_dialogoOperatoreObserver, 
				_avanzamentoObserver, 
				_operatoreMapper, 
				_attivitaMapper);
		}

		[Fact]
		public void EseguiOperazione_InizioLavoro_AggiungeAttivitaAdOperatore()
		{
			// Arrange
			_dialogoOperatoreObserver.OperazioneInCorso = Costanti.INIZIO_LAVORO;

			// Act
			_confermaOperazioneHelper.EseguiOperazione();

			// Assert
			Assert.Equal(Costanti.IN_LAVORO, _mockAttivita.Causale);
			Assert.Contains(_attivitaMapper.AttivitaViewModelToAttivita(_mockAttivita), _mockOperatore.AttivitaAperte);
			Assert.Null(_dialogoOperatoreObserver.AttivitaSelezionata);
			Assert.Equal(Costanti.NESSUNA, _dialogoOperatoreObserver.OperazioneInCorso);
		}

		[Fact]
		public void EseguiOperazione_FineLavoro_RimuoveAttivitaDaOperatore()
		{
			// Arrange
			_dialogoOperatoreObserver.OperazioneInCorso = Costanti.FINE_LAVORO;
			_mockOperatore.AttivitaAperte.Add(_attivitaMapper.AttivitaViewModelToAttivita(_mockAttivita));

			// Act
			_confermaOperazioneHelper.EseguiOperazione();

			// Assert
			Assert.Empty(_mockOperatore.AttivitaAperte);
			Assert.Equal(string.Empty, _mockAttivita.Causale);
			Assert.Equal(Costanti.NESSUNA, _dialogoOperatoreObserver.OperazioneInCorso);
		}

		[Fact]
		public void EseguiOperazione_Avanzamento_AggiornaQuantitaProdottaEScartata()
		{
			// Arrange
			_dialogoOperatoreObserver.OperazioneInCorso = Costanti.AVANZAMENTO;
			_avanzamentoObserver.QuantitaProdotta = 5;
			_avanzamentoObserver.QuantitaScartata = 1;
			_avanzamentoObserver.SaldoAcconto = "A";

			// Act
			_confermaOperazioneHelper.EseguiOperazione();

			// Assert
			Assert.Equal(15, _mockAttivita.QuantitaProdotta);
			Assert.Equal(3, _mockAttivita.QuantitaScartata);
			Assert.Equal("A", _mockAttivita.SaldoAcconto);
			Assert.Equal(0, _avanzamentoObserver.QuantitaProdotta);
			Assert.Equal(0, _avanzamentoObserver.QuantitaScartata);
			Assert.Equal(Costanti.NESSUNA, _dialogoOperatoreObserver.OperazioneInCorso);
		}

		[Fact]
		public void EseguiOperazione_InizioAttrezzaggio_AggiungeAttivitaConCausaleAttrezzaggio()
		{
			// Arrange
			_dialogoOperatoreObserver.OperazioneInCorso = Costanti.INIZIO_ATTREZZAGGIO;

			// Act
			_confermaOperazioneHelper.EseguiOperazione();

			// Assert
			Assert.Equal(Costanti.IN_ATTREZZAGGIO, _mockAttivita.Causale);
			Assert.Contains(_attivitaMapper.AttivitaViewModelToAttivita(_mockAttivita), _mockOperatore.AttivitaAperte);
			Assert.Null(_dialogoOperatoreObserver.AttivitaSelezionata);
			Assert.Equal(Costanti.NESSUNA, _dialogoOperatoreObserver.OperazioneInCorso);
		}

		[Fact]
		public void EseguiOperazione_FineAttrezzaggio_RimuoveAttivitaDaOperatore()
		{
			// Arrange
			_dialogoOperatoreObserver.OperazioneInCorso = Costanti.FINE_ATTREZZAGGIO;
			_mockOperatore.AttivitaAperte.Add(_attivitaMapper.AttivitaViewModelToAttivita(_mockAttivita));
			_mockAttivita.Causale = Costanti.IN_ATTREZZAGGIO;

			// Act
			_confermaOperazioneHelper.EseguiOperazione();

			// Assert
			Assert.Empty(_mockOperatore.AttivitaAperte);
			Assert.Equal(string.Empty, _mockAttivita.Causale);
			Assert.Equal(Costanti.NESSUNA, _dialogoOperatoreObserver.OperazioneInCorso);
		}

		[Fact]
		public void EseguiOperazione_OperazioneInCorsoNulla_NonEsegueOperazione()
		{
			// Arrange
			_dialogoOperatoreObserver.OperazioneInCorso = null;

			// Act
			_confermaOperazioneHelper.EseguiOperazione();

			// Assert
			Assert.Equal(0, _mockOperatore.AttivitaAperte.Count);
			Assert.Null(_dialogoOperatoreObserver.OperazioneInCorso);
		}
	}

}
