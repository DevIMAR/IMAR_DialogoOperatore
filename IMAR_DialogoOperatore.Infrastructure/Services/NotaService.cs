using Dapper;
using IMAR_DialogoOperatore.Application.Interfaces.Repositories;
using IMAR_DialogoOperatore.Application.Interfaces.Services.Activities;
using IMAR_DialogoOperatore.Application.Interfaces.Utilities;
using IMAR_DialogoOperatore.Domain.Models;

namespace IMAR_DialogoOperatore.Infrastructure.Services
{
    public class NotaService : INotaService
    {
        private const int MAX_LEN_NOTA = 40;

        private readonly IAs400Repository _as400Repository;
        private readonly IOperatoreService _operatoreService;
        private readonly ILoggingService _loggingService;

        public NotaService(
            IAs400Repository as400Repository,
            IOperatoreService operatoreService,
            ILoggingService loggingService)
        {
            _as400Repository = as400Repository;
            _operatoreService = operatoreService;
            _loggingService = loggingService;
        }

        public IEnumerable<Nota> GetNoteAttivita(Attivita? attivita)
        { 
            if (attivita == null || (attivita.Bolla.Length == 5 && !attivita.Bolla.Contains("AI")))
                return new List<Nota>();

            if (attivita.Bolla.Contains("AI"))
               return GetNoteDaBolla(attivita.Bolla);
            else
                return GetNoteDaOdpFase(attivita.Odp, attivita.Fase);
        }

        public IEnumerable<Nota> GetNoteDaBolla(string bolla)
        {
            string query = "SELECT DT01PM AS DataImmissione, PROFPM AS Operatore, ORPRPM AS Odp, CDFAPM AS Fase, NRRGPM AS Riga, COMMPM AS Testo, FLA5PM AS Bolla " +
                           "FROM IMA90DAT.PMNOT00F " +
                           $"WHERE FLA5PM = '{bolla.Substring(2)}'";

            return _as400Repository.ExecuteQuery<Nota>(query);
        }
        public IEnumerable<Nota> GetNoteDaOdpFase(string odp, string fase)
        {
            string query = "SELECT DT01PM AS DataImmissione, PROFPM AS Operatore, ORPRPM AS Odp, CDFAPM AS Fase, NRRGPM AS Riga, COMMPM AS Testo, FLA5PM AS Bolla " +
                           "FROM IMA90DAT.PMNOT00F " +
                           $"WHERE ORPRPM = '{odp}' AND CDFAPM = '{fase}'";

            return _as400Repository.ExecuteQuery<Nota>(query);
        }
        public void AggiungiNota(Attivita? attivita, string? testo)
        {
            if (attivita == null || (attivita.Bolla.Length == 5 && !attivita.Bolla.Contains("AI")))
                return;

            try
            {
                ArgumentNullException.ThrowIfNull(attivita);

                ArgumentNullException.ThrowIfNull(testo);

                string operatore = _operatoreService.Operatore.Badge;
                decimal data = decimal.Parse(DateTime.Today.ToString("yyyyMMdd"));
                string odp = attivita.Odp ?? attivita.Bolla.Substring(2).PadLeft(5, '0');
                string articolo = attivita.Articolo ?? "";
                string fase = attivita.Fase ?? "";
                decimal rigaBase = attivita.Note != null ? attivita.Note.Count() : 0;
                string bolla = (attivita.Bolla.Contains("AI") ? attivita.Bolla.Substring(2) : "");

                string query = @"INSERT INTO IMA90DAT.PMNOT00F
                                    (TIREPM, PROFPM, DT01PM, DTMNPM, CDDTPM,
                                     CDLGPM, ORPRPM, TPCOPM, SEQUPM, CDARPM,
                                     CTGMPM, CDPRPM, CDMGPM, CDFAPM, NRRGPM,
                                     COMMPM, FLA1PM, FLA2PM, FLA3PM, FLA4PM,
                                     FLA5PM)
                                VALUES
                                    (' ', ?, ?, 0, '01',
                                     ' ', ?, 'T', ' ', ?,
                                     ' ', '0', '100', ?, ?,
                                     ?, ' ', ' ', ' ', ' ',
                                     ?)";

                var righeNota = SplitNote(testo).ToList();
                for (int i = 0; i < righeNota.Count; i++)
                {
                    decimal riga = rigaBase + i + 1;

                    var p = new DynamicParameters();
                    p.Add("Operatore", operatore);
                    p.Add("Data", data);
                    p.Add("Odp", odp);
                    p.Add("Articolo", articolo);
                    p.Add("Fase", fase);
                    p.Add("Riga", riga);
                    p.Add("Testo", righeNota[i]);
                    p.Add("Bolla", bolla);

                    _as400Repository.ExecuteCommand(query, p);
                }
            }
            catch (Exception ex)
            {
                _loggingService.LogError($"Errore nell'aggiunta nota per attività {attivita?.Bolla}", ex);
                throw;
            }
        }

        private IEnumerable<string> SplitNote(string testo)
        {
            if (string.IsNullOrEmpty(testo))
                yield break;

            // Normalizzo gli a capo
            testo = testo.Replace("\r\n", "\n").Replace("\r", "\n");

            foreach (var paragrafo in testo.Split('\n'))
            {
                var remaining = paragrafo.Trim();
                while (remaining.Length > MAX_LEN_NOTA)
                {
                    int len = MAX_LEN_NOTA;

                    // provo a spezzare sull'ultimo spazio prima di maxLen
                    int lastSpace = remaining.LastIndexOf(' ', MAX_LEN_NOTA - 1, MAX_LEN_NOTA);
                    if (lastSpace > 0)
                        len = lastSpace;

                    var line = remaining.Substring(0, len).TrimEnd();
                    if (line.Length > 0)
                        yield return line;

                    remaining = remaining.Substring(len).TrimStart();
                }

                if (remaining.Length > 0)
                    yield return remaining;
            }
        }
    }
}
