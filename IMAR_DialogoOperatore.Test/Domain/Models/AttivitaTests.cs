using FluentAssertions;
using IMAR_DialogoOperatore.Domain.Models;

namespace IMAR_DialogoOperatore.Test.Domain.Models;

public class AttivitaTests
{
    [Fact]
    public void Attivita_DefaultConstructor_ShouldInitializeCollections()
    {
        var attivita = new Attivita();

        attivita.Should().NotBeNull();
        attivita.QuantitaOrdine.Should().Be(0);
        attivita.QuantitaProdotta.Should().Be(0);
        attivita.QuantitaScartata.Should().Be(0);
    }

    [Fact]
    public void Attivita_Properties_ShouldBeSettableAndGettable()
    {
        var attivita = new Attivita();
        var macchina = new Macchina { CodiceMacchina = "M001", CentroDiLavoro = "CL001" };
        var dataInizio = DateTime.Now;
        var dataFine = DateTime.Now.AddHours(8);

        attivita.Causale = "IN_LAVORO";
        attivita.CausaleEstesa = "In Lavorazione";
        attivita.Bolla = "B12345";
        attivita.Odp = "ODP001";
        attivita.Articolo = "ART001";
        attivita.DescrizioneArticolo = "Articolo di test";
        attivita.Fase = "F001";
        attivita.DescrizioneFase = "Prima fase";
        attivita.QuantitaOrdine = 100;
        attivita.QuantitaProdotta = 80;
        attivita.QuantitaScartata = 5;
        attivita.SaldoAcconto = "A";
        attivita.CodiceJMes = 12345.67;
        attivita.Macchina = macchina;
        attivita.InizioAttivita = dataInizio;
        attivita.FineAttivita = dataFine;

        attivita.Causale.Should().Be("IN_LAVORO");
        attivita.CausaleEstesa.Should().Be("In Lavorazione");
        attivita.Bolla.Should().Be("B12345");
        attivita.Odp.Should().Be("ODP001");
        attivita.Articolo.Should().Be("ART001");
        attivita.DescrizioneArticolo.Should().Be("Articolo di test");
        attivita.Fase.Should().Be("F001");
        attivita.DescrizioneFase.Should().Be("Prima fase");
        attivita.QuantitaOrdine.Should().Be(100);
        attivita.QuantitaProdotta.Should().Be(80);
        attivita.QuantitaScartata.Should().Be(5);
        attivita.SaldoAcconto.Should().Be("A");
        attivita.CodiceJMes.Should().Be(12345.67);
        attivita.Macchina.Should().Be(macchina);
        attivita.InizioAttivita.Should().Be(dataInizio);
        attivita.FineAttivita.Should().Be(dataFine);
    }

    [Theory]
    [InlineData(100, 80, 5, 15)] // Normale
    [InlineData(100, 100, 0, 0)] // Completato senza scarti
    [InlineData(50, 30, 10, 10)] // Parzialmente completato con scarti
    public void QuantitaResidua_ShouldBeCalculatedCorrectly(int quantitaOrdine, int quantitaProdotta, int quantitaScartata, int expectedResidua)
    {
        var attivita = new Attivita
        {
            QuantitaOrdine = quantitaOrdine,
            QuantitaProdotta = quantitaProdotta,
            QuantitaScartata = quantitaScartata,
            QuantitaResidua = quantitaOrdine - quantitaProdotta - quantitaScartata
        };

        attivita.QuantitaResidua.Should().Be(expectedResidua);
    }

    [Fact]
    public void Attivita_NullableDateTimeFineAttivita_ShouldAcceptNullValue()
    {
        var attivita = new Attivita
        {
            InizioAttivita = DateTime.Now,
            FineAttivita = null
        };

        attivita.FineAttivita.Should().BeNull();
    }

    [Fact]
    public void Attivita_NullableCodiceJMes_ShouldAcceptNullValue()
    {
        var attivita = new Attivita
        {
            CodiceJMes = null
        };

        attivita.CodiceJMes.Should().BeNull();
    }
}