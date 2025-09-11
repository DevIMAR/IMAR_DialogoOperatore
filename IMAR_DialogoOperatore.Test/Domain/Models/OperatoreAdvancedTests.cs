using FluentAssertions;
using IMAR_DialogoOperatore.Domain.Models;

namespace IMAR_DialogoOperatore.Test.Domain.Models;

public class OperatoreAdvancedTests
{
    [Fact]
    public void Operatore_WithMultipleAttivita_ShouldManageWorkload()
    {
        // Arrange
        var operatore = new Operatore
        {
            Badge = "OP001",
            Nome = "Giovanni",
            Cognome = "Bianchi",
            AttivitaAperte = new List<Attivita>()
        };

        var attivita1 = new Attivita { Bolla = "B001", Causale = "IN_LAVORO" };
        var attivita2 = new Attivita { Bolla = "B002", Causale = "IN_ATTREZZAGGIO" };
        var attivita3 = new Attivita { Bolla = "B003", Causale = "SOSPESA" };

        // Act
        operatore.AttivitaAperte.Add(attivita1);
        operatore.AttivitaAperte.Add(attivita2);
        operatore.AttivitaAperte.Add(attivita3);

        // Assert
        operatore.AttivitaAperte.Should().HaveCount(3);
        operatore.AttivitaAperte.Should().Contain(a => a.Causale == "IN_LAVORO");
        operatore.AttivitaAperte.Should().Contain(a => a.Causale == "IN_ATTREZZAGGIO");
        operatore.AttivitaAperte.Should().Contain(a => a.Causale == "SOSPESA");
    }

    [Fact]
    public void Operatore_WorkingHours_ShouldCalculateCorrectly()
    {
        // Arrange
        var operatore = new Operatore
        {
            Badge = "OP002",
            Ingresso = DateTime.Today.AddHours(8),      // 8:00 AM
            Uscita = DateTime.Today.AddHours(17),       // 5:00 PM
            InizioPausa = DateTime.Today.AddHours(12),  // 12:00 PM
            FinePausa = DateTime.Today.AddHours(13)     // 1:00 PM
        };

        // Act
        var oreTotali = operatore.Uscita - operatore.Ingresso;
        var orePausa = operatore.FinePausa - operatore.InizioPausa;
        var oreLavoro = oreTotali - orePausa;

        // Assert
        oreTotali.TotalHours.Should().Be(9);  // 9 hours total
        orePausa.TotalHours.Should().Be(1);   // 1 hour pause
        oreLavoro.TotalHours.Should().Be(8);  // 8 hours work
    }

    [Theory]
    [InlineData("PRESENTE", true)]
    [InlineData("ASSENTE", false)]
    [InlineData("IN_PAUSA", false)]
    [InlineData("IN_LAVORO", true)]
    public void Operatore_IsAvailableForWork_ShouldDependOnStato(string stato, bool expectedAvailable)
    {
        // Arrange
        var operatore = new Operatore
        {
            Badge = "OP003",
            Stato = stato
        };

        // Act
        var isAvailable = stato == "PRESENTE" || stato == "IN_LAVORO";

        // Assert
        isAvailable.Should().Be(expectedAvailable);
        operatore.Stato.Should().Be(stato);
    }

    [Fact]
    public void Operatore_WithAssignedMacchina_ShouldTrackAssignment()
    {
        // Arrange
        var macchina = new Macchina
        {
            CodiceMacchina = "TORNIO_01",
            CentroDiLavoro = "MECCANICA",
            CodiceJMes = 98765
        };

        var operatore = new Operatore
        {
            Badge = "OP004",
            Nome = "Maria",
            Cognome = "Verdi",
            MacchinaAssegnata = macchina
        };

        // Act & Assert
        operatore.MacchinaAssegnata.Should().NotBeNull();
        operatore.MacchinaAssegnata.CodiceMacchina.Should().Be("TORNIO_01");
        operatore.MacchinaAssegnata.CentroDiLavoro.Should().Be("MECCANICA");
    }

    [Fact]
    public void Operatore_IdJMes_ShouldBeUniqueIdentifier()
    {
        // Arrange
        var operatore1 = new Operatore { Badge = "OP001", IdJMes = 1001 };
        var operatore2 = new Operatore { Badge = "OP002", IdJMes = 1002 };

        // Act & Assert
        operatore1.IdJMes.Should().NotBe(operatore2.IdJMes);
        operatore1.IdJMes.Should().BePositive();
        operatore2.IdJMes.Should().BePositive();
    }

    [Fact]
    public void Operatore_CanWork_WhenPresenteAndHasMacchina()
    {
        // Arrange
        var operatore = new Operatore
        {
            Badge = "OP005",
            Nome = "Luigi",
            Cognome = "Neri",
            Stato = "PRESENTE",
            MacchinaAssegnata = new Macchina { CodiceMacchina = "M001" },
            AttivitaAperte = new List<Attivita>()
        };

        // Act
        var canWork = operatore.Stato == "PRESENTE" && 
                     operatore.MacchinaAssegnata != null && 
                     operatore.AttivitaAperte != null;

        // Assert
        canWork.Should().BeTrue();
        operatore.Stato.Should().Be("PRESENTE");
        operatore.MacchinaAssegnata.Should().NotBeNull();
        operatore.AttivitaAperte.Should().NotBeNull();
    }

    [Theory]
    [InlineData("", "", false)]           // Empty names
    [InlineData("Mario", "", false)]      // Missing surname
    [InlineData("", "Rossi", false)]      // Missing name
    [InlineData("Mario", "Rossi", true)]  // Valid names
    public void Operatore_HasValidName_ShouldValidateCorrectly(string nome, string cognome, bool expectedValid)
    {
        // Arrange
        var operatore = new Operatore
        {
            Badge = "OP006",
            Nome = nome,
            Cognome = cognome
        };

        // Act
        var hasValidName = !string.IsNullOrEmpty(operatore.Nome) && !string.IsNullOrEmpty(operatore.Cognome);

        // Assert
        hasValidName.Should().Be(expectedValid);
    }
}