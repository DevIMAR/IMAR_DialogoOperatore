using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Domain.Models;

namespace EsportatoreTimbratureTeamSystem.Services
{
    public class EncodingService
    {
        public string GetTimbratureCodificate(List<Timbratura> timbrature)
        {
            string timbrtureCodificate = string.Empty;

            foreach (Timbratura timbratura in timbrature)
                timbrtureCodificate += CodificaTimbratura(timbratura) + "\n";

            return timbrtureCodificate;
        }

        private string CodificaTimbratura(Timbratura timbratura)
        {
            string codiceDitta = "1";
            string codicePostazione = "1";
            string codiceOperatore = timbratura.BadgeOperatore.PadLeft(10, '0');
            string tipo = timbratura.Causale == Costanti.INGRESSO || timbratura.Causale == Costanti.FINE_PAUSA ? "1" : "2";
            string causale = timbratura.Causale == Costanti.INIZIO_PAUSA || timbratura.Causale == Costanti.FINE_PAUSA ? "000P" : "    ";
            string data = timbratura.Timestamp.ToString("yyMMddHHmm");

            return "x" + codiceDitta + codicePostazione + codiceOperatore + tipo + causale + data;
        }
    }
}
