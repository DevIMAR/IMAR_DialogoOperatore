using IMAR_DialogoOperatore.Application.Interfaces.Clients;
using IMAR_DialogoOperatore.Application.Interfaces.Repositories;
using IMAR_DialogoOperatore.Application.Interfaces.Services.Activities;
using IMAR_DialogoOperatore.Application.Interfaces.UoW;
using IMAR_DialogoOperatore.Domain.Entities.As400;
using IMAR_DialogoOperatore.Domain.Entities.JMES;
using IMAR_DialogoOperatore.Domain.Models;

namespace IMAR_DialogoOperatore.Infrastructure.Services
{
    public class MacchinaService : IMacchinaService
    {
        private readonly ISynergyJmesUoW _synergyJmesUoW;
        private readonly IAs400Repository _as400Repository;
        private readonly IJmesApiClient _jmesApiClient;

        public MacchinaService(
            ISynergyJmesUoW synergyJmesUoW,
            IAs400Repository as400Repository,
            IJmesApiClient jmesApiClient)
        {
            _synergyJmesUoW = synergyJmesUoW;
            _as400Repository = as400Repository;
            _jmesApiClient = jmesApiClient;
        }

        public Macchina GetMacchinaRealeByAttivita(Attivita attivita)
        {
            PCIMP00F? pCIMP00F = _as400Repository.ExecuteQuery<PCIMP00F>($"SELECT * FROM IMA90DAT.PCIMP00F WHERE NRBLCI = '{attivita.Bolla}'").SingleOrDefault();
            if (pCIMP00F == null)
                return null;

            int codiceJmes = (int)_synergyJmesUoW.AngRes.Get(x => x.ResCod == pCIMP00F.CDCLCI + pCIMP00F.CDMUCI)
                                                        .Select(x => x.Uid)
                                                        .Single();

            return new Macchina
            {
                CentroDiLavoro = pCIMP00F.CDCLCI,
                CodiceMacchina = pCIMP00F.CDMUCI,
                CodiceJMes = codiceJmes
            };
        }

        public Macchina? GetMacchinaFittiziaByFirstAttivitaAperta(Attivita attivitaAperta, int idJMesOperatore)
        {
            mesEvtToEndMac? macchinaFittiziaConAttivitaOperatoreAperta = _jmesApiClient.ChiamaQueryGetJmes<mesEvtToEndMac>()?
                                                                                       .FirstOrDefault(x => x.ID_Det3350.Trim() == attivitaAperta.Bolla.ToString() && 
                                                                                                             x.ID_Evt3245 == idJMesOperatore);
            if (macchinaFittiziaConAttivitaOperatoreAperta == null)
                return null;

            return new Macchina
            {
                CentroDiLavoro = macchinaFittiziaConAttivitaOperatoreAperta.ID_Mac368.Substring(0, 3),
                CodiceMacchina = macchinaFittiziaConAttivitaOperatoreAperta.ID_Mac368.Substring(3, 3),
                CodiceJMes = macchinaFittiziaConAttivitaOperatoreAperta.ID_Mac365
            };
        }

        public Macchina? GetPrimaMacchinaFittiziaNonUtilizzata()
        {
            IEnumerable<AngRes> macchineFittizie = _synergyJmesUoW.AngRes.Get(x => x.ResDsc.Contains("FITTIZIA"));

            IEnumerable<mesEvtToEndMac>? macchineFittizieConAttivitaAperte = _jmesApiClient.ChiamaQueryGetJmes<mesEvtToEndMac>()?.Where(x => x.ID_Mac371.Contains("FITTIZIA"));
            if (macchineFittizieConAttivitaAperte == null || !macchineFittizieConAttivitaAperte.Any())
                return new Macchina
                {
                    CentroDiLavoro = "999",
                    CodiceMacchina = "001",
                    CodiceJMes = 491
                };

            foreach (AngRes macchina in macchineFittizie)
            {
                if (!macchineFittizieConAttivitaAperte.Any(x => x.ID_Mac365 == macchina.Uid))
                {
                    return new Macchina
                    {
                        CentroDiLavoro = macchina.ResCod.Trim().Substring(0, 3),
                        CodiceMacchina = macchina.ResCod.Trim().Substring(3, 3),
                        CodiceJMes = (int)macchina.Uid
                    };
                }
            }

            return null;
        }

        public int GetCodiceJmesByCodice(string codiceMacchinaCompleto)
        {
            return (int)_synergyJmesUoW.AngRes.Get(x => x.ResCod == codiceMacchinaCompleto)
                                                        .Select(x => x.Uid)
                                                        .Single();
        }
    }
}
