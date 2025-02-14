using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore;
using IMAR_DialogoOperatore.Commands;
using IMAR_DialogoOperatore.Observers;
using NSubstitute;

namespace IMAR_DialogoOperatore.Test.Commands
{
	public class AvanzamentoCommandTest
    {
        [Fact]
        public void AvanzamentoCommand_Execute_ReturnsString()
        {
            //Arrange
            DialogoOperatoreObserver dialogoOperatoreStoreMock = Substitute.For<DialogoOperatoreObserver>();
            AvanzamentoCommand sut = new AvanzamentoCommand(dialogoOperatoreStoreMock);

            //Act
            sut.Execute(null);

            //Assert
            Assert.Equal(Costanti.AVANZAMENTO, dialogoOperatoreStoreMock.OperazioneInCorso);
        }
    }
}