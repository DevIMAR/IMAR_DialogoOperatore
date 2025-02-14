using IMAR_DialogoOperatore.Commands;
using IMAR_DialogoOperatore.Observers;
using NSubstitute;

namespace IMAR_DialogoOperatore.Test.Commands
{
	public class RispostaPopupDiConfermaCommandTest
    {
        [Theory]
        [InlineData(true, true)]
        [InlineData(false, false)]
        public void RispostaPopupDiConfermaCommand_Execute_ReturnsVoid(bool expected, object? parameter)
        {
            //Arrange
            PopupObserver popupStoreMock = Substitute.For<PopupObserver>();
			RispostaPopupDiConfermaCommand rispostaPopupDiConfermaCommand = new RispostaPopupDiConfermaCommand(popupStoreMock);

			//Act
			rispostaPopupDiConfermaCommand.Execute(parameter);

			//Assert
			Assert.Equal(expected, popupStoreMock.IsConfermato);
			Assert.False(popupStoreMock.IsPopupVisible);
		}

	}
}
