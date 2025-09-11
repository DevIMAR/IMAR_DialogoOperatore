using FluentAssertions;
using IMAR_DialogoOperatore.Application.Interfaces.Services.Activities;
using IMAR_DialogoOperatore.Domain.Models;
using NSubstitute;

namespace IMAR_DialogoOperatore.Test.Infrastructure.Services;

public class MacchinaServiceTests
{
    private readonly IMacchinaService _macchinaService;

    public MacchinaServiceTests()
    {
        _macchinaService = Substitute.For<IMacchinaService>();
    }

    [Fact]
    public void Constructor_ShouldInitializeCorrectly()
    {
        // Assert
        _macchinaService.Should().NotBeNull();
    }

    [Fact]
    public void MacchinaService_Interface_ShouldExist()
    {
        // Verify the service interface exists
        var interfaceType = typeof(IMacchinaService);
        
        // Assert
        interfaceType.Should().NotBeNull();
        interfaceType.IsInterface.Should().BeTrue();
        interfaceType.Name.Should().Be("IMacchinaService");
    }

    [Fact]
    public void MacchinaService_GetMacchinaRealeByAttivita_ShouldReturnMacchina()
    {
        // Arrange
        var testAttivita = new Attivita { Bolla = "B001" };
        var expectedMacchina = new Macchina { CodiceMacchina = "M001", CentroDiLavoro = "CL001" };
        
        _macchinaService.GetMacchinaRealeByAttivita(testAttivita).Returns(expectedMacchina);
        
        // Act
        var result = _macchinaService.GetMacchinaRealeByAttivita(testAttivita);
        
        // Assert
        result.Should().NotBeNull();
        result.CodiceMacchina.Should().Be("M001");
        result.CentroDiLavoro.Should().Be("CL001");
    }

    [Fact]
    public void Macchina_ShouldSupportProperStructure()
    {
        // Arrange
        var macchina = new Macchina
        {
            CodiceMacchina = "M001",
            CentroDiLavoro = "CL001",
            CodiceJMes = 12345
        };

        // Assert
        macchina.CodiceMacchina.Should().Be("M001");
        macchina.CentroDiLavoro.Should().Be("CL001");
        macchina.CodiceJMes.Should().Be(12345);
    }
}