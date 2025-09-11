using FluentAssertions;
using IMAR_DialogoOperatore.Domain.Models;

namespace IMAR_DialogoOperatore.Test.Validation;

public class DomainValidationTests
{
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Operatore_Badge_ShouldHandleInvalidValues(string? badge)
    {
        // Arrange & Act
        var operatore = new Operatore { Badge = badge! };

        // Assert
        operatore.Badge.Should().Be(badge);
    }

    [Theory]
    [InlineData("12345")]
    [InlineData("OP001")]
    [InlineData("BADGE123")]
    public void Operatore_Badge_ShouldAcceptValidValues(string badge)
    {
        // Arrange & Act
        var operatore = new Operatore { Badge = badge };

        // Assert
        operatore.Badge.Should().Be(badge);
        operatore.Badge.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Operatore_AttivitaAperte_ShouldNotBeNull()
    {
        // Arrange
        var operatore = new Operatore
        {
            AttivitaAperte = new List<Attivita>()
        };

        // Act & Assert
        operatore.AttivitaAperte.Should().NotBeNull();
        operatore.AttivitaAperte.Should().BeEmpty();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Attivita_QuantityValues_ShouldHandleNegativeValues(int quantity)
    {
        // Arrange & Act
        var attivita = new Attivita
        {
            QuantitaProdotta = quantity,
            QuantitaScartata = quantity
        };

        // Assert
        attivita.QuantitaProdotta.Should().Be(quantity);
        attivita.QuantitaScartata.Should().Be(quantity);
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(100, 50, 50)]
    [InlineData(100, 80, 20)]
    public void Attivita_QuantityCalculations_ShouldBeConsistent(int ordine, int prodotta, int residua)
    {
        // Arrange
        var attivita = new Attivita
        {
            QuantitaOrdine = ordine,
            QuantitaProdotta = prodotta,
            QuantitaResidua = residua
        };

        // Assert
        (attivita.QuantitaProdotta + attivita.QuantitaResidua).Should().BeLessOrEqualTo(attivita.QuantitaOrdine);
    }

    [Fact]
    public void Macchina_CodiceJMes_ShouldAcceptZeroValue()
    {
        // Arrange & Act
        var macchina = new Macchina { CodiceJMes = 0 };

        // Assert
        macchina.CodiceJMes.Should().Be(0);
    }

    [Theory]
    [InlineData("PRESENTE")]
    [InlineData("ASSENTE")]
    [InlineData("IN_PAUSA")]
    [InlineData("IN_LAVORO")]
    public void Operatore_Stato_ShouldAcceptCommonStates(string stato)
    {
        // Arrange & Act
        var operatore = new Operatore { Stato = stato };

        // Assert
        operatore.Stato.Should().Be(stato);
    }

    [Fact]
    public void DateTime_Properties_ShouldHandleMinMaxValues()
    {
        // Arrange
        var operatore = new Operatore
        {
            Ingresso = DateTime.MinValue,
            Uscita = DateTime.MaxValue
        };

        // Assert
        operatore.Ingresso.Should().Be(DateTime.MinValue);
        operatore.Uscita.Should().Be(DateTime.MaxValue);
        operatore.Uscita.Should().BeAfter(operatore.Ingresso);
    }
}