using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Commands;
using IMAR_DialogoOperatore.Domain.Models;
using IMAR_DialogoOperatore.Interfaces.Helpers;
using IMAR_DialogoOperatore.Observers;
using IMAR_DialogoOperatore.ViewModels;
using NSubstitute;
using System.Collections.ObjectModel;

namespace IMAR_DialogoOperatore.Test.Commands
{
	public class IngressoUscitaCommandTests
	{
		private IngressoUscitaCommand _command;
		private DialogoOperatoreObserver _dialogoOperatoreStore;
		private IInterruzioneAttivitaHelper _interruzioneLavoroUtility;

		public IngressoUscitaCommandTests()
		{
			_dialogoOperatoreStore = Substitute.For<DialogoOperatoreObserver>();
			_interruzioneLavoroUtility = Substitute.For<IInterruzioneAttivitaHelper>();
			Operatore operatoreMock = Substitute.For<Operatore>();
			_dialogoOperatoreStore.OperatoreSelezionato = new OperatoreViewModel(operatoreMock);
			_dialogoOperatoreStore.OperatoreSelezionato.AttivitaAperte = new ObservableCollection<Attivita>();
			_dialogoOperatoreStore.IsUscita = false;

			_command = new IngressoUscitaCommand(_dialogoOperatoreStore, _interruzioneLavoroUtility);
		}

		[Fact]
		public void CanExecute_ReturnsTrue_WhenOperatoreSelezionatoIsValid_AndNotInPausa()
		{
			//Arrange
			_dialogoOperatoreStore.OperatoreSelezionato.Stato = Costanti.PRESENTE;

			//Act
			var result = _command.CanExecute(null);

			//Assert
			Assert.True(result);
		}

		[Fact]
		public void CanExecute_ReturnsFalse_WhenOperatoreSelezionatoIsNull()
		{
			//Arrange
			_dialogoOperatoreStore.OperatoreSelezionato = null;

			//Act
			var result = _command.CanExecute(null);

			//Assert
			Assert.False(result);
		}

		[Fact]
		public void CanExecute_ReturnsFalse_WhenAreTastiBloccatiIsTrue()
		{
			//Arrange
			_dialogoOperatoreStore.OperatoreSelezionato.Stato = Costanti.PRESENTE;
			_dialogoOperatoreStore.IsUscita = true;

			//Act
			var result = _command.CanExecute(null);

			//Assert
			Assert.False(result);
		}

		[Fact]
		public void CanExecute_ReturnsFalse_WhenOperatoreSelezionatoIsInPausa()
		{
			//Arrange
			_dialogoOperatoreStore.OperatoreSelezionato.Stato = Costanti.IN_PAUSA;

			//Act
			var result = _command.CanExecute(null);

			//Assert
			Assert.False(result);
		}

		[Fact]
		public async Task Execute_SetsOperatoreStatoToPresente_WhenStatoIsAssente()
		{
			//Arrange
			_dialogoOperatoreStore.OperatoreSelezionato.Stato = Costanti.ASSENTE;

			//Act
			_command.Execute(null);

			//Assert
			Assert.Equal(Costanti.PRESENTE, _dialogoOperatoreStore.OperatoreSelezionato.Stato);
		}

		[Fact]
		public async Task Execute_SetsOperatoreToNull_WhenOperazioneIsNotAnnullata()
		{
			//Arrange
			_dialogoOperatoreStore.OperatoreSelezionato.Stato = Costanti.PRESENTE;
			_dialogoOperatoreStore.IsOperazioneAnnullata = false;

			//Act
			_command.Execute(null);

			//Assert
			Assert.Null(_dialogoOperatoreStore.OperatoreSelezionato);
		}

		[Fact]
		public async Task Execute_DoesNotSetOperatoreToNull_WhenOperazioneIsAnnullata()
		{
			//Arrange
			_dialogoOperatoreStore.OperatoreSelezionato.Stato = Costanti.PRESENTE;
			_dialogoOperatoreStore.IsOperazioneAnnullata = true;

			//Act
			_command.Execute(null);

			//Assert
			Assert.NotNull(_dialogoOperatoreStore.OperatoreSelezionato);
		}
	}

}
