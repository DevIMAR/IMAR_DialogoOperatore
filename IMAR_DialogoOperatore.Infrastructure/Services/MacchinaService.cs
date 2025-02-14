using IMAR_DialogoOperatore.Application.Interfaces.Repositories;
using IMAR_DialogoOperatore.Application.Interfaces.Services.Activities;
using IMAR_DialogoOperatore.Application.Interfaces.UoW;
using IMAR_DialogoOperatore.Domain.Entities.As400;
using IMAR_DialogoOperatore.Domain.Models;

namespace IMAR_DialogoOperatore.Infrastructure.Services
{
    public class MacchinaService : IMacchinaService
    {
        private ISynergyJmesUoW _synergyJmesUoW;
        private readonly IAs400Repository _as400Repository;

        public MacchinaService(
            ISynergyJmesUoW synergyJmesUoW,
            IAs400Repository as400Repository)
        {
            _synergyJmesUoW = synergyJmesUoW;
            _as400Repository = as400Repository;
        }

        public Macchina GetMacchinaByAttivita(Attivita attivita)
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

        public int GetCodiceJmesByCodice(string codiceMacchinaCompleto)
        {
            return (int)_synergyJmesUoW.AngRes.Get(x => x.ResCod == codiceMacchinaCompleto)
                                                        .Select(x => x.Uid)
                                                        .Single();


        }
    }
}
