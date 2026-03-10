using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Application.Interfaces.Clients;
using IMAR_DialogoOperatore.Application.Interfaces.Services.Activities;
using IMAR_DialogoOperatore.Application.Interfaces.UoW;
using IMAR_DialogoOperatore.Application.Interfaces.Utilities;
using IMAR_DialogoOperatore.Domain.Entities.JMES;
using IMAR_DialogoOperatore.Domain.Models;

namespace IMAR_DialogoOperatore.Infrastructure.Services
{
    public class FaseNonPianificataService : IFaseNonPianificataService
    {
        private readonly IJmesApiClient _jmesApiClient;
        private readonly IJMesApiClientErrorUtility _jMesApiClientErrorUtility;
        private readonly ISynergyJmesUoW _synergyJmesUoW;

        public FaseNonPianificataService(
            IJmesApiClient jmesApiClient,
            IJMesApiClientErrorUtility jMesApiClientErrorUtility,
            ISynergyJmesUoW synergyJmesUoW)
        {
            _jmesApiClient = jmesApiClient;
            _jMesApiClientErrorUtility = jMesApiClientErrorUtility;
            _synergyJmesUoW = synergyJmesUoW;
        }

        public async Task<string?> ApriAttrezzaggioFaseNonPianificataAsync(Attivita attivita, Operatore operatore)
        {
            string codiceFase = GetCodiceFase(attivita);

            HttpResponseMessage responseMessage = await _jmesApiClient.MesEquipStartNotPlnAsync(operatore, attivita.Bolla, codiceFase);

            var (errore, jsonData) = await _jMesApiClientErrorUtility.GestioneEventualeErroreAsync(responseMessage);
            if (errore != null)
                return errore;

            if (jsonData != null)
                return jsonData.result.instanceRef.model.diaOpe.evtUid.ToString();

            return null;
        }

        public async Task<string?> ApriLavoroFaseNonPianificataAsync(Attivita attivita, Operatore operatore)
        {
            string codiceFase = GetCodiceFase(attivita);

            HttpResponseMessage responseMessage = await _jmesApiClient.MesWorkStartNotPlnAsync(operatore, attivita, codiceFase);

            var (errore, jsonData) = await _jMesApiClientErrorUtility.GestioneEventualeErroreAsync(responseMessage);
            if (errore != null)
                return errore;

            if (jsonData != null)
                return jsonData.result.instanceRef.model.diaOpe.evtUid.ToString();

            return null;
        }

        private string GetCodiceFase(Attivita attivita)
        {
            string descrizioneFaseNonPianificata = Costanti.PREFISSO_RILAVORAZIONE + " " + attivita.DescrizioneFase;
            AngMesNotPlnLng? angMesNotPlnLng = _synergyJmesUoW.AngMesNotPlnLng.Get(x => x.NotPlnDsc.Equals(descrizioneFaseNonPianificata))
                                                                             .SingleOrDefault();

            angMesNotPlnLng ??= CreaFaseNonPianificata(descrizioneFaseNonPianificata);

            return _synergyJmesUoW.AngMesNotPln.Get(x => x.Uid == angMesNotPlnLng.RecUid).Single().NotPlnCod;
        }

        private AngMesNotPlnLng CreaFaseNonPianificata(string descrizioneFaseNonPianificata)
        {
            AngMesNotPln ultimaFaseRilavorazione = _synergyJmesUoW.AngMesNotPln.Get()
                                                                  .OrderByDescending(x => x.NotPlnCod)
                                                                  .First();

            int codiceNumerico = Int32.Parse(ultimaFaseRilavorazione.NotPlnCod) + 1;

            _synergyJmesUoW.AngMesNotPln.Insert(new AngMesNotPln
            {
                Uid = codiceNumerico,
                NotPlnCod = codiceNumerico.ToString().PadLeft(4, '0'),
                NotPlnIco = ultimaFaseRilavorazione.NotPlnIco,
                LogDel = 0,
                Tsi = DateTime.Now,
                RecVer = 0
            });

            _synergyJmesUoW.Save();

            AngMesNotPlnLng nuovaFaseNonPianificata = new AngMesNotPlnLng
            {
                RecUid = codiceNumerico,
                LngUid = 1,
                NotPlnDsc = descrizioneFaseNonPianificata,
                Tsi = DateTime.Now,
                RecVer = 0
            };

            _synergyJmesUoW.AngMesNotPlnLng.Insert(nuovaFaseNonPianificata);

            _synergyJmesUoW.Save();

            return nuovaFaseNonPianificata;
        }
    }
}
