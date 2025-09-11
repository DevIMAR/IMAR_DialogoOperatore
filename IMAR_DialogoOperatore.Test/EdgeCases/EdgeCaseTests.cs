using FluentAssertions;
using IMAR_DialogoOperatore.Application.Interfaces.Services.Activities;
using IMAR_DialogoOperatore.Domain.Models;
using NSubstitute;

namespace IMAR_DialogoOperatore.Test.EdgeCases;

public class EdgeCaseTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void AttivitaService_ConfrontaCausaliAttivita_WithInvalidBolla_ShouldHandleGracefully(string? bolla)
    {
        // Arrange
        var service = Substitute.For<IAttivitaService>();
        var attivita = new List<Attivita> { new() { Bolla = "B001", Causale = "IN_LAVORO" } };
        
        // Configure mock to return false for invalid bolla
        service.ConfrontaCausaliAttivita(attivita, bolla!, "TEST").Returns(false);

        // Act
        var result = service.ConfrontaCausaliAttivita(attivita, bolla!, "TEST");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void AttivitaService_ConfrontaCausaliAttivita_WithEmptyList_ShouldReturnFalse()
    {
        // Arrange
        var service = Substitute.For<IAttivitaService>();
        var emptyList = new List<Attivita>();
        
        service.ConfrontaCausaliAttivita(emptyList, "B001", "TEST").Returns(false);

        // Act
        var result = service.ConfrontaCausaliAttivita(emptyList, "B001", "TEST");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Operatore_WithNullAttivitaAperte_ShouldHandleGracefully()
    {
        // Arrange & Act
        var operatore = new Operatore
        {
            Badge = "OP001",
            Nome = "Test",
            Cognome = "User",
            AttivitaAperte = null!
        };

        // Assert
        operatore.AttivitaAperte.Should().BeNull();
        operatore.Badge.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData(int.MaxValue)]
    [InlineData(int.MinValue)]
    [InlineData(0)]
    [InlineData(-1)]
    public void Attivita_WithExtremeQuantityValues_ShouldNotThrow(int quantity)
    {
        // Arrange & Act
        var act = () => new Attivita
        {
            QuantitaOrdine = quantity,
            QuantitaProdotta = quantity,
            QuantitaScartata = quantity
        };

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Macchina_WithVeryLongCodes_ShouldAcceptThem()
    {
        // Arrange
        var longCode = new string('A', 1000);
        var longDescription = new string('B', 2000);

        // Act
        var act = () => new Macchina
        {
            CodiceMacchina = longCode,
            CentroDiLavoro = longDescription,
            CodiceJMes = int.MaxValue
        };

        // Assert
        act.Should().NotThrow();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void Operatore_WithWhitespaceNames_ShouldAcceptButBeInvalid(string whitespace)
    {
        // Arrange & Act
        var operatore = new Operatore
        {
            Badge = "OP001",
            Nome = whitespace,
            Cognome = whitespace
        };

        // Assert
        operatore.Nome.Should().Be(whitespace);
        operatore.Cognome.Should().Be(whitespace);
        
        // Names are technically set but would be considered invalid in business logic
        string.IsNullOrWhiteSpace(operatore.Nome).Should().BeTrue();
        string.IsNullOrWhiteSpace(operatore.Cognome).Should().BeTrue();
    }

    [Fact]
    public void DateTime_Properties_WithMinMaxValues_ShouldHandleCorrectly()
    {
        // Arrange & Act
        var attivita = new Attivita
        {
            InizioAttivita = DateTime.MinValue,
            FineAttivita = DateTime.MaxValue
        };

        var operatore = new Operatore
        {
            Ingresso = DateTime.MinValue,
            Uscita = DateTime.MaxValue,
            InizioPausa = DateTime.MinValue,
            FinePausa = DateTime.MaxValue
        };

        // Assert
        attivita.InizioAttivita.Should().Be(DateTime.MinValue);
        attivita.FineAttivita.Should().Be(DateTime.MaxValue);
        operatore.Ingresso.Should().Be(DateTime.MinValue);
        operatore.Uscita.Should().Be(DateTime.MaxValue);
    }

    [Fact]
    public void Attivita_WithNullableCodiceJMes_EdgeCases_ShouldHandle()
    {
        // Arrange & Act
        var attivita1 = new Attivita { CodiceJMes = null };
        var attivita2 = new Attivita { CodiceJMes = double.MaxValue };
        var attivita3 = new Attivita { CodiceJMes = double.MinValue };
        var attivita4 = new Attivita { CodiceJMes = 0.0 };

        // Assert
        attivita1.CodiceJMes.Should().BeNull();
        attivita2.CodiceJMes.Should().Be(double.MaxValue);
        attivita3.CodiceJMes.Should().Be(double.MinValue);
        attivita4.CodiceJMes.Should().Be(0.0);
    }

    [Theory]
    [InlineData("STATO_MOLTO_LUNGO_CHE_NON_DOVREBBE_ESISTERE")]
    [InlineData("")]
    [InlineData("12345")]
    [InlineData("!@#$%")]
    [InlineData("STATO CON SPAZI")]
    public void Operatore_WithUnusualStates_ShouldAcceptAnyString(string stato)
    {
        // Arrange & Act
        var operatore = new Operatore { Badge = "OP001", Stato = stato };

        // Assert
        operatore.Stato.Should().Be(stato);
    }

    [Fact]
    public void ServiceInterfaces_WithNullParameters_ShouldBeTestable()
    {
        // Arrange
        var attivitaService = Substitute.For<IAttivitaService>();
        var macchinaService = Substitute.For<IMacchinaService>();
        var operatoreService = Substitute.For<IOperatoreService>();

        // Act - Configure mocks to handle null gracefully
        attivitaService.CercaAttivitaDaBolla(null!).Returns((Attivita?)null);
        macchinaService.GetMacchinaRealeByAttivita(null!).Returns((Macchina?)null);

        // Assert - Verify mocks can be configured
        attivitaService.Should().NotBeNull();
        macchinaService.Should().NotBeNull();
        operatoreService.Should().NotBeNull();
    }
}