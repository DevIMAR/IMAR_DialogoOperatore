using Dapper;
using IMAR_DialogoOperatore.Application.DTOs;
using IMAR_DialogoOperatore.Application.Interfaces.Clients;
using IMAR_DialogoOperatore.Application.Interfaces.Services.External;
using IMAR_DialogoOperatore.Application.Interfaces.UoW;
using IMAR_DialogoOperatore.Application.Interfaces.Utilities;
using IMAR_DialogoOperatore.Domain.Entities.Imar_Produzione;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace IMAR_DialogoOperatore.Infrastructure.Services
{
    public class SegnalazioniDifformitaService : ISegnalazioniDifformitaService
    {
        private readonly IImarProduzioneUoW _imarProduzioneUoW;
        private readonly ILoggingService _loggingService;
		private readonly IImarApiClient _imarApiClient;
		private readonly string _connectionString;

		public SegnalazioniDifformitaService(
            IImarProduzioneUoW imarProduzioneUoW,
            ILoggingService loggingService,
			IImarApiClient imarApiClient,
			IConfiguration configuration)
        {
            _imarProduzioneUoW = imarProduzioneUoW;
            _loggingService = loggingService;
			_imarApiClient = imarApiClient;
			_connectionString = configuration.GetConnectionString("ImarProduzione")!;
		}

        public int InsertSegnalazione(SegnalazioneDifformita segnalazione)
        {
            if (segnalazione.Id.Equals(Guid.Empty)) 
                segnalazione.Id = Guid.NewGuid();

            segnalazione.DataCreazione = segnalazione.DataCreazione is null ? DateTime.Now : segnalazione.DataCreazione;
            segnalazione.UltimaModifica = DateTime.Now;
            segnalazione.CodiceSegnalazione = SetCodiceSequenziale(segnalazione);
            _imarProduzioneUoW.SegnalazioniDifformitaRepository.Insert(segnalazione);
            return _imarProduzioneUoW.Save();
        }

        private bool CheckUniqueCodiceSequenziale(string nuovoCodiceSegnalazione)
        {
            var records = _imarProduzioneUoW.SegnalazioniDifformitaRepository.Get(x => x.CodiceSegnalazione == nuovoCodiceSegnalazione).AsQueryable();
            return records.Count() == 0 ? true : false;
        }

        private string SetCodiceSequenziale(SegnalazioneDifformita segnalazione)
        {
            try
            {
                int year = DateTime.Today.Year;
                var origine = segnalazione.OrigineSegnalazione;

                int recordsCount = _imarProduzioneUoW.SegnalazioniDifformitaRepository
                                                     .ExecuteQuery<SegnalazioneDifformita>($"SELECT * FROM SegnalazioneDifformita WHERE YEAR(dataCreazione) = '{year}' AND OrigineSegnalazione = '{origine}'")
                                                     .AsEnumerable()
                                                     .Count();
                string nuovoCodiceSegnalazione;

                nuovoCodiceSegnalazione = OttieniCodiceSegnalazioneUnivoco(segnalazione, ref recordsCount);

                return nuovoCodiceSegnalazione;
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Errore nella generazione codice segnalazione difformità", ex);
                throw;
            }
        }

        private string OttieniCodiceSegnalazioneUnivoco(SegnalazioneDifformita segnalazione, ref int recordsCount)
        {
            string nuovoCodiceSegnalazione;
            do
            {
                recordsCount++;
                string codificaSequenziale = GetCodificaSequenziale(segnalazione, recordsCount);
                nuovoCodiceSegnalazione = DateTime.Now.Year.ToString() + "_" + codificaSequenziale + "_" + segnalazione.OrigineSegnalazione;
            } while (!CheckUniqueCodiceSequenziale(nuovoCodiceSegnalazione));
            return nuovoCodiceSegnalazione;
        }

        private string GetCodificaSequenziale(SegnalazioneDifformita segnalazione, int recordsCount)
        {
            return segnalazione.OrigineSegnalazione == "E"
                        ? segnalazione.CodiceCliente + GetSequenziale(true, recordsCount)
                        : GetSequenziale(false, recordsCount);
        }

        private string GetSequenziale(bool esterna, int count)
        {
            string sequenziale = count.ToString();

            int max = esterna ? 4 : 8;
            for (int i = sequenziale.Length; i < max; i++)
                sequenziale = "0" + sequenziale;

            return sequenziale;
        }

        public async Task<CostiArticoloDTO> GetCostiArticolo(string codiceArticolo) => await _imarApiClient.GetCostiArticolo(codiceArticolo);

		public string? GetFlussoByOdpFase(string odp, string fase)
		{
			try
			{
				using var connection = new SqlConnection(_connectionString);
				return connection.QueryFirstOrDefault<string?>(
					"SELECT TOP 1 Flusso FROM FasePerOdpConQtaDifformeInFase WHERE RTRIM(Odp) = @Odp AND RTRIM(Fase) = @Fase",
					new { Odp = odp, Fase = fase });
			}
			catch (Exception ex)
			{
				_loggingService.LogError($"Errore nel recupero Flusso per ODP={odp}, Fase={fase}", ex);
				return null;
			}
		}

		public List<string> GetCategorie()
		{
			try
			{
				using var connection = new SqlConnection(_connectionString);
				return connection.Query<string>(
					"SELECT DISTINCT CategoriaDifformita FROM SegnalazioneDifformita WHERE CategoriaDifformita IS NOT NULL AND CategoriaDifformita <> '' AND CategoriaDifformita <> 'Test' ORDER BY CategoriaDifformita")
					.ToList();
			}
			catch (Exception ex)
			{
				_loggingService.LogError("Errore nel recupero categorie difformità", ex);
				return new List<string> { "Mt Materiale", "Tr Trattamenti", "Fn Finiture", "Rilevazione Ok", "Dm Dimensionale", "Quantitativo", "Lg Logistica", "St Strutturale" };
			}
		}
	}
}
