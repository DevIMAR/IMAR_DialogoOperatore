using FluentAssertions;
using IMAR_DialogoOperatore.Domain.Models;

namespace IMAR_DialogoOperatore.Test.Domain.Models;

public class AttivitaAdvancedTests
{
    [Fact]
    public void Attivita_WithComplexScenario_ShouldHandleMultipleOperations()
    {
        // Arrange
        var attivita = new Attivita
        {
            Bolla = "B12345",
            Odp = "ODP001",
            QuantitaOrdine = 1000,
            QuantitaProdotta = 750,
            QuantitaScartata = 50
        };

        // Act - Simulate production progress
        attivita.QuantitaResidua = attivita.QuantitaOrdine - attivita.QuantitaProdotta - attivita.QuantitaScartata;

        // Assert
        attivita.QuantitaResidua.Should().Be(200);
        (attivita.QuantitaProdotta + attivita.QuantitaScartata + attivita.QuantitaResidua)
            .Should().Be(attivita.QuantitaOrdine);
    }

    [Theory]
    [InlineData(1000, 800, 100, 100)]  // Normal case
    [InlineData(500, 500, 0, 0)]       // Completed without scrap
    [InlineData(100, 0, 0, 100)]       // Not started
    [InlineData(1000, 900, 200, -100)] // Over-produced scenario
    public void Attivita_QuantityCalculations_ShouldBeConsistent(
        int ordine, int prodotta, int scartata, int expectedResidua)
    {
        // Arrange
        var attivita = new Attivita
        {
            QuantitaOrdine = ordine,
            QuantitaProdotta = prodotta,
            QuantitaScartata = scartata
        };

        // Act
        var calculatedResidua = ordine - prodotta - scartata;

        // Assert
        calculatedResidua.Should().Be(expectedResidua);
        
        if (expectedResidua >= 0)
        {
            (prodotta + scartata).Should().BeLessOrEqualTo(ordine);
        }
    }

    [Fact]
    public void Attivita_WithContabilizzatedQuantities_ShouldTrackBothTypes()
    {
        // Arrange
        var attivita = new Attivita
        {
            QuantitaProdottaNonContabilizzata = 100,
            QuantitaProdottaContabilizzata = 50,
            QuantitaScartataNonContabilizzata = 20,
            QuantitaScartataContabilizzata = 10
        };

        // Act
        var totalProdotta = attivita.QuantitaProdottaNonContabilizzata + attivita.QuantitaProdottaContabilizzata;
        var totalScartata = attivita.QuantitaScartataNonContabilizzata + attivita.QuantitaScartataContabilizzata;

        // Assert
        totalProdotta.Should().Be(150);
        totalScartata.Should().Be(30);
        attivita.QuantitaProdottaNonContabilizzata.Should().BeGreaterThan(0);
        attivita.QuantitaProdottaContabilizzata.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Attivita_WithTimeTracking_ShouldCalculateDuration()
    {
        // Arrange
        var inizio = DateTime.Now.AddHours(-8);
        var fine = DateTime.Now;
        var attivita = new Attivita
        {
            InizioAttivita = inizio,
            FineAttivita = fine
        };

        // Act
        var durata = attivita.FineAttivita - attivita.InizioAttivita;

        // Assert
        durata.Should().NotBeNull();
        durata.Value.TotalHours.Should().BeApproximately(8, 0.1);
        attivita.FineAttivita.Should().BeAfter(attivita.InizioAttivita);
    }

    [Theory]
    [InlineData("A")]      // Acconto
    [InlineData("S")]      // Saldo
    [InlineData("")]       // Empty
    [InlineData(null)]     // Null
    public void Attivita_SaldoAcconto_ShouldAcceptValidValues(string? saldoAcconto)
    {
        // Arrange & Act
        var attivita = new Attivita { SaldoAcconto = saldoAcconto! };

        // Assert
        attivita.SaldoAcconto.Should().Be(saldoAcconto);
    }

    [Fact]
    public void Attivita_WithMacchina_ShouldMaintainReference()
    {
        // Arrange
        var macchina = new Macchina
        {
            CodiceMacchina = "CNC001",
            CentroDiLavoro = "PRODUZIONE",
            CodiceJMes = 12345
        };

        var attivita = new Attivita
        {
            Bolla = "B001",
            Macchina = macchina
        };

        // Act & Assert
        attivita.Macchina.Should().NotBeNull();
        attivita.Macchina.CodiceMacchina.Should().Be("CNC001");
        attivita.Macchina.CentroDiLavoro.Should().Be("PRODUZIONE");
        attivita.Macchina.CodiceJMes.Should().Be(12345);
    }
}