using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Application.DTOs;
using IMAR_DialogoOperatore.Application.Interfaces.Clients;
using IMAR_DialogoOperatore.Application.Interfaces.Repositories;
using IMAR_DialogoOperatore.Application.Interfaces.Services.Activities;
using IMAR_DialogoOperatore.Application.Interfaces.UoW;
using IMAR_DialogoOperatore.Application.Interfaces.Utilities;
using IMAR_DialogoOperatore.Domain.Entities.JMES;
using IMAR_DialogoOperatore.Domain.Models;
using IMAR_DialogoOperatore.Infrastructure.Mappers;
using IMAR_DialogoOperatore.Infrastructure.Services;
using IMAR_DialogoOperatore.Infrastructure.Utilities;


namespace IMAR_DialogoOperatore.Services
{
    public class AttivitaService : IAttivitaService
    {
        private readonly IMacchinaService _macchinaService;
        private readonly IJmesApiClient _jmesApiClient;
        private readonly IJMesApiClientErrorUtility _jMesApiClientErrorUtility;
        private readonly CaricamentoAttivitaInBackgroundService _caricamentoAttivitaInBackroundService;
        private readonly ISynergyJmesUoW _synergyJmesUoW;
        private readonly IAs400Repository _as400Repository;

        public AttivitaService(
            IMacchinaService macchinaService,
            IJmesApiClient jmesApiClient,
            IJMesApiClientErrorUtility jMesApiClientErrorUtility,
            CaricamentoAttivitaInBackgroundService caricamentoAttivitaInBackroundService,
            ISynergyJmesUoW synergyJmesUoW,
            IAs400Repository as400Repository)
        {
            _macchinaService = macchinaService;

            _jmesApiClient = jmesApiClient;
            _jMesApiClientErrorUtility = jMesApiClientErrorUtility;

            _caricamentoAttivitaInBackroundService = caricamentoAttivitaInBackroundService;

            _as400Repository = as400Repository;
            _synergyJmesUoW = synergyJmesUoW;
        }

        public bool ConfrontaCausaliAttivita(IList<Attivita> listaAttivitaDaControllare, string bollaAttivitaDaConfrontare, string operazioneAttivitaDaConfrontare)
        {
            if (listaAttivitaDaControllare == null || listaAttivitaDaControllare.Count == 0)
                return false;

            Attivita? attivitaSelezionataDallaLista = listaAttivitaDaControllare.SingleOrDefault(x => x.Bolla == bollaAttivitaDaConfrontare);

            if (attivitaSelezionataDallaLista == null)
                return false;

            string causaleAttivitaSelezionata = attivitaSelezionataDallaLista.Causale;
            string operazioneInCorso = operazioneAttivitaDaConfrontare;

            if (causaleAttivitaSelezionata == operazioneInCorso)
                return false;

            return true;
        }

        public Attivita? CercaAttivitaDaBolla(string bolla)
        {
            Attivita? attivita = null;

            if (bolla.Length == 5)
                attivita = OttieniAttivitaIndirettaDaBolla(bolla);
            else
                attivita = OttieniAttivitaApertaDaBolla(bolla);

            return attivita;
        }

        private Attivita? OttieniAttivitaIndirettaDaBolla(string bolla)
        {
            stdMesIndTsk? stdMesIndTsk = _caricamentoAttivitaInBackroundService.GetAttivitaIndirette()
                                                    .SingleOrDefault(x => Int32.Parse(x.ID_Ind3463) == Int32.Parse(bolla));
            if (stdMesIndTsk == null)
                return null;

            return CreateNuovaAttivitaIndiretta(stdMesIndTsk);
        }

        private Attivita? CreateNuovaAttivitaIndiretta(stdMesIndTsk stdMesIndTsk)
        {
            Attivita nuovaAttivita = new Attivita
            {
                Bolla = stdMesIndTsk.ID_Ind3463,
                DescrizioneFase = stdMesIndTsk.ID_Ind3464
            };

            return nuovaAttivita;
        }

