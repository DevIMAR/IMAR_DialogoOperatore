using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore;
using IMAR_DialogoOperatore.Commands;
using IMAR_DialogoOperatore.Observers;
using NSubstitute;

namespace IMAR_DialogoOperatore.Test.Commands
{
	public class FineAttrezzaggioCommandTest
    {
        [Fact]
        public void FineAttrezzaggioCommand_Execute_ReturnsString()
        {
            //Arrange
            DialogoOperatoreObserver dialogoOperatoreStoreMock = Substitute.For<DialogoOperatoreObserver>();
            FineAttrezzaggioCommand sut = new FineAttrezzaggioCommand(dialogoOperatoreStoreMock);

            //Act
            sut.Execute(null);

            //Assertr
            Assert.Equal(Costanti.FINE_ATTREZZAGGIO, dialogoOperatoreStoreMock.OperazioneInCorso);
        }
    }
}
