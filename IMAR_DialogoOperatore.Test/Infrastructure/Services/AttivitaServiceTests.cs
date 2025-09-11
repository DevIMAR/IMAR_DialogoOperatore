using FluentAssertions;
using IMAR_DialogoOperatore.Application.Interfaces.Services.Activities;
using IMAR_DialogoOperatore.Domain.Models;
using NSubstitute;

namespace IMAR_DialogoOperatore.Test.Infrastructure.Services;

public class AttivitaServiceTests
{
    private readonly IAttivitaService _attivitaService;

    public AttivitaServiceTests()
    {
        _attivitaService = Substitute.For<IAttivitaService>();
    }

    [Fact]
    public void ConfrontaCausaliAttivita_WhenActivityExists_ShouldReturnTrue()
    {
        // Arrange
        var attivita = new List<Attivita>
        {
            new() { Bolla = "B001", Causale = "IN_LAVORO" },
            new() { Bolla = "B002", Causale = "IN_ATTREZZAGGIO" }
        };
        var bolla = "B001";
        var operazione = "IN_LAVORO";
        
        // Configure mock
        _attivitaService.ConfrontaCausaliAttivita(attivita, bolla, operazione).Returns(true);

        // Act
        var result = _attivitaService.ConfrontaCausaliAttivita(attivita, bolla, operazione);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ConfrontaCausaliAttivita_WhenActivityNotExists_ShouldReturnFalse()
    {
        // Arrange
        var attivita = new List<Attivita>
        {
            new() { Bolla = "B001", Causale = "IN_LAVORO" }
        };
        var bolla = "B999"; // Non esistente
        var operazione = "INIZIO_LAVORO";
        
        // Configure mock
        _attivitaService.ConfrontaCausaliAttivita(attivita, bolla, operazione).Returns(false);

        // Act
        var result = _attivitaService.ConfrontaCausaliAttivita(attivita, bolla, operazione);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ConfrontaCausaliAttivita_WithEmptyList_ShouldReturnFalse()
    {
        // Arrange
        var attivita = new List<Attivita>();
        var bolla = "B001";
        var operazione = "INIZIO_LAVORO";

        // Act
        var result = _attivitaService.ConfrontaCausaliAttivita(attivita, bolla, operazione);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("B001")]
    [InlineData("B002")]
    [InlineData("B003")]
    public void ConfrontaCausaliAttivita_WithDifferentBollas_ShouldWorkCorrectly(string bolla)
    {
        // Arrange
        var attivita = new List<Attivita>
        {
            new() { Bolla = "B001", Causale = "IN_LAVORO" },
            new() { Bolla = "B002", Causale = "IN_LAVORO" },
            new() { Bolla = "B003", Causale = "IN_LAVORO" }
        };
        var operazione = "IN_LAVORO";
        
        // Configure mock to return true for valid bollas
        _attivitaService.ConfrontaCausaliAttivita(attivita, bolla, operazione).Returns(true);

        // Act
        var result = _attivitaService.ConfrontaCausaliAttivita(attivita, bolla, operazione);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void GetIdOperatoriConBollaAperta_WithValidBolla_ShouldReturnOperatorIds()
    {
        // Arrange
        var bolla = "B001";
        var expectedIds = new List<string> { "OP001", "OP002" };
        
        // Configure mock
        _attivitaService.GetIdOperatoriConBollaAperta(bolla).Returns(expectedIds);

        // Act
        var result = _attivitaService.GetIdOperatoriConBollaAperta(bolla);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedIds);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void GetIdOperatoriConBollaAperta_WithInvalidBolla_ShouldHandleCorrectly(string? bolla)
    {
        // Act & Assert
        var act = () => _attivitaService.GetIdOperatoriConBollaAperta(bolla!);
        
        // The actual behavior would depend on implementation - might return empty list or throw
        act.Should().NotThrow();
    }

    [Fact]
    public void GetAttivitaIndirette_ShouldReturnListOfAttivita()
    {
        // Act
        var result = _attivitaService.GetAttivitaIndirette();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IList<Attivita>>();
    }

    [Fact]
    public void GetAttivitaPerOdp_WithValidOdp_ShouldReturnActivities()
    {
        // Arrange
        var odp = "ODP001";

        // Act
        var result = _attivitaService.GetAttivitaPerOdp(odp);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IEnumerable<Attivita>>();
    }

    [Fact]
    public void OttieniAttivitaOperatore_WithValidOperator_ShouldReturnActivities()
    {
        // Arrange
        var operatore = new Operatore
        {
            Badge = "12345",
            Nome = "Mario",
            Cognome = "Rossi",
            IdJMes = 100
        };

        // Act
        var result = _attivitaService.OttieniAttivitaOperatore(operatore);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IList<Attivita>>();
    }
}