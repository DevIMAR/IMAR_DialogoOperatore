using IMAR_DialogoOperatore.Domain.Models;

namespace IMAR_DialogoOperatore.Domain.DTO
{
    public class ForzaturaDTO
    {
        public List<GiornoSchedulazione> Forzatura { get; set; }
        public string Errore { get; set; }
    }
}
