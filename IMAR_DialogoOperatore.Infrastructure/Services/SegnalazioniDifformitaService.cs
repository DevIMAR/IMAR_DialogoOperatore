using IMAR_DialogoOperatore.Application.Interfaces.Services.External;
using IMAR_DialogoOperatore.Application.Interfaces.UoW;
using IMAR_DialogoOperatore.Domain.Entities.Imar_Produzione;

namespace IMAR_DialogoOperatore.Infrastructure.Services
{
    public class SegnalazioniDifformitaService : ISegnalazioniDifformitaService
    {
        private readonly IImarProduzioneUoW _imarProduzioneUoW;
        public SegnalazioniDifformitaService(
            IImarProduzioneUoW imarProduzioneUoW
            )
        {
            _imarProduzioneUoW = imarProduzioneUoW;
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
                throw new Exception(ex.Message);
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
    }
}
