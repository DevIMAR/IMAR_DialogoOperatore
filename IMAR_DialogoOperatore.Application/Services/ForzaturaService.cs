using IMAR_DialogoOperatore.Application.Interfaces.Clients;
using IMAR_DialogoOperatore.Application.Interfaces.Repositories;
using IMAR_DialogoOperatore.Application.Interfaces.Services.Activities;
using IMAR_DialogoOperatore.Application.Interfaces.UoW;
using IMAR_DialogoOperatore.Domain.DTO;
using IMAR_DialogoOperatore.Domain.Entities.As400;
using IMAR_DialogoOperatore.Domain.Entities.Imar_Produzione;
using IMAR_DialogoOperatore.Domain.Entities.Imar_Schedulazione;
using IMAR_DialogoOperatore.Domain.Entities.JMES;
using IMAR_DialogoOperatore.Domain.Models;
using Microsoft.EntityFrameworkCore;

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

        private decimal _oreAllocazioneGiornaliera;
        private List<ODPSchedulazione> _schedulazioneAttuale;
        private List<ODPSchedulazione> _schedulazionePreview;
        private ForzaturaDTO _forzaturaDTO;


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

        public async Task ForzaRigaOrdineDaIdEvento(int idEvento)
        {
            MesEvtDet? evento = _synergyJmesUoW.MesEvtDet.Get(x => x.EvtUid == idEvento).SingleOrDefault();
            if (evento == null)
                return;

            await ForzaRigaOrdineDaOdp(evento.PrdOrdCod);
        }

        public async Task ForzaRigaOrdineDaOdp(string odp)
        {
            List<ODC_ODP> odcOdps = _imarSchedulatoreUoW.OdcOdpRepository.Get(x => x.ODP == odp).ToList();
            if (odcOdps.Count == 0)
                return;

            await ForzaRigheOrdine(odcOdps.Select(x => x.ORDINE_RIGA));
        }

        public async Task ForzaRigheOrdine(IEnumerable<string> righeOrdine)
        {
            List<ORDINE_CLIENTE> ordiniClienti = _imarSchedulatoreUoW.OrdineClienteRepository.Get(x => righeOrdine.Contains(x.ORDINE_RIGA))
                                                                                             .OrderByDescending(x => x.DATA_FINE)
                                                                                             .ToList();

            foreach (ORDINE_CLIENTE ordineCliente in ordiniClienti)
            {
                if (!ordineCliente.DATA_FINE.HasValue)
                    continue;

                await ForzaRigaOrdineDaOrdineCliente(ordineCliente);
            }
        }

        public async Task ForzaRigaOrdineDaOrdineCliente(ORDINE_CLIENTE ordineCliente)
        {
            ICollection<OrdineProduzioneForzato> ordiniProduzioneForzati = new List<OrdineProduzioneForzato>();

			_schedulazioneAttuale = await GetAttualeSchedulazione(ordineCliente.ORDINE_RIGA);

			await AggiungiNuoveFasiDaForzare(ordineCliente.ORDINE_RIGA);

			_schedulazionePreview = await GetPreviewSchedulazione(ordineCliente.ORDINE_RIGA, _schedulazioneAttuale);

			await _imarApiClient.RimuoviSchedulazioneAttuale("101", _schedulazioneAttuale);

			await _imarApiClient.InserisciNuovaSchedulazione(_forzaturaDTO.Forzatura, ordineCliente.ORDINE_RIGA, _schedulazioneAttuale.Max(x => x.GiornoSchedulazione));

            await RegistraForzatura(ordineCliente);
		}

		private async Task<List<ODPSchedulazione>> GetAttualeSchedulazione(string rigaOrdine)
		{
			var schedulazioneAttualeRiga = await _imarApiClient.GetSchedulazioneAttuale(rigaOrdine);
			foreach (var allocazione in schedulazioneAttualeRiga)
			{
				allocazione.DataArrivoMateriale = allocazione.DataArrivoMateriale?.Year > 1
													? allocazione.DataArrivoMateriale : null;
			}

			return schedulazioneAttualeRiga;
		}

		private async Task<List<ODPSchedulazione>> GetPreviewSchedulazione(string rigaOrdine, List<ODPSchedulazione> odpSchedulazione)
		{
			var preview = new List<ODPSchedulazione>();
            DateTime massimoGiornoSchedulazione = odpSchedulazione.Max(x => x.GiornoSchedulazione);
            double durataLavorazioni = odpSchedulazione.Sum(x => x.Durata);

            _oreAllocazioneGiornaliera = decimal.Parse(CalcolaAllocazioneTempoGiornaliera(massimoGiornoSchedulazione, durataLavorazioni).ToString());

			_forzaturaDTO = await _imarApiClient.GetPreviewForzatura(rigaOrdine, 
                                                            massimoGiornoSchedulazione.ToString("ddMMyyyy"),
															_oreAllocazioneGiornaliera);


			List<GiornoSchedulazione> giorniSchedulazione = _forzaturaDTO.Forzatura;
			foreach (var giorno in giorniSchedulazione)
			{
				preview.AddRange(giorno.OrdiniProduzioneInFlusso);
				preview.AddRange(giorno.OrdiniProduzioneNonFlusso);
			}

			foreach (var allocazione in preview)
			{
				allocazione.DataArrivoMateriale = allocazione.DataArrivoMateriale?.Year > 1
													? allocazione.DataArrivoMateriale : null;
			}

			return preview;
		}

        private async Task AggiungiNuoveFasiDaForzare(string oridneRiga)
        {
            List<PCIMP00F> pCIMP00Fs;
            ODPSchedulazione fasePrecedente;
            bool faseAggiunta = false;

			List<ODC_ODP> odcOdp = _imarSchedulatoreUoW.OdcOdpRepository.Get(x => x.ORDINE_RIGA == oridneRiga).ToList();

            foreach (string odp in odcOdp.Select(x => x.ODP))
            {
                pCIMP00Fs = _as400Repository.ExecuteQuery<PCIMP00F>($"SELECT * " +
                                                                    $"FROM IMA90DAT.PCIMP00F " +
                                                                    $"WHERE ORPRCI = '{odp}'" +
                                                                    $"ORDER BY CDFACI")
                                            .ToList();

                for (int i = 1; i < pCIMP00Fs.Count(); i++)
                {
                    if (_schedulazioneAttuale.Select(x => x.SequenzaFase).Contains(Int32.Parse(pCIMP00Fs[i].CDFACI)))
                        continue;

                    fasePrecedente = _schedulazioneAttuale.Single(x => x.SequenzaFase == Int32.Parse(pCIMP00Fs[i - 1].CDFACI));

                    await RegistraNuovaFaseInSchedulatore(fasePrecedente, Int32.Parse(pCIMP00Fs[i].CDFACI));
                    faseAggiunta = true;

                    break;
                }

                if (faseAggiunta)
                    break;
            }
		}

		private async Task RegistraForzatura(ORDINE_CLIENTE ordineCliente)
		{
			Forzatura forzatura = new Forzatura()
			{
				Id = Guid.NewGuid(),
				CreationTime = DateTime.Now,
				DataFineLavori = (DateTime)ordineCliente.DATA_FINE,
				RigaOrdine = ordineCliente.ORDINE_RIGA,
				OreAllocazioneGiornaliera = _oreAllocazioneGiornaliera.ToString(),
				Utente = "DialogoOperatore",
				Attiva = true
			};

            OrdineProduzioneForzato odpForzatoIn;

			ICollection<OrdineProduzioneForzato> OrdiniProduzioneForzati = new List<OrdineProduzioneForzato>();
			foreach (var odp in _schedulazioneAttuale)
				OrdiniProduzioneForzati.Add(BuildOdpToLog(odp, TypeAction.VECCHIA.ToString(), forzatura));

			foreach (var odp in _schedulazionePreview)
				OrdiniProduzioneForzati.Add(BuildOdpToLog(odp, TypeAction.NUOVA.ToString(), forzatura));

			forzatura.OrdineProduzioneForzato = OrdiniProduzioneForzati;
			await _imarApiClient.RegistraForzature(forzatura);
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

        private double CalcolaAllocazioneTempoGiornaliera(DateTime giornoSchedulazione, double tempoMacchinaTotale) =>
            Math.Ceiling(tempoMacchinaTotale / giornoSchedulazione.Date.Subtract(DateTime.Today).Days);

        private async Task RegistraNuovaFaseInSchedulatore(ODPSchedulazione fasePrecedente, int sequenzaNuovaFase)
        {
            FASI faseSrc = _imarSchedulatoreUoW.FasiRepository
                .Get(f => f.ORDINE_PRODUZIONE_ODP == fasePrecedente.Codice
						  && f.SEQUENZA == fasePrecedente.SequenzaFase)
                .AsNoTracking()
                .Single();

            FASI nuovaFase = faseSrc with
            {
                SEQUENZA = sequenzaNuovaFase,
                CAL_FL_ODP = new List<CAL_FL_ODP>()
            };

            _imarSchedulatoreUoW.FasiRepository.Insert(nuovaFase);
            await _imarSchedulatoreUoW.SaveAsync();
        }
    }
}
