using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Commands;
using IMAR_DialogoOperatore.Domain.Models;
using IMAR_DialogoOperatore.Observers;
using IMAR_DialogoOperatore.ViewModels;
using NSubstitute;

namespace IMAR_DialogoOperatore.Test.Commands
{
	public class InizioAttrezzaggioCommandTest
	{
		private DialogoOperatoreObserver _dialogoOperatoreStore;
		private InizioAttrezzaggioCommand _command;

		public InizioAttrezzaggioCommandTest()
		{
			_dialogoOperatoreStore = Substitute.For<DialogoOperatoreObserver>();
			Operatore operatoreMock = Substitute.For<Operatore>();
			_dialogoOperatoreStore.OperazioneInCorso = Costanti.NESSUNA;
			_dialogoOperatoreStore.IsUscita = false;

			_dialogoOperatoreStore.OperatoreSelezionato = new OperatoreViewModel(operatoreMock);

			_command = new InizioAttrezzaggioCommand(_dialogoOperatoreStore);
		}

		[Fact]
		public void CanExecute_ReturnsTrue()
		{
			//Arrange
			_dialogoOperatoreStore.OperatoreSelezionato.Stato = Costanti.PRESENTE;

			//Act
			bool result = _command.CanExecute(null);

			//Assert
			Assert.True(result);
		}

		[Fact]
		public void CanExecute_ReturnsFalse_WhenOperatoreSelezionatoIsNull()
		{
			//Arrange
			_dialogoOperatoreStore.OperatoreSelezionato = null;

			//Act
			bool result = _command.CanExecute(null);

			//Assert
			Assert.False(result);
		}

		[Theory]
		[InlineData(Costanti.ASSENTE)]
		[InlineData(Costanti.IN_PAUSA)]
		public void CanExecute_ReturnsFalse_WhenOperatoreSelezionatoIsNotPresente(string statoOperatore)
		{
			//Arrange
			_dialogoOperatoreStore.OperatoreSelezionato.Stato = statoOperatore;

			//Act
			bool result = _command.CanExecute(null);

			//Assert
			Assert.False(result);
		}

		[Fact]
		public void CanExecute_ReturnsFalse_WhenTastiBloccati()
		{
			//Arrange
			_dialogoOperatoreStore.IsUscita = true;

			//Act
			bool result = _command.CanExecute(null);

			//Assert
			Assert.False(result);
		}

		[Fact]
		public void Execute_SetOperazioneInCorsoToInizioAttrezzaggio_AndAttivitaSelezionataToNull()
		{
			//Arrange

			//Act
			_command.Execute(null);

			//Assert
			Assert.Null(_dialogoOperatoreStore.AttivitaSelezionata);
			Assert.Equal(_dialogoOperatoreStore.OperazioneInCorso, Costanti.INIZIO_ATTREZZAGGIO);
		}

		[Theory]
		[InlineData(Costanti.AVANZAMENTO)]
		[InlineData(Costanti.INIZIO_LAVORO)]
		public void Execute_NotSetAttivitaSelezionataToNull(string operazioneInCorso)
		{
			//Arrange
			_dialogoOperatoreStore.OperazioneInCorso = operazioneInCorso;
			_dialogoOperatoreStore.AttivitaSelezionata = new AttivitaViewModel(new Attivita());

			//Act
			_command.Execute(null);

			//Assert
			Assert.NotNull(_dialogoOperatoreStore.AttivitaSelezionata);
		}
	}
}
