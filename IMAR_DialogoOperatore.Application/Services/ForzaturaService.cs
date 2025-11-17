using IMAR_DialogoOperatore.Application.Interfaces.Clients;
using IMAR_DialogoOperatore.Application.Interfaces.Repositories;
using IMAR_DialogoOperatore.Application.Interfaces.Services.Activities;
using IMAR_DialogoOperatore.Application.Interfaces.UoW;
using IMAR_DialogoOperatore.Domain.Entities.As400;
using IMAR_DialogoOperatore.Domain.Entities.Imar_Produzione;
using IMAR_DialogoOperatore.Domain.Entities.Imar_Schedulazione;
using IMAR_DialogoOperatore.Domain.Entities.JMES;
using IMAR_DialogoOperatore.Domain.Models;

namespace IMAR_DialogoOperatore.Application.Services
{
    public class ForzaturaService : IForzaturaService
    {
        enum TypeAction
        {
            VECCHIA,
            NUOVA,
            ROTTA
        }

        private readonly ISynergyJmesUoW _synergyJmesUoW;
        private readonly IImarSchedulatoreUoW _imarSchedulatoreUoW;
        private readonly IAs400Repository _as400Repository;
        private readonly IImarApiClient _imarApiClient;

        public ForzaturaService(
            ISynergyJmesUoW synergyJmesUoW,
            IImarSchedulatoreUoW imarSchedulatoreUoW,
            IAs400Repository as400Repository,
            IImarApiClient imarApiClient)
        {
            _synergyJmesUoW = synergyJmesUoW;
            _imarSchedulatoreUoW = imarSchedulatoreUoW;
            _as400Repository = as400Repository;
            _imarApiClient = imarApiClient;
        }

        public void ForzaRigaOrdineDaIdEvento(int idEvento)
        {
            MesEvtDet? evento = _synergyJmesUoW.MesEvtDet.Get(x => x.EvtUid == idEvento).SingleOrDefault();
            if (evento == null)
                return;

            ForzaRigaOrdineDaOdp(evento.PrdOrdCod);
        }

        public void ForzaRigaOrdineDaOdp(string odp)
        {
            List<ODC_ODP> odcOdps = _imarSchedulatoreUoW.OdcOdpRepository.Get(x => x.ODP == odp).ToList();
            if (odcOdps.Count == 0)
                return;

            ForzaRigheOrdine(odcOdps.Select(x => x.ORDINE_RIGA));
        }

        public void ForzaRigheOrdine(IEnumerable<string> righeOrdine)
        {
            List<ORDINE_CLIENTE> ordiniClienti = _imarSchedulatoreUoW.OrdineClienteRepository.Get(x => righeOrdine.Contains(x.ORDINE_RIGA))
                                                                                             .OrderByDescending(x => x.DATA_FINE)
                                                                                             .ToList();

            foreach (ORDINE_CLIENTE ordineCliente in ordiniClienti)
            {
                if (!ordineCliente.DATA_FINE.HasValue)
                    continue;

                ForzaRigaOrdineDaOrdineCliente(ordineCliente);
            }
        }

        public void ForzaRigaOrdineDaOrdineCliente(ORDINE_CLIENTE ordineCliente)
        {
            Forzatura forzatura = new Forzatura()
            {
                Id = Guid.NewGuid(),
                CreationTime = DateTime.Now,
                DataFineLavori = (DateTime)ordineCliente.DATA_FINE,
                RigaOrdine = ordineCliente.ORDINE_RIGA,
                Utente = "DialogoOperatore",
                Attiva = true
            };

            ICollection<OrdineProduzioneForzato> ordiniProduzioneForzati = new List<OrdineProduzioneForzato>();

            List<ODPSchedulazione> odpSchedulazione = GetSchedulazioneRigaOrdine(ordineCliente.ORDINE_RIGA);
            foreach (ODPSchedulazione odp in odpSchedulazione)
            {
                OrdineProduzioneForzato odpForzatoOut = BuildOdpToLog(odp, TypeAction.VECCHIA.ToString(), forzatura);
                ordiniProduzioneForzati.Add(odpForzatoOut);
            }

            forzatura.OrdineProduzioneForzato = ordiniProduzioneForzati;

            AggiungiNuoveFasiDaForzare(ref forzatura);

            _imarApiClient.RegistraForzature(forzatura);
        }

