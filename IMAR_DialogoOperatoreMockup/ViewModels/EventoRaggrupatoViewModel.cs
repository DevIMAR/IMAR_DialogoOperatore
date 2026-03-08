namespace IMAR_DialogoOperatore.ViewModels
{
    /// <summary>
    /// Rappresenta un'attività raggruppata (Inizio + Fine) come riga unica nella grid del TaskPopup.
    /// Per le timbrature (Ingresso/Uscita), OraFine è null.
    /// </summary>
    public class EventoRaggrupatoViewModel
    {
        public double? CodiceJMes { get; set; }
        public string? Causale { get; set; }
        public string? CausaleEstesa { get; set; }
        public string? Bolla { get; set; }
        public string? Odp { get; set; }
        public string? CodiceFase { get; set; }
        public string? DescrizioneFase { get; set; }
        public string? CodiceArticolo { get; set; }
        public string? DescrizioneArticolo { get; set; }
        public int? QuantitaProdotta { get; set; }
        public int? QuantitaScartata { get; set; }
        public string? SaldoAcconto { get; set; }

        public DateTime? OraInizio { get; set; }
        public DateTime? OraFine { get; set; }

        /// <summary>True se è un'attività (ha Bolla), false se è una timbratura (Ingresso/Uscita)</summary>
        public bool IsAttivita => !string.IsNullOrEmpty(Bolla);
    }
}
