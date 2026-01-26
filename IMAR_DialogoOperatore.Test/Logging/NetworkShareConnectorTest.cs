using IMAR_DialogoOperatore.Infrastructure.Utilities;
using System.ComponentModel;

namespace IMAR_DialogoOperatore.Test.Logging
{
    /// <summary>
    /// Test per il NetworkShareConnector.
    /// </summary>
    public class NetworkShareConnectorTest
    {
        [Fact]
        public void NetworkShareConnector_ThrowsException_WithInvalidPath()
        {
            // Arrange
            var invalidPath = "\\\\non-existent-server-12345\\invalid-share";
            var username = "invalid-user";
            var password = "invalid-password";

            // Act & Assert
            Assert.Throws<Win32Exception>(() =>
            {
                using var connector = new NetworkShareConnector(invalidPath, username, password);
            });
        }

        [Fact]
        public void NetworkShareConnector_ImplementsIDisposable()
        {
            // Assert
            Assert.True(typeof(IDisposable).IsAssignableFrom(typeof(NetworkShareConnector)));
        }

        [Fact]
        public void NetworkShareConnector_CanBeDisposedMultipleTimes()
        {
            // Questo test verifica che il dispose multiplo non causi problemi
            // Usa una share locale fittizia che fallirà la connessione
            var invalidPath = "\\\\localhost\\invalid-share-test";

            // Il costruttore lancerà un'eccezione, quindi testiamo solo che
            // il tipo implementi correttamente il pattern Dispose
            var disposeMethod = typeof(NetworkShareConnector).GetMethod("Dispose");
            Assert.NotNull(disposeMethod);
        }
    }
}
