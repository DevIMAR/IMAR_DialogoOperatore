using IMAR_DialogoOperatore.Commands;
using IMAR_DialogoOperatore;
using NSubstitute;
using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Observers;

namespace IMAR_DialogoOperatore.Test.Commands
{
	public class InizioLavoroCommandTest
	{
		[Fact]
		public void InizioLavoroCommand_Avanzamento_Execute_ReturnsString()
		{
			//Arrange
			DialogoOperatoreObserver dialogoOperatoreStoreMock = Substitute.For<DialogoOperatoreObserver>();
			InizioLavoroCommand sut = new InizioLavoroCommand(dialogoOperatoreStoreMock);

			dialogoOperatoreStoreMock.OperazioneInCorso = Costanti.AVANZAMENTO;

			//Act
			sut.Execute(null);

			//Assertr
			Assert.Null(dialogoOperatoreStoreMock.AttivitaSelezionata);
			Assert.Equal(Costanti.INIZIO_LAVORO, dialogoOperatoreStoreMock.OperazioneInCorso);
		}

		[Fact]
		public void InizioLavoroCommand_InizioAttrezzaggio_Execute_ReturnsString()
		{
			//Arrange
			DialogoOperatoreObserver dialogoOperatoreStoreMock = Substitute.For<DialogoOperatoreObserver>();
			InizioLavoroCommand sut = new InizioLavoroCommand(dialogoOperatoreStoreMock);

			dialogoOperatoreStoreMock.OperazioneInCorso = Costanti.INIZIO_ATTREZZAGGIO;

			//Act
			sut.Execute(null);

			//Assertr
			Assert.Null(dialogoOperatoreStoreMock.AttivitaSelezionata);
			Assert.Equal(Costanti.INIZIO_LAVORO, dialogoOperatoreStoreMock.OperazioneInCorso);
		}

		[Fact]
		public void InizioLavoroCommand_Execute_ReturnsString()
		{
			//Arrange
			DialogoOperatoreObserver dialogoOperatoreStoreMock = Substitute.For<DialogoOperatoreObserver>();
			InizioLavoroCommand sut = new InizioLavoroCommand(dialogoOperatoreStoreMock);

			//Act
			sut.Execute(null);

			//Assertr
			Assert.Equal(Costanti.INIZIO_LAVORO, dialogoOperatoreStoreMock.OperazioneInCorso);
		}
	}
}
