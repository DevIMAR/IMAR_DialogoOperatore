using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Commands;
using IMAR_DialogoOperatore.Domain.Models;
using IMAR_DialogoOperatore.Interfaces.Helpers;
using IMAR_DialogoOperatore.Interfaces.Observers;
using IMAR_DialogoOperatore.Interfaces.ViewModels;
using IMAR_DialogoOperatore.Observers;
using NSubstitute;
using System.Collections.ObjectModel;

namespace IMAR_DialogoOperatore.Test.Commands
{
    public class InizioFinePausaCommandTest
	{
		private IDialogoOperatoreObserver _dialogoOperatoreStore;
		private IInterruzioneAttivitaHelper _interruzioneLavoroUtility;
		private IOperatoreViewModel _operatore;
		private InizioFinePausaCommand _command;

		public InizioFinePausaCommandTest()
		{
			_dialogoOperatoreStore = Substitute.For<DialogoOperatoreObserver>();
			_interruzioneLavoroUtility = Substitute.For<IInterruzioneAttivitaHelper>();
			_operatore = Substitute.For<IOperatoreViewModel>();
			_operatore.AttivitaAperte = new ObservableCollection<Attivita>();
			_dialogoOperatoreStore.OperatoreSelezionato = _operatore;

			_command = new InizioFinePausaCommand(_dialogoOperatoreStore, _interruzioneLavoroUtility);
		}

		[Fact]
		public void CanExecute_ReturnsTrue_WhenOperatoreIsValidAndNotInAssente()
		{
			//Arrange
			_dialogoOperatoreStore.IsUscita = false;
			_dialogoOperatoreStore.OperatoreSelezionato.Stato = Costanti.PRESENTE;

			//Act
			var result = _command.CanExecute(true);

			//Assert
			Assert.True(result);
		}

		[Fact]
		public void CanExecute_ReturnsFalse_WhenOperatoreSelezionatoIsNull()
		{
			//Arrange
			_dialogoOperatoreStore.OperatoreSelezionato = null;

			//Act
			var result = _command.CanExecute(true);

			//Assert
			Assert.False(result);
		}

		[Fact]
		public void CanExecute_ReturnsFalse_WhenAreTastiBloccatiIsTrue()
		{
			//Arrange
			_dialogoOperatoreStore.IsUscita = true;

			//Act
			var result = _command.CanExecute(true);

			//Assert
			Assert.False(result);
		}

		[Fact]
		public void CanExecute_ReturnsFalse_WhenParameterIsFalse()
		{
			//Arrange

			//Act
			var result = _command.CanExecute(false);

			//Assert
			Assert.False(result);
		}

		[Fact]
		public void CanExecute_ReturnsFalse_WhenParameterIsNotBool()
		{
			//Arrange

			//Act
			var result = _command.CanExecute("false");

			//Assert
			Assert.False(result);
		}

		[Fact]
		public async Task Execute_SetsOperatoreStatoToPresente_WhenStatoIsInPausa()
		{
			//Arrange
			_dialogoOperatoreStore.OperatoreSelezionato.Stato = Costanti.IN_PAUSA;

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

		[Fact]
		public async Task Execute_CallsGestisciInterruzioneAttivita_WhenAttivitaAperteExists()
		{
			//Arrange
			_dialogoOperatoreStore.OperatoreSelezionato.Stato = Costanti.PRESENTE;

			//Act
			_command.Execute(null);

			//Assert
			await _interruzioneLavoroUtility.ReceivedWithAnyArgs(1).GestisciInterruzioneAttivita(default, false);
		}
	}

}
