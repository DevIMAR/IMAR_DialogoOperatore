using IMAR_DialogoOperatore.Domain.Models;

namespace IMAR_DialogoOperatore.Test.Helpers;

public static class TestDataBuilder
{
    public static Operatore CreateDefaultOperatore(string? badge = null)
    {
        return new Operatore
        {
            Badge = badge ?? "12345",
            Nome = "Mario",
            Cognome = "Rossi",
            Stato = "PRESENTE",
            Ingresso = DateTime.Today.AddHours(8),
            Uscita = DateTime.Today.AddHours(17),
            AttivitaAperte = new List<Attivita>(),
            IdJMes = 100
        };
    }

    public static Attivita CreateDefaultAttivita(string? bolla = null)
    {
        return new Attivita
        {
            Causale = "IN_LAVORO",
            CausaleEstesa = "In Lavorazione",
            Bolla = bolla ?? "B001",
            Odp = "ODP001",
            Articolo = "ART001",
            DescrizioneArticolo = "Articolo di test",
            Fase = "F001",
            DescrizioneFase = "Prima fase",
            QuantitaOrdine = 100,
            QuantitaProdotta = 0,
            QuantitaScartata = 0,
            QuantitaResidua = 100,
            SaldoAcconto = "A",
            CodiceJMes = 12345.67,
            InizioAttivita = DateTime.Now,
            Macchina = CreateDefaultMacchina()
        };
    }

    public static Macchina CreateDefaultMacchina(string? codice = null)
    {
        return new Macchina
        {
            CodiceMacchina = codice ?? "M001",
            CentroDiLavoro = "CL001",
            CodiceJMes = 12345
        };
    }

    public static List<Attivita> CreateAttivitaList(int count = 3)
    {
        var attivita = new List<Attivita>();
        for (int i = 1; i <= count; i++)
        {
            attivita.Add(CreateDefaultAttivita($"B{i:000}"));
        }
        return attivita;
    }

    public static Operatore CreateOperatoreWithAttivita(int attivitaCount = 2)
    {
        var operatore = CreateDefaultOperatore();
        operatore.AttivitaAperte = CreateAttivitaList(attivitaCount);
        return operatore;
    }

    public static Attivita CreateAttivitaInProgress(string bolla, int quantitaProdotta = 50)
    {
        var attivita = CreateDefaultAttivita(bolla);
        attivita.QuantitaProdotta = quantitaProdotta;
        attivita.QuantitaResidua = attivita.QuantitaOrdine - quantitaProdotta;
        return attivita;
    }

    public static Attivita CreateCompletedAttivita(string bolla)
    {
        var attivita = CreateDefaultAttivita(bolla);
        attivita.QuantitaProdotta = attivita.QuantitaOrdine;
        attivita.QuantitaResidua = 0;
        attivita.FineAttivita = DateTime.Now;
        attivita.Causale = "COMPLETATA";
        return attivita;
    }
}