        private Attivita? OttieniAttivitaApertaDaBolla(string bolla)
        {
            Attivita? attivitaTrovata = _caricamentoAttivitaInBackroundService.GetAttivitaAperte()
                                                    .SingleOrDefault(x => x.Bolla == bolla);

            if (attivitaTrovata == null)
            {
                // Fetch tutte le fasi dello stesso ODP (necessario per il calcolo QuantitaOrdine in C#)
                var tutteLeFasiOdp = _as400Repository.ExecuteQuery<Attivita>(@"WITH
																		    NONCONTABILIZZATE AS (
																			    SELECT
																				    NRTSKJM,
																				    SUM(QTVERJM) AS TOT_QTA_NC,
																				    SUM(QTSCAJM) AS TOT_SCARTO_NC,
																				    SUM(QTNCOJM) AS TOT_NONCONF_NC,
																				    MAX(SAACCJM) AS MAX_SAACCJM
																			    FROM IMA90DAT.JMRILM00F
																			    WHERE QCONTJM = ''
																			    GROUP BY NRTSKJM
																		    )
																		    SELECT
																			    pf2.NRBLCI AS BOLLA,
																			    pf2.ORPRCI AS ODP,
																			    pf2.CDARCI AS ARTICOLO,
																			    TRIM(mf.DSARMA) AS DESCRIZIONEARTICOLO,
																			    pf2.CDFACI AS FASE,
																			    pf2.DSFACI AS DESCRIZIONEFASE,
																			    pf2.QORDCI AS QUANTITAORDINEORIGINALE,
																			    0 AS QUANTITAORDINE,
																			    COALESCE(ra.TOT_QTA_NC, 0) AS QUANTITAPRODOTTANONCONTABILIZZATA,
																			    pf2.QPROCI AS QUANTITAPRODOTTACONTABILIZZATA,
																			    COALESCE(ra.TOT_SCARTO_NC, 0) AS QUANTITASCARTATANONCONTABILIZZATA,
																			    pf2.QSCACI AS QUANTITASCARTATACONTABILIZZATA,
																			    COALESCE(ra.TOT_NONCONF_NC, 0) AS QTANONCONFORMENONCONTABILIZZATA,
																			    pf2.QRESCI AS QTANONCONFORMECONTABILIZZATA,
																			    pf2.TIRECI AS TIPORICEVIMENTO,
																			    CASE WHEN ra.MAX_SAACCJM IS NULL THEN pf2.TIRECI ELSE ra.MAX_SAACCJM END AS SALDOACCONTO,
																			    ra.MAX_SAACCJM AS SALDOACCONTOJMES,
																			    pf2.FLSFCI AS ISNONPIANIFICATA
																		    FROM IMA90DAT.PCIMP00F pf2
																		    JOIN IMA90DAT.MGART00F mf ON pf2.CDARCI = mf.CDARMA
																		    LEFT JOIN NONCONTABILIZZATE ra ON ra.NRTSKJM = pf2.NRBLCI
																		    WHERE pf2.ORPRCI = (SELECT ORPRCI FROM IMA90DAT.PCIMP00F WHERE NRBLCI = '" + bolla + "')").ToList();

                QuantitaOrdineCalculator.Calcola(tutteLeFasiOdp);
                attivitaTrovata = tutteLeFasiOdp.SingleOrDefault(x => x.Bolla == bolla);
            }

            return attivitaTrovata;
        }

        public IEnumerable<Attivita> GetAttivitaPerOdp(string odp)
        {
            IEnumerable<Attivita> attivitaFiltrate = _caricamentoAttivitaInBackroundService.GetAttivitaAperte()
                                                                .Where(x => x.Odp == odp);

            if (attivitaFiltrate == null || !attivitaFiltrate.Any())
            {
                var risultati = _as400Repository.ExecuteQuery<Attivita>(@"WITH
																		    NONCONTABILIZZATE AS (
																			    SELECT
																				    NRTSKJM,
																				    SUM(QTVERJM) AS TOT_QTA_NC,
																				    SUM(QTSCAJM) AS TOT_SCARTO_NC,
																				    SUM(QTNCOJM) AS TOT_NONCONF_NC,
																				    MAX(SAACCJM) AS MAX_SAACCJM
																			    FROM IMA90DAT.JMRILM00F
																			    WHERE QCONTJM = ''
																			    GROUP BY NRTSKJM
																		    )
																		    SELECT
																			    pf2.NRBLCI AS BOLLA,
																			    pf2.ORPRCI AS ODP,
																			    pf2.CDARCI AS ARTICOLO,
																			    TRIM(mf.DSARMA) AS DESCRIZIONEARTICOLO,
																			    pf2.CDFACI AS FASE,
																			    pf2.DSFACI AS DESCRIZIONEFASE,
																			    pf2.QORDCI AS QUANTITAORDINEORIGINALE,
																			    0 AS QUANTITAORDINE,
																			    COALESCE(ra.TOT_QTA_NC, 0) AS QUANTITAPRODOTTANONCONTABILIZZATA,
																			    pf2.QPROCI AS QUANTITAPRODOTTACONTABILIZZATA,
																			    COALESCE(ra.TOT_SCARTO_NC, 0) AS QUANTITASCARTATANONCONTABILIZZATA,
																			    pf2.QSCACI AS QUANTITASCARTATACONTABILIZZATA,
																			    COALESCE(ra.TOT_NONCONF_NC, 0) AS QTANONCONFORMENONCONTABILIZZATA,
																			    pf2.QRESCI AS QTANONCONFORMECONTABILIZZATA,
																			    pf2.TIRECI AS TIPORICEVIMENTO,
																			    CASE WHEN ra.MAX_SAACCJM IS NULL THEN pf2.TIRECI ELSE ra.MAX_SAACCJM END AS SALDOACCONTO,
																			    ra.MAX_SAACCJM AS SALDOACCONTOJMES,
																			    pf2.FLSFCI AS ISNONPIANIFICATA
																		    FROM IMA90DAT.PCIMP00F pf2
																		    JOIN IMA90DAT.MGART00F mf ON pf2.CDARCI = mf.CDARMA
																		    LEFT JOIN NONCONTABILIZZATE ra ON ra.NRTSKJM = pf2.NRBLCI
																		    WHERE pf2.ORPRCI = '" + odp + "'").ToList();

                QuantitaOrdineCalculator.Calcola(risultati);
                attivitaFiltrate = risultati;
            }

            return attivitaFiltrate.OrderBy(x => x.Fase);
        }

        public async Task<string?> AvanzaAttivitaAsync(Operatore operatore, Attivita attivitaDaAvanzare, int quantitaProdotta, int quantitaScartata)
        {
            if (attivitaDaAvanzare.SaldoAcconto != "S")
                attivitaDaAvanzare.QuantitaResidua = attivitaDaAvanzare.QuantitaOrdine - attivitaDaAvanzare.QuantitaProdottaNonContabilizzata;


            HttpResponseMessage result = await _jmesApiClient.MesAdvanceDeclarationAsync(operatore, attivitaDaAvanzare, quantitaProdotta, quantitaScartata);

            var (errore, _) = await _jMesApiClientErrorUtility.GestioneEventualeErroreAsync(result);
            if (errore != null)
                return errore;

            return null;
        }

        public async Task<IList<Attivita>> OttieniAttivitaOperatoreAsync(Operatore operatore)
        {
            IList<mesTskForOpe>? attivitaIndiretteOperatore = await OttieniAttivitaIndiretteOperatoreAsync(operatore.Badge);
            if (attivitaIndiretteOperatore == null)
                return new List<Attivita>();

            IList<mesDiaOpe>? attivitaAperte = await GetAttivitaAperteAsync();
            if (attivitaAperte == null)
                return new List<Attivita>();

            IList<Attivita> attivitaOperatoreAperte = GetAttivitaOperatoreAperte(operatore, attivitaAperte, attivitaIndiretteOperatore);

            return attivitaOperatoreAperte.OrderBy(x => x.Bolla).ToList();
        }

        public async Task<IList<mesDiaOpe>?> GetAttivitaAperteAsync()
        {
            return await _jmesApiClient.ChiamaQueryGetJmesAsync<mesDiaOpe>();
        }

        private async Task<IList<mesTskForOpe>?> OttieniAttivitaIndiretteOperatoreAsync(string badgeOperatore)
        {
            IList<mesTskForOpe>? attivitaOperatore = await _jmesApiClient.ChiamaQueryGetJmesAsync<mesTskForOpe>();
            if (attivitaOperatore == null)
                return null;

            attivitaOperatore = attivitaOperatore.Where(x => x.ID_Mac368.TrimStart('0') == badgeOperatore.TrimStart('0')).ToList(); //ID_Mac368 è il badge operatore nella query di SanMarco

            return attivitaOperatore;
        }

        private List<Attivita> GetAttivitaOperatoreAperte(Operatore operatore, IList<mesDiaOpe> attivitaAperte, IList<mesTskForOpe>? attivitaIndiretteOperatore)
        {
            List<Attivita> attivitaOperatoreAperte = new List<Attivita>();

            List<Attivita> attivitaOperatore = GetAttivitaOperatore(operatore.IdJMes);
            if (attivitaOperatore != null)
                attivitaOperatoreAperte.AddRange(attivitaOperatore);

            if (attivitaIndiretteOperatore != null)
                attivitaOperatoreAperte.AddRange(OttieniAttivitaIndiretteOperatoreAperte(attivitaAperte, attivitaIndiretteOperatore));

            foreach (Attivita attivita in attivitaOperatoreAperte)
                attivita.MacchinaReale = _macchinaService.GetMacchinaRealeByAttivita(attivita);

            return attivitaOperatoreAperte;
        }

        private List<Attivita> GetAttivitaOperatore(int idJmesOperatore)
        {
            List<Attivita> attivitaOperatore = _synergyJmesUoW.MesEvtOpe
                                                               .Get(x => (int)x.ResUid == idJmesOperatore)
                                                               .Where(x => x.TssEnd == null)
                                                               .Join(_synergyJmesUoW.MesEvt.Get(),
                                                                        meo => meo.EvtUid,
                                                                        me => me.Uid,
                                                                        (meo, me) => me)
                                                               .Join(_synergyJmesUoW.MesEvtDet.Get(),
                                                                        me => me.Uid,
                                                                        med => med.EvtUid,
                                                                        (me, med) => new { me, med })
                                                               .Join(_synergyJmesUoW.MesDiaOpe.Get(),
                                                                        x => (double?)x.me.Uid,
                                                                        mdo => (double?) mdo.EvtUid,
                                                                        (x, mdo) => new Attivita
                                                                        {
                                                                            Bolla = x.med.DocCod,
                                                                            Causale = StatoAttivitaMapper.FromJMesCode(x.me.EvtTypUid),
                                                                            CausaleEstesa = StatoAttivitaMapper.FromJMesCodeExtended(x.me.EvtTypUid),
                                                                            CodiceJMes = (double) mdo.Uid,
                                                                            InizioAttivita = x.me.TssStr,
                                                                            FineAttivita = x.me.TssEnd
                                                                        })
                                                               .ToList();

            List<Attivita> attivita = _caricamentoAttivitaInBackroundService.GetAttivitaAperte()
                                       .Join(attivitaOperatore,
                                             a => a.Bolla,
                                             aoa => aoa.Bolla,
                                             (a, aoa) =>
                                             {
                                                 a.CausaleEstesa = aoa.CausaleEstesa;
                                                 a.Causale = aoa.Causale;
                                                 a.CodiceJMes = aoa.CodiceJMes;
                                                 a.InizioAttivita = aoa.InizioAttivita;
                                                 a.FineAttivita = aoa.FineAttivita;
                                                 return a;
                                             })
                                       .ToList();

            return attivita;
        }

        private List<Attivita> OttieniAttivitaIndiretteOperatoreAperte(IList<mesDiaOpe> attivitaAperte, IList<mesTskForOpe> attivitaIndiretteOperatore)
        {
            List<Attivita> attivitaOperatoreAperte = attivitaAperte
                                                    .Join(attivitaIndiretteOperatore,
                                                            aa => aa.ID_Evt3240,
                                                            ao => ao.ID_Evt3240,
                                                            (aa, ao) => new Attivita
                                                            {
                                                                Bolla = aa.ID_Det3350,
                                                                DescrizioneArticolo = Costanti.FASE_INDIRETTA,
                                                                DescrizioneFase = aa.ID_Det3356,
                                                                CodiceJMes = aa.ID_Det3348,
                                                                CausaleEstesa = aa.ID_Sts3130,
                                                                Causale = StatoAttivitaMapper.FromJMesStatus(aa.ID_Sts3130),
                                                                SaldoAcconto = "A",
                                                                IsIndiretta = true
                                                            })
                                                    .Where(x => x.Causale == Costanti.IN_LAVORO ||
                                                                x.Causale == Costanti.IN_ATTREZZAGGIO ||
                                                                x.Causale == Costanti.LAVORO_SOSPESO ||
                                                                x.Causale == Costanti.ATTREZZAGGIO_SOSPESO)
                                                    .ToList();
            return attivitaOperatoreAperte;
        }

        public IList<Attivita> GetAttivitaIndirette()
        {
            IList<Attivita> attivitaIndirette = new List<Attivita>();

            IList<stdMesIndTsk> stdMesIndTsks = _caricamentoAttivitaInBackroundService.GetAttivitaIndirette();

            foreach (stdMesIndTsk stdMesIndTsk in stdMesIndTsks)
            {
                attivitaIndirette.Add(new Attivita
                {
                    Bolla = stdMesIndTsk.ID_Ind3463,
                    DescrizioneArticolo = Costanti.FASE_INDIRETTA,
                    DescrizioneFase = stdMesIndTsk.ID_Ind3464,
                    IsIndiretta = true
                });
            }

            return attivitaIndirette;
        }

        public string? GetNomeOperatoreFasePrecedente(string bollaFasePrecedente)
        {
            if (string.IsNullOrWhiteSpace(bollaFasePrecedente))
                return null;

            // 1. Cerca in PCMOV12L (contabilizzato)
            string? codiceOperatore = _as400Repository.ExecuteQuery<string>(
                @"SELECT MAX(TRIM(CDDICM)) FROM IMA90DAT.PCMOV12L
                  WHERE CDDTCM = '01' AND TRIM(NRBLCM) = '" + bollaFasePrecedente.Trim() + "'")
                .FirstOrDefault();

            // 2. Fallback JMRILM00F (non contabilizzato)
            if (string.IsNullOrWhiteSpace(codiceOperatore))
            {
                codiceOperatore = _as400Repository.ExecuteQuery<string>(
                    @"SELECT MAX(TRIM(CDOPEJM)) FROM IMA90DAT.JMRILM00F
                      WHERE TRIM(NRTSKJM) = '" + bollaFasePrecedente.Trim() + "'")
                    .FirstOrDefault();
            }

            if (string.IsNullOrWhiteSpace(codiceOperatore))
                return null;

            // 3. Risolvi nome da AngRes (synergyJMES)
            var operatore = _synergyJmesUoW.AngRes
                .Get(x => x.ResCod == codiceOperatore)
                .FirstOrDefault();

            if (operatore != null)
                return (operatore.ResNam?.Trim() + " " + operatore.ResSur?.Trim()).Trim();

            return codiceOperatore;
        }

        public async Task<List<string>> GetIdOperatoriConBollaApertaAsync(string bolla)
        {
            IList<mesDiaOpe>? attivitaAperte = await GetAttivitaAperteAsync();
            IList<mesRcpActOpe>? riepilogoAttivitaOperatore = await _jmesApiClient.ChiamaQueryGetJmesAsync<mesRcpActOpe>();

            return attivitaAperte
                   .Join(riepilogoAttivitaOperatore,
                         aa => aa.ID_Det3350,
                         ao => ao.ID_EvtDetLst3309,
                         (aa, ao) => new { aa.ID_Det3350, ao.ID_EvtOpe3279 })?
                   .Where(x => x.ID_Det3350 == bolla)
                   .Distinct()
                   .Select(x => x.ID_EvtOpe3279.ToString())
                   .ToList();
        }

        public IList<Attivita>? GetAttivitaOperatoreDellUltimaGiornata(int idJmesOperatore)
		{
            List<Attivita> attivitaOperatoreDellUltimaGiornata = new List<Attivita>();
			DateTime ieriAlle2045 = DateTime.Today.AddHours(-4).AddMinutes(45);
			DateTime oggiAlle2115 = DateTime.Today.AddHours(21).AddMinutes(15);

			attivitaOperatoreDellUltimaGiornata.AddRange(GetAttivitaOperatoreDellUltimaGiornataNonContabilizzate(idJmesOperatore, ieriAlle2045, oggiAlle2115));
			attivitaOperatoreDellUltimaGiornata.AddRange(GetAttivitaOperatoreDellUltimaGiornataContabilizzate(idJmesOperatore, ieriAlle2045, oggiAlle2115));

			return attivitaOperatoreDellUltimaGiornata;
		}

		private IList<Attivita> GetAttivitaOperatoreDellUltimaGiornataNonContabilizzate(int idJmesOperatore, DateTime ieriAlle2045, DateTime oggiAlle2115)
		{
			IList<Attivita> attivitaOperatoreDellUltimaGiornata = _synergyJmesUoW.MesEvt
																				 .Get(x => (int)x.ResEffStrUid == idJmesOperatore)
																				 .Where(x => x.TssStr >= ieriAlle2045 &&
																							 x.TssStr <= oggiAlle2115 &&
																							 x.EvtTypUid != 3) //evito le sospensioni
																				 .Join(_synergyJmesUoW.MesEvtDet.Get(),
																					   me => me.Uid,
																					   med => med.EvtUid,
																					   (me, med) => new { me, med })
																				 .Join(_synergyJmesUoW.MesEvtMacDet.Get(),
																					   x => x.med.Uid,
																					   memd => memd.EvtDetUid,
																					   (x, memd) =>
																					   new Attivita
																					   {
																						   CodiceJMes = (double?)x.me.Uid,
																						   CausaleEstesa = StatoAttivitaMapper.FromJMesCodeExtended(x.me.EvtTypUid),
																						   Bolla = x.med.DocCod,
																						   Odp = x.med.PrdOrdCod,
																						   Fase = x.med.PrdPhsCod,
																						   QuantitaProdotta = (int)memd.QtyPrd,
																						   QuantitaScartata = (int)memd.QtyRej,
																						   InizioAttivita = x.me.TssStr,
																						   FineAttivita = x.me.TssEnd,
																						   SaldoAcconto = memd.DecAdv == -1 ? Costanti.ACCONTO : memd.DecAdv == 0 ? Costanti.SALDO : ""
																					   })
																				 .ToList();
			return attivitaOperatoreDellUltimaGiornata;
		}

		private IList<Attivita> GetAttivitaOperatoreDellUltimaGiornataContabilizzate(int idJmesOperatore, DateTime ieriAlle2045, DateTime oggiAlle2115)
		{
			IList<Attivita> attivitaOperatoreDellUltimaGiornata = _synergyJmesUoW.MesBckEvt
																				 .Get(x => (int)x.ResEffStrUid == idJmesOperatore)
																				 .Where(x => x.TssStr >= ieriAlle2045 &&
																							 x.TssStr <= oggiAlle2115 &&
																							 x.EvtTypUid != 3) //evito le sospensioni
																				 .Join(_synergyJmesUoW.MesBckEvtDet.Get(),
																					   me => me.Uid,
																					   med => med.EvtUid,
																					   (me, med) => new { me, med })
																				 .Join(_synergyJmesUoW.MesBckEvtMacDet.Get(),
																					   x => x.med.Uid,
																					   memd => memd.EvtDetUid,
																					   (x, memd) =>
																					   new Attivita
																					   {
																						   CodiceJMes = (double?)x.me.Uid,
																						   CausaleEstesa = StatoAttivitaMapper.FromJMesCodeExtended(x.me.EvtTypUid),
																						   Bolla = x.med.DocCod,
																						   Odp = x.med.PrdOrdCod,
																						   Fase = x.med.PrdPhsCod,
																						   QuantitaProdotta = (int)memd.QtyPrd,
																						   QuantitaScartata = (int)memd.QtyRej,
																						   InizioAttivita = x.me.TssStr,
																						   FineAttivita = x.me.TssEnd,
																						   SaldoAcconto = memd.DecAdv == -1 ? Costanti.ACCONTO : memd.DecAdv == 0 ? Costanti.SALDO : ""
																					   })
																				 .ToList();
			return attivitaOperatoreDellUltimaGiornata;
		}

		public async Task<string?> ApriAttrezzaggioFaseNonPianificataAsync(Attivita attivita, Operatore operatore)
        {
            string codiceFase = GetCodiceFase(attivita); ;

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
