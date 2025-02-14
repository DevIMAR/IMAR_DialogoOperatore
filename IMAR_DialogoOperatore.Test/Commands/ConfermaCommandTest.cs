using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Application.Interfaces.Services.Activities;
using IMAR_DialogoOperatore.Commands;
using IMAR_DialogoOperatore.Domain.Models;
using IMAR_DialogoOperatore.Interfaces.Helpers;
using IMAR_DialogoOperatore.Observers;
using IMAR_DialogoOperatore.ViewModels;
using NSubstitute;

namespace IMAR_DialogoOperatore.Test.Commands
{
    public class ConfermaCommandTest
	{
		private ConfermaCommand _command;
		private IPopupConfermaHelper _popupConfermaUtility;
		private IConfermaOperazioneHelper _confermaOperazioneUtility;
		private DialogoOperatoreObserver _dialogoOperatoreStore;
		private PopupObserver _popupStore;

		// Mock delle dipendenze richieste dai costruttori di PopupConfermaUtility e ConfermaOperazioneUtility
		private IAttivitaService _attivitaService;
		private AvanzamentoObserver _avanzamentoStore;
		private CercaAttivitaObserver _cercaAttivitaStore;

		public ConfermaCommandTest()
		{
			// Creazione dei mock per le dipendenze
			_dialogoOperatoreStore = Substitute.For<DialogoOperatoreObserver>();
			Operatore operatoreMock = Substitute.For<Operatore>();
			Attivita attivitaMock = Substitute.For<Attivita>();
			_avanzamentoStore = Substitute.For<AvanzamentoObserver>();
			_cercaAttivitaStore = Substitute.For<CercaAttivitaObserver>();
			_popupStore = Substitute.For<PopupObserver>();

			_dialogoOperatoreStore.OperatoreSelezionato = new OperatoreViewModel(operatoreMock);
			_dialogoOperatoreStore.AttivitaSelezionata = new AttivitaViewModel(attivitaMock);
			_attivitaService = Substitute.For<IAttivitaService>();
			_confermaOperazioneUtility = Substitute.For<IConfermaOperazioneHelper>();
			_popupConfermaUtility = Substitute.For<IPopupConfermaHelper>();

			// Inizializzazione di ConfermaCommand con le utility e il popup store
			_command = new ConfermaCommand(_popupConfermaUtility, _confermaOperazioneUtility, _dialogoOperatoreStore, _popupStore);
		}

		[Fact]
		public void CanExecute_ReturnsTrue_WhenOperatoreSelezionatoIsValid()
		{
			// Arrange
			_dialogoOperatoreStore.OperatoreSelezionato.Stato = Costanti.PRESENTE;
			_dialogoOperatoreStore.OperazioneInCorso = Costanti.INIZIO_LAVORO;

			// Act
			var canExecute = _command.CanExecute(null);

			// Assert
			Assert.True(canExecute);
		}

		[Fact]
		public void CanExecute_ReturnsFalse_WhenOperatoreSelezionatoIsNull()
		{
			// Arrange
			_dialogoOperatoreStore.OperatoreSelezionato = null;

			// Act
			var canExecute = _command.CanExecute(null);

			// Assert
			Assert.False(canExecute);
		}

		[Fact]
		public void CanExecute_ReturnsFalse_WhenOperatoreStatoIsAssente()
		{
			// Arrange
			_dialogoOperatoreStore.OperatoreSelezionato.Stato = Costanti.ASSENTE;

			// Act
			var canExecute = _command.CanExecute(null);

			// Assert
			Assert.False(canExecute);
		}

		[Fact]
		public void CanExecute_ReturnsFalse_WhenOperazioneInCorsoIsNessuna()
		{
			// Arrange
			_dialogoOperatoreStore.OperatoreSelezionato.Stato = Costanti.PRESENTE;
			_dialogoOperatoreStore.OperazioneInCorso = Costanti.NESSUNA;

			// Act
			var canExecute = _command.CanExecute(null);

			// Assert
			Assert.False(canExecute);
		}

		[Fact]
		public void CanExecute_ReturnsFalse_WhenAttivitaSelezionataIsNull()
		{
			// Arrange
			_dialogoOperatoreStore.AttivitaSelezionata = null;

			// Act
			var canExecute = _command.CanExecute(null);

			// Assert
			Assert.False(canExecute);
		}

		[Fact]
		public void Execute_CallsEseguiOperazione_WhenTestoPopupIsEmpty()
		{
			// Arrange
			_popupConfermaUtility.GetTestoPopup().Returns(string.Empty);

			// Act
			_command.Execute(null);

			// Assert
			_confermaOperazioneUtility.Received(1).EseguiOperazione();
		}

		[Fact]
		public void Execute_ShowsPopup_WhenTestoPopupIsNotEmpty()
		{
			// Arrange
			var testoPopup = "Conferma l'operazione";
			_popupConfermaUtility.GetTestoPopup().Returns(testoPopup);

			// Act
			_command.Execute(null);

			// Assert
			Assert.Equal(testoPopup, _popupStore.TestoPopup);
			Assert.True(_popupStore.IsPopupVisible);
		}

		[Fact]
		public void PopupStore_OnIsConfermatoChanged_DoesNotCallEseguiOperazione_WhenIsConfermatoIsFalse()
		{
			// Arrange
			_popupStore.IsConfermato = false;

			// Act

			// Assert
			_confermaOperazioneUtility.DidNotReceive().EseguiOperazione();
		}

		[Fact]
		public void PopupStore_OnIsConfermatoChanged_CallsEseguiOperazione_WhenIsConfermatoIsTrue()
		{
			// Arrange
			_popupStore.IsConfermato = true;

			// Act

			// Assert
			_confermaOperazioneUtility.Received(1).EseguiOperazione();
		}
	}

}
