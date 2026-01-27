namespace IMAR_DialogoOperatore.Domain.Models
{
    public class GiornoSchedulazione
    {
        public DateTime Giorno { get; set; }
        public List<ODPSchedulazione> OrdiniProduzioneInFlusso { get; set; } = new List<ODPSchedulazione>();
        public List<ODPSchedulazione> OrdiniProduzioneNonFlusso { get; set; } = new List<ODPSchedulazione>();
    }
}