        private List<ODPSchedulazione> GetSchedulazioneRigaOrdine(string rigaOrdine)
        {
            List<FASI> fasi = _imarSchedulatoreUoW.FasiRepository.Get().ToList();
            List<DESCRIZIONE_FASI> descrizioneFasi = _imarSchedulatoreUoW.DescrizioneFasiRepository.Get().ToList();
            List<CAL_FL_ODP> schedulazioneOrdiniProduzione = _imarSchedulatoreUoW.CalFlOdpRepository.Get().ToList();
            List<ODC_ODP> ordiniProduzioneRigheOrdine = _imarSchedulatoreUoW.OdcOdpRepository.Get().ToList();
            List<ODC_ODP> ordiniProduzioneRigaOrdine = ordiniProduzioneRigheOrdine.Where(x => x.ORDINE_RIGA == rigaOrdine).
                                                                                   OrderByDescending(x => x.LIVELLO).ToList();
            ODPSchedulazione ordineProduzione;
            List<ODPSchedulazione> ordiniProduzioneSchedulati = new List<ODPSchedulazione>();
            ORDINE_PRODUZIONE currentOdp;
            foreach (ODC_ODP odp in ordiniProduzioneRigaOrdine)
            {
                currentOdp = _imarSchedulatoreUoW.OrdineProduzioneRepository.Get(x => x.ODP == odp.ODP).SingleOrDefault();
                foreach (CAL_FL_ODP schedulazioneOrdineProduzione in schedulazioneOrdiniProduzione.Where(x => x.ODP == odp.ODP).ToList())
                {
                    foreach (var fase in fasi.Where(x => x.ORDINE_PRODUZIONE_ODP == schedulazioneOrdineProduzione.ODP &&
                                                         x.SEQUENZA == schedulazioneOrdineProduzione.FASE).OrderByDescending(x => x.SEQUENZA).ToList())
                    {
                        ordineProduzione = new ODPSchedulazione()
                        {
                            Codice = odp.ODP,
                            SequenzaFase = fase.SEQUENZA,
                            CodiceFase = fase.CODICE_FASE,
                            DescrizioneFase = descrizioneFasi.Where(x => x.CODICE_FASE == fase.CODICE_FASE).First().DESCRIZIONE,
                            DataArrivoMateriale = fase.DATA_ARRIVO_MATE,
                            InfoDataArrivoMateriale = fase.DATA_MAT_UPDATER,
                            GiornoSchedulazione = schedulazioneOrdineProduzione.GIORNO,
                            Durata = schedulazioneOrdineProduzione.T_ODP ?? 0,
                            Sovraccarico = schedulazioneOrdineProduzione.SOVRACCARICO,
                            Manuale = schedulazioneOrdineProduzione.MANUALE ?? 0,
                            Livello = odp.LIVELLO ?? -1,
                            Flusso = fase.FLUSSO,
                            InFlusso = fase.I_O ?? false,
                            CodiceArticolo = currentOdp.ARTICOLO,
                            QuantitaResidua = currentOdp.QUANTITA_RESIDUA ?? 0
                        };
                        ordiniProduzioneSchedulati.Add(ordineProduzione);
                    }
                }
            }
            ordiniProduzioneSchedulati = ordiniProduzioneSchedulati.OrderByDescending(x => x.GiornoSchedulazione).ThenBy(x => x.Codice).ThenByDescending(x => x.SequenzaFase).ToList();
            int i = 0;
            foreach (var odp in ordiniProduzioneSchedulati)
                odp.IdxForzatura = i++;
            return ordiniProduzioneSchedulati;
        }

        private void AggiungiNuoveFasiDaForzare(ref Forzatura forzatura)
        {
            List<OrdineProduzioneForzato> ordiniProduzioneForzati = new List<OrdineProduzioneForzato>();
            List<PCIMP00F> pCIMP00Fs;
            Forzatura forzaturaTemp = forzatura;
            OrdineProduzioneForzato nuovaFaseDaForzare;
            decimal tempoMacchinaTotale = 0;

            IQueryable<ODC_ODP> odcOdp = _imarSchedulatoreUoW.OdcOdpRepository.Get(x => x.ORDINE_RIGA == forzaturaTemp.RigaOrdine);

            foreach (string odp in odcOdp.Select(x => x.ODP))
            {
                pCIMP00Fs = _as400Repository.ExecuteQuery<PCIMP00F>($"SELECT * " +
                                                                    $"FROM IMA90DAT.PCIMP00F " +
                                                                    $"WHERE ORPRCI = '{odp}'" +
                                                                    $"ORDER BY CDFACI")
                                            .ToList();

                for (int i = 0; i < pCIMP00Fs.Count(); i++)
                {
                    tempoMacchinaTotale += pCIMP00Fs[i].OIAMCI + pCIMP00Fs[i].OILMCI;

                    if (forzatura.OrdineProduzioneForzato.Select(x => x.Fase).Contains(Int32.Parse(pCIMP00Fs[i].CDFACI)))
                        continue;

                    nuovaFaseDaForzare = forzatura.OrdineProduzioneForzato.Single(x => x.Fase == Int32.Parse(pCIMP00Fs[i - 1].CDFACI));
                    nuovaFaseDaForzare.Fase = Int32.Parse(pCIMP00Fs[i].CDFACI);
                    nuovaFaseDaForzare.Action = TypeAction.NUOVA.ToString();

                    ordiniProduzioneForzati.Add(nuovaFaseDaForzare);
                }
            }

            forzatura.OreAllocazioneGiornaliera = CalcolaAllocazioneTempoGiornaliera(tempoMacchinaTotale);
            forzatura.OrdineProduzioneForzato = (ICollection<OrdineProduzioneForzato>)forzatura.OrdineProduzioneForzato.Union(ordiniProduzioneForzati);
        }

        private OrdineProduzioneForzato BuildOdpToLog(ODPSchedulazione odp, string action, Forzatura forzatura)
        {
            return new OrdineProduzioneForzato()
            {
                Id = Guid.NewGuid(),
                OrdineProduzione = odp.Codice,
                Flusso = odp.Flusso,
                Giorno = odp.GiornoSchedulazione,
                Action = action.ToString(),
                CreationDate = forzatura.CreationTime,
                Durata = odp.Durata,
                Fase = odp.SequenzaFase,
                ForzaturaId = forzatura.Id,
                InFlusso = odp.InFlusso,
                Livello = odp.Livello,
                DescrizioneFase = odp.DescrizioneFase,
                DataArrivoMateriale = odp.DataArrivoMateriale
            };
        }

        private string CalcolaAllocazioneTempoGiornaliera(decimal tempoMacchinaTotale) =>
            tempoMacchinaTotale switch
            {
                < 5 => "1",
                >= 5 and < 9 => "2",
                >= 9 and < 16 => "3",
                >= 16 and < 25 => "4",
                >= 25 and < 36 => "5",
                >= 36 and < 45 => "6",
                >= 45 and < 61 => "7",
                >= 60 and < 81 => "8",
                >= 81 => "9"
            };
    }
}
