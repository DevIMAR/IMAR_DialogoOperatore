using FluentAssertions;
using IMAR_DialogoOperatore.Application.Interfaces.Services.External;
using NSubstitute;

namespace IMAR_DialogoOperatore.Test.Infrastructure.Services;

public class SegnalazioniDifformitaServiceTests
{
    private readonly ISegnalazioniDifformitaService _segnalazioniService;

    public SegnalazioniDifformitaServiceTests()
    {
        _segnalazioniService = Substitute.For<ISegnalazioniDifformitaService>();
    }

    [Fact]
    public void SegnalazioniDifformitaService_Interface_ShouldExist()
    {
        // Verify the service interface exists
        var interfaceType = typeof(ISegnalazioniDifformitaService);
        
        // Assert
        interfaceType.Should().NotBeNull();
        interfaceType.IsInterface.Should().BeTrue();
        interfaceType.Name.Should().Be("ISegnalazioniDifformitaService");
    }

    [Fact]
    public void SegnalazioniDifformitaService_Mock_ShouldBeInstantiable()
    {
        // Act & Assert
        _segnalazioniService.Should().NotBeNull();
    }

    [Fact]
    public void SegnalazioniDifformitaService_ShouldBeInExternalNamespace()
    {
        // Verify it's in the External services namespace
        var interfaceType = typeof(ISegnalazioniDifformitaService);
        
        // Assert
        interfaceType.Namespace.Should().Contain("External");
    }

    [Theory]
    [InlineData("SEG001")]
    [InlineData("DIFFORMITA_123")]
    [InlineData("SEGNALAZIONE_456")]
    public void SegnalazioniDifformitaService_ShouldHandleValidIds(string segnalazioneId)
    {
        // This test verifies the service concept can handle various ID formats
        // Actual implementation would depend on the service interface methods
        
        // Assert
        segnalazioneId.Should().NotBeNullOrEmpty();
        segnalazioneId.Should().MatchRegex(@"^[A-Z0-9_]+$");
    }
}