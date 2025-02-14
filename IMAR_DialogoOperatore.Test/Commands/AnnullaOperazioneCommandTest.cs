using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Application.Interfaces.ViewModels;
using IMAR_DialogoOperatore.Commands;
using IMAR_DialogoOperatore.Domain.Models;
using IMAR_DialogoOperatore.Interfaces.Mappers;
using IMAR_DialogoOperatore.Observers;
using IMAR_DialogoOperatore.ViewModels;
using NSubstitute;
using System.Collections.ObjectModel;

namespace IMAR_DialogoOperatore.Test.Commands
{
	public class AnnullaOperazioneCommandTests
	{
		private AnnullaOperazioneCommand _command;
		private DialogoOperatoreObserver _dialogoOperatoreStore;
		private IAttivitaMapper _attivitaMapper;

		public AnnullaOperazioneCommandTests()
		{
			_dialogoOperatoreStore = Substitute.For<DialogoOperatoreObserver>();
			Operatore operatoreMock = Substitute.For<Operatore>();
			_dialogoOperatoreStore.OperatoreSelezionato = new OperatoreViewModel(operatoreMock);
			_attivitaMapper = Substitute.For<IAttivitaMapper>();

			_command = new AnnullaOperazioneCommand(_dialogoOperatoreStore, _attivitaMapper);
		}

		[Fact]
		public void CanExecute_ReturnsTrue_WhenOperatoreSelezionatoIsNotNull()
		{
			// Arrange

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
		public void Execute_SetsOperatoreSelezionatoToNull_WhenCondizioneDiLogoutIsTrue()
		{
			// Arrange
			_dialogoOperatoreStore.OperazioneInCorso = Costanti.NESSUNA;
			_dialogoOperatoreStore.ListaAttivita = (ObservableCollection<IAttivitaViewModel>)_attivitaMapper.ListaAttivitaToListaAttivitaViewModel(_dialogoOperatoreStore.OperatoreSelezionato.AttivitaAperte);
			_dialogoOperatoreStore.AttivitaSelezionata = null;

			// Act
			_command.Execute(null);

			// Assert
			Assert.Null(_dialogoOperatoreStore.OperatoreSelezionato);
		}

		[Fact]
		public void Execute_ResetsIsOperazioneAnnullata_WhenCondizioneDiLogoutIsTrue()
		{
			// Arrange
			_dialogoOperatoreStore.OperazioneInCorso = Costanti.NESSUNA;
			_dialogoOperatoreStore.ListaAttivita = (ObservableCollection<IAttivitaViewModel>)_attivitaMapper.ListaAttivitaToListaAttivitaViewModel(_dialogoOperatoreStore.OperatoreSelezionato.AttivitaAperte);
			_dialogoOperatoreStore.AttivitaSelezionata = null;

			// Act
			_command.Execute(null);

			// Assert
			Assert.False(_dialogoOperatoreStore.IsOperazioneAnnullata);
		}

		[Fact]
		public void Execute_SetsOperazioneInCorsoToNESSUNA_AndUpdatesListaAttivita_WhenAttivitaDetailsDaChiudereIsTrue()
		{
			// Arrange
			_dialogoOperatoreStore.OperazioneInCorso = Costanti.INIZIO_LAVORO;

			var attivitaAperte = new ObservableCollection<Attivita>();
			_dialogoOperatoreStore.OperatoreSelezionato.AttivitaAperte = attivitaAperte;

			// Act
			_command.Execute(null);

			// Assert
			Assert.Equal(Costanti.NESSUNA, _dialogoOperatoreStore.OperazioneInCorso);
			Assert.Equal(attivitaAperte, _attivitaMapper.ListaAttivitaViewModelToListaAttivita(_dialogoOperatoreStore.ListaAttivita));
		}

		[Fact]
		public void Execute_SetsAttivitaSelezionataToNull_AndResetsIsOperazioneAnnullata_AfterExecution()
		{
			//Arrange
			_dialogoOperatoreStore.OperazioneInCorso = Costanti.IN_LAVORO;

			// Act
			_command.Execute(null);

			// Assert
			Assert.Null(_dialogoOperatoreStore.AttivitaSelezionata);
			Assert.False(_dialogoOperatoreStore.IsOperazioneAnnullata);
		}
	}

}
