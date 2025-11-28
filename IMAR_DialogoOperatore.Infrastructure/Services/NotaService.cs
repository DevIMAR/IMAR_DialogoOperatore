using IMAR_DialogoOperatore.Application.Interfaces.Repositories;
using IMAR_DialogoOperatore.Application.Interfaces.Services.Activities;
using IMAR_DialogoOperatore.Domain.Models;

namespace IMAR_DialogoOperatore.Infrastructure.Services
{
    public class NotaService : INotaService
    {
        private readonly IAs400Repository _as400Repository;
        private readonly IOperatoreService _operatoreService;

        public NotaService(
            IAs400Repository as400Repository,
            IOperatoreService operatoreService)
        {
            _as400Repository = as400Repository;
            _operatoreService = operatoreService;
        }

        public IEnumerable<Nota> GetNoteDaBolla(string bolla)
        {
            string query = "SELECT DT01PM AS DataImmissione, PROFPM AS Operatore, ORPRPM AS Odp, CDFAPM AS Fase, NRRGPM AS Riga, COMMPM AS Testo, FLA5PM AS Bolla " +
                           "FROM IMA90DAT.PMNOT00F " +
                           $"WHERE FLA5PM = '{bolla}'";

            return _as400Repository.ExecuteQuery<Nota>(query);
        }
        public IEnumerable<Nota> GetNoteDaOdpFase(string odp, string fase)
        {
            string query = "SELECT DT01PM AS DataImmissione, PROFPM AS Operatore, ORPRPM AS Odp, CDFAPM AS Fase, NRRGPM AS Riga, COMMPM AS Testo, FLA5PM AS Bolla " +
                           "FROM IMA90DAT.PMNOT00F " +
                           $"WHERE ORPRPM = '{odp}' AND CDFAPM = '{fase}'";

            return _as400Repository.ExecuteQuery<Nota>(query);
        }
        public void AggiungiNota(Attivita attivita, string testo)
        {
            string query = "INSERT INTO IMA90DAT.PMNOT00F" +
                           "(TIREPM, PROFPM, DT01PM, DTMNPM, CDDTPM, " +
                           "CDLGPM, ORPRPM, TPCOPM, SEQUPM, CDARPM," +
                           " CTGMPM, CDPRPM, CDMGPM, CDFAPM, NRRGPM, " +
                           "COMMPM, FLA1PM, FLA2PM, FLA3PM, FLA4PM, " +
                           "FLA5PM)" +
                           $"VALUES(' ', '{_operatoreService.Operatore}', {decimal.Parse(DateTime.Today.ToString("yyyyMMdd"))}, 0, '01', " +
                           $"' ', '{attivita.Odp ?? ""}', 'T', ' ', '{attivita.Articolo ?? ""}', " +
                           $"' ', '0', '100', '{attivita.Fase ?? ""}', {attivita.Note.Count()}, " +
                           $"'{testo}', ' ', ' ', ' ', ' ', " +
                           $"'{(attivita.Bolla.Contains("AI") ? attivita.Bolla.Substring(2) : "")}')";

            _as400Repository.ExecuteQuery<Nota>(query);
        }
    }
}
