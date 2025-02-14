using IMAR_DialogoOperatore.Commands;
using IMAR_DialogoOperatore;
using NSubstitute;
using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Observers;

namespace IMAR_DialogoOperatore.Test.Commands
{
	public class FineLavoroCommandTest
	{
		[Fact]
		public void FineLavoroCommand_Execute_ReturnsString()
		{
			//Arrange
			DialogoOperatoreObserver dialogoOperatoreStoreMock = Substitute.For<DialogoOperatoreObserver>();
			FineLavoroCommand sut = new FineLavoroCommand(dialogoOperatoreStoreMock);

			//Act
			sut.Execute(null);

			//Assertr
			Assert.Equal(Costanti.FINE_LAVORO, dialogoOperatoreStoreMock.OperazioneInCorso);
		}
	}
}
