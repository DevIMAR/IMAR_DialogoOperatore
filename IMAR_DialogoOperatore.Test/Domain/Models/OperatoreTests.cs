using FluentAssertions;
using IMAR_DialogoOperatore.Domain.Models;

namespace IMAR_DialogoOperatore.Test.Domain.Models;

public class OperatoreTests
{
    [Fact]
    public void Operatore_DefaultConstructor_ShouldInitializeProperties()
    {
        var operatore = new Operatore();

        operatore.Should().NotBeNull();
        operatore.IdJMes.Should().Be(0);
        operatore.Ingresso.Should().Be(default);
        operatore.Uscita.Should().Be(default);
    }

    [Fact]
    public void Operatore_Properties_ShouldBeSettableAndGettable()
    {
        var operatore = new Operatore();
        var macchina = new Macchina { CodiceMacchina = "M001", CentroDiLavoro = "CL001" };
        var attivita = new List<Attivita>
        {
            new() { Bolla = "B001", Causale = "IN_LAVORO" },
            new() { Bolla = "B002", Causale = "IN_ATTREZZAGGIO" }
        };
        var ingresso = DateTime.Now.Date.AddHours(8);
        var uscita = DateTime.Now.Date.AddHours(17);

        operatore.Badge = "12345";
        operatore.Nome = "Mario";
        operatore.Cognome = "Rossi";
        operatore.Stato = "PRESENTE";
        operatore.Ingresso = ingresso;
        operatore.Uscita = uscita;
        operatore.AttivitaAperte = attivita;
        operatore.MacchinaAssegnata = macchina;
        operatore.IdJMes = 100;

        operatore.Badge.Should().Be("12345");
        operatore.Nome.Should().Be("Mario");
        operatore.Cognome.Should().Be("Rossi");
        operatore.Stato.Should().Be("PRESENTE");
        operatore.Ingresso.Should().Be(ingresso);
        operatore.Uscita.Should().Be(uscita);
        operatore.AttivitaAperte.Should().BeEquivalentTo(attivita);
        operatore.MacchinaAssegnata.Should().Be(macchina);
        operatore.IdJMes.Should().Be(100);
    }

    [Theory]
    [InlineData("PRESENTE")]
    [InlineData("ASSENTE")]
    [InlineData("IN_PAUSA")]
    public void Operatore_Stato_ShouldAcceptValidStates(string stato)
    {
        var operatore = new Operatore { Stato = stato };

        operatore.Stato.Should().Be(stato);
    }

    [Fact]
    public void Operatore_MacchinaAssegnata_ShouldAcceptNullValue()
    {
        var operatore = new Operatore
        {
            MacchinaAssegnata = null
        };

        operatore.MacchinaAssegnata.Should().BeNull();
    }

    [Fact]
    public void Operatore_AttivitaAperte_ShouldSupportMultipleActivities()
    {
        var operatore = new Operatore();
        var attivita1 = new Attivita { Bolla = "B001", Causale = "IN_LAVORO" };
        var attivita2 = new Attivita { Bolla = "B002", Causale = "IN_ATTREZZAGGIO" };

        operatore.AttivitaAperte = new List<Attivita> { attivita1, attivita2 };

        operatore.AttivitaAperte.Should().HaveCount(2);
        operatore.AttivitaAperte.Should().Contain(attivita1);
        operatore.AttivitaAperte.Should().Contain(attivita2);
    }

    [Fact]
    public void Operatore_DateTimeProperties_ShouldHandleDateTimeValues()
    {
        var operatore = new Operatore();
        var now = DateTime.Now;

        operatore.Ingresso = now;
        operatore.Uscita = now.AddHours(8);
        operatore.InizioPausa = now.AddHours(4);
        operatore.FinePausa = now.AddHours(4.5);

        operatore.Ingresso.Should().Be(now);
        operatore.Uscita.Should().Be(now.AddHours(8));
        operatore.InizioPausa.Should().Be(now.AddHours(4));
        operatore.FinePausa.Should().Be(now.AddHours(4.5));
    }
}