namespace IMAR_DialogoOperatore.Domain.Models
{
    public class ODPSchedulazione
    {
        public int IdxForzatura { get; set; }
        public string Codice { get; set; }
        public int SequenzaFase { get; set; }
        public string CodiceFase { get; set; }
        public string DescrizioneFase { get; set; }
        public double Durata { get; set; }
        public int Livello { get; set; }
        public string Flusso { get; set; }
        public bool InFlusso { get; set; }
        public DateTime? DataArrivoMateriale { get; set; }
        public string InfoDataArrivoMateriale { get; set; }
        public DateTime GiornoSchedulazione { get; set; }
        public bool Sovraccarico { get; set; }
        public int Manuale { get; set; }
        public string CodiceArticolo { get; set; }
        public double QuantitaResidua { get; set; }
    }
}
