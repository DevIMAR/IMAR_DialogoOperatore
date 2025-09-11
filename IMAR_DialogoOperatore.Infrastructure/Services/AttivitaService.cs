using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Application.Interfaces.Clients;
using IMAR_DialogoOperatore.Application.Interfaces.Repositories;
using IMAR_DialogoOperatore.Application.Interfaces.Services.Activities;
using IMAR_DialogoOperatore.Application.Interfaces.UoW;
using IMAR_DialogoOperatore.Application.Interfaces.Utilities;
using IMAR_DialogoOperatore.Domain.Entities.Imar_Connect;
using IMAR_DialogoOperatore.Domain.Models;
using IMAR_DialogoOperatore.Infrastructure.Mappers;
using IMAR_DialogoOperatore.Infrastructure.Services;

namespace IMAR_DialogoOperatore.Services
{
    public class AttivitaService : IAttivitaService
    {
        private readonly IMacchinaService _macchinaService;
        private readonly IJmesApiClient _jmesApiClient;
        private readonly CaricamentoAttivitaInBackgroundService _caricamentoAttivitaInBackroundService;
        private readonly ISynergyJmesUoW _synergyJmesUoW;
        private readonly IImarConnectUoW _imarConnectUoW;
        private readonly IAs400Repository _as400Repository;

        public AttivitaService(
            IMacchinaService macchinaService,
            IJmesApiClient jmesApiClient,
            CaricamentoAttivitaInBackgroundService caricamentoAttivitaInBackroundService,
            ISynergyJmesUoW synergyJmesUoW,
            IImarConnectUoW imarConnectUoW,
            IAs400Repository as400Repository)
        {
            _macchinaService = macchinaService;

            _jmesApiClient = jmesApiClient;

            _caricamentoAttivitaInBackroundService = caricamentoAttivitaInBackroundService;

            _as400Repository = as400Repository;
            _synergyJmesUoW = synergyJmesUoW;
            _imarConnectUoW = imarConnectUoW;
        }

        public bool ConfrontaCausaliAttivita(IList<Attivita> listaAttivitaDaControllare, string bollaAttivitaDaConfrontare, string operazioneAttivitaDaConfrontare)
        {
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
                attivitaTrovata = _as400Repository.ExecuteQuery<Attivita>(@"SELECT
                                                                            NRBLCI AS BOLLA, ORPRCI AS ODP, CDARCI AS ARTICOLO, trim(DSARMA) AS DESCRIZIONEARTICOLO, 
                                                                            CDFACI AS FASE, DSFACI AS DESCRIZIONEFASE, PF2.QORDCI AS QUANTITAORDINE, 
                                                                            COALESCE(SUM(jf.QTVERJM), 0) AS QUANTITAPRODOTTANONCONTABILIZZATA, 
                                                                            pf2.QPROCI AS QUANTITAPRODOTTACONTABILIZZATA,
                                                                            COALESCE(SUM(jf.QTSCAJM), 0) AS QUNATITASCARTATANONCONTABILIZZATA, 
                                                                            pf2.QSCACI AS QUANTITASCARTATACONTABILIZZATA,
                                                                            COALESCE(SUM(jf.QTNCOJM), 0) AS QTANONCONFORMENONCONTABILIZZATA,
                                                                            pf2.QRESCI AS QTANONCONFORMECONTABILIZZATA,
                                                                            CASE WHEN MAX(SAACCJM) IS NULL THEN max(TIRECI) ELSE  MAX(SAACCJM) end AS SALDOACCONTO
                                                                            FROM IMA90DAT.PCIMP00F pf2 
                                                                            JOIN IMA90DAT.MGART00F mf ON pf2.CDARCI = mf.CDARMA 
                                                                            LEFT JOIN IMA90DAT.JMRILM00F jf ON NRBLCI = NRTSKJM AND jf.QCONTJM = ''
                                                                            WHERE NRBLCI = '" + bolla + @"'
                                                                            GROUP BY NRBLCI, ORPRCI, CDARCI, DSARMA, CDFACI, DSFACI,
                                                                                    pf2.QORDCI, pf2.QPROCI, pf2.QSCACI, pf2.QRESCI")
                                                  .SingleOrDefault();

            if (attivitaTrovata == null)
                return attivitaTrovata;

            Interfaccia? attivitaInterfaccia = _imarConnectUoW.InterfacciaRepository.Get(i => i.Bolla == attivitaTrovata.Bolla)
                                                                                   .OrderByDescending(i => i.TimestampEnd)
                                                                                   .FirstOrDefault();

            return attivitaTrovata;
        }

        public IEnumerable<Attivita> GetAttivitaPerOdp(string odp)
        {
            IEnumerable<Attivita> attivitaFiltrate = _caricamentoAttivitaInBackroundService.GetAttivitaAperte()
                                                                .Where(x => x.Odp == odp);

            if (attivitaFiltrate == null || attivitaFiltrate.Count() == 0)
                attivitaFiltrate = _as400Repository.ExecuteQuery<Attivita>(@"SELECT
                                                                            NRBLCI AS BOLLA, ORPRCI AS ODP, CDARCI AS ARTICOLO, trim(DSARMA) AS DESCRIZIONEARTICOLO, 
                                                                            CDFACI AS FASE, DSFACI AS DESCRIZIONEFASE, PF2.QORDCI AS QUANTITAORDINE, 
                                                                            COALESCE(SUM(jf.QTVERJM), 0) AS QUANTITAPRODOTTANONCONTABILIZZATA, 
                                                                            pf2.QPROCI AS QUANTITAPRODOTTACONTABILIZZATA,
                                                                            COALESCE(SUM(jf.QTSCAJM), 0) AS QUNATITASCARTATANONCONTABILIZZATA, 
                                                                            pf2.QSCACI AS QUANTITASCARTATACONTABILIZZATA,
                                                                            COALESCE(SUM(jf.QTNCOJM), 0) AS QTANONCONFORMENONCONTABILIZZATA,
                                                                            pf2.QRESCI AS QTANONCONFORMECONTABILIZZATA,
                                                                            CASE WHEN MAX(SAACCJM) IS NULL THEN max(TIRECI) ELSE  MAX(SAACCJM) end AS SALDOACCONTO
                                                                            FROM IMA90DAT.PCIMP00F pf2 
                                                                            JOIN IMA90DAT.MGART00F mf ON pf2.CDARCI = mf.CDARMA 
                                                                            LEFT JOIN IMA90DAT.JMRILM00F jf ON NRBLCI = NRTSKJM AND jf.QCONTJM = ''
                                                                            WHERE ORPRCI = '" + odp + @"'
                                                                            GROUP BY NRBLCI, ORPRCI, CDARCI, DSARMA, CDFACI, DSFACI,
                                                                                    pf2.QORDCI, pf2.QPROCI, pf2.QSCACI, pf2.QRESCI");

            IEnumerable<Interfaccia> attivitaInterfaccia = _imarConnectUoW.InterfacciaRepository.Get();
            Interfaccia? interfaccia;

            foreach (Attivita attivita in attivitaFiltrate)
            {
                interfaccia = attivitaInterfaccia.Where(i => i.Bolla == attivita.Bolla)
                                                 .OrderByDescending(i => i.TimestampEnd)
                                                 .FirstOrDefault();
            }

            return attivitaFiltrate.OrderBy(x => x.Bolla);
        }

        public string? AvanzaAttivita(Operatore operatore, Attivita attivitaDaAvanzare, int quantitaProdotta, int quantitaScartata)
        {
            if (attivitaDaAvanzare.SaldoAcconto != Costanti.SALDO)
                attivitaDaAvanzare.QuantitaResidua = attivitaDaAvanzare.QuantitaOrdine - attivitaDaAvanzare.QuantitaProdottaNonContabilizzata;

            Interfaccia nuovoRecordInterfaccia = new Interfaccia
            {
                Id = Guid.NewGuid(),
                BadgeOperatore = operatore.Badge,
                Bolla = attivitaDaAvanzare.Bolla,
                TimestampStart = DateTime.Now,
                TimestampEnd = DateTime.Now,
                QuantitaProdotta = quantitaProdotta,
                QuantitaProdottaTotale = attivitaDaAvanzare.QuantitaProdotta,
                QuantitaScartata = quantitaScartata,
                QuantitaScartataTotale = attivitaDaAvanzare.QuantitaScartata
            };

            nuovoRecordInterfaccia.Acconto = attivitaDaAvanzare.SaldoAcconto == Costanti.ACCONTO;
            string? errore = ControllaAttivitaConcorrentiEGestisci(nuovoRecordInterfaccia, operatore);

            _imarConnectUoW.InterfacciaRepository.Insert(nuovoRecordInterfaccia);

            _imarConnectUoW.Save();

            return errore;
        }

        public string? ControllaAttivitaConcorrentiEGestisci(Interfaccia attivita, Operatore operatore)
        {
            string? errore = null;

            IQueryable<Interfaccia> attivitaConcorrenti = _imarConnectUoW.InterfacciaRepository.Get(i => i.Bolla == attivita.Bolla)
                                                                                               .Where(i => i.BadgeOperatore != operatore.Badge &&
                                                                                                           i.TimestampEnd == null &&
                                                                                                           i.Sospeso != true);
            if (attivitaConcorrenti.Any() && (bool)!attivita.Acconto)
            {
                errore = GestisciPresenzaAttivitaConcorrenti(attivita);
            }

            return errore;
        }

        private string GestisciPresenzaAttivitaConcorrenti(Interfaccia attivita)
        {
            attivita.Acconto = true;
            string errore = "Attivita aperta da altri utenti.\nLa chiusura è stata posta in acconto (NON A SALDO)";

            return errore;
        }

        public IList<Attivita> OttieniAttivitaOperatore(Operatore operatore)
        {
            IList<mesTskForOpe>? attivitaIndiretteOperatore = OttieniAttivitaIndiretteOperatore(operatore.Badge);
            if (attivitaIndiretteOperatore == null)
                return new List<Attivita>();

            IList<mesDiaOpe>? attivitaAperte = GetAttivitaAperte();
            if (attivitaAperte == null)
                return new List<Attivita>();

            IList<Attivita> attivitaOperatoreAperte = GetAttivitaOperatoreAperte(operatore, attivitaAperte, attivitaIndiretteOperatore);

            return attivitaOperatoreAperte.OrderBy(x => x.Bolla).ToList();
        }

        public IList<mesDiaOpe>? GetAttivitaAperte()
        {
            return _jmesApiClient.ChiamaQueryGetJmes<mesDiaOpe>();
        }

        private IList<mesTskForOpe>? OttieniAttivitaIndiretteOperatore(string badgeOperatore)
        {
            IList<mesTskForOpe>? attivitaOperatore = _jmesApiClient.ChiamaQueryGetJmes<mesTskForOpe>();
            if (attivitaOperatore == null)
                return null;

            attivitaOperatore = attivitaOperatore.Where(x => x.ID_Mac368.TrimStart('0') == badgeOperatore.TrimStart('0')).ToList(); //ID_Mac368 è il badge operatore nella query di SanMarco

            return attivitaOperatore;
        }

        private List<Attivita> GetAttivitaOperatoreAperte(Operatore operatore, IList<mesDiaOpe> attivitaAperte, IList<mesTskForOpe>? attivitaIndiretteOperatore)
        {
            List<Attivita> attivitaOperatoreAperte = new List<Attivita>();

            List<Attivita> attivitaOperatore = GetAttivitaOperatore(operatore);
            if (attivitaOperatore != null)
                attivitaOperatoreAperte.AddRange(attivitaOperatore);

            if (attivitaIndiretteOperatore != null)
                attivitaOperatoreAperte.AddRange(OttieniAttivitaIndiretteOperatoreAperte(attivitaAperte, attivitaIndiretteOperatore));

            foreach (Attivita attivita in attivitaOperatoreAperte)
                attivita.Macchina = _macchinaService.GetMacchinaRealeByAttivita(attivita);

            return attivitaOperatoreAperte;
        }

        private List<Attivita> GetAttivitaOperatore(Operatore operatore)
        {
            List<Attivita> lavoriOperatore = new List<Attivita>();
            List<Interfaccia> attivitaInterfaccia = _imarConnectUoW.InterfacciaRepository.Get(i => i.BadgeOperatore == operatore.Badge)
                                                                                         .Where(i => i.TimestampEnd == null)
                                                                                         .ToList();
            Interfaccia ultimoAggiornamento;

            foreach (Interfaccia interfaccia in attivitaInterfaccia)
            {
                ultimoAggiornamento = _imarConnectUoW.InterfacciaRepository.Get(i => i.Bolla == interfaccia.Bolla)
                                                                           .OrderByDescending(i => i.TimestampEnd)
                                                                           .First();

                lavoriOperatore.Add(new Attivita
                {
                    Bolla = interfaccia.Bolla,
                    Causale = Costanti.IN_LAVORO,
                    CausaleEstesa = Costanti.JMES_IN_LAVORO,
                    InizioAttivita = interfaccia.TimestampStart
                });
            }

            List<Attivita> attivitaOperatore = _synergyJmesUoW.MesEvt
                                                               .Get(x => (int)x.ResEffStrUid == operatore.IdJMes)
                                                               .Where(x => x.TssEnd == null)
                                                               .Join(_synergyJmesUoW.MesEvtDet.Get(),
                                                                        me => me.Uid,
                                                                        med => med.EvtUid,
                                                                        (me, med) => new { me, med })
                                                               .Join(_synergyJmesUoW.MesDiaOpe.Get(),
                                                                        x => (double?)x.me.Uid,
                                                                        mdo => (double?)mdo.EvtUid,
                                                                        (x, mdo) => new Attivita
                                                                        {
                                                                            Bolla = x.med.DocCod,
                                                                            Causale = StatoAttivitaMapper.FromJMesCode(x.me.EvtTypUid),
                                                                            CausaleEstesa = StatoAttivitaMapper.FromJMesCodeExtended(x.me.EvtTypUid),
                                                                            CodiceJMes = (double)mdo.Uid,
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

            attivita.AddRange(lavoriOperatore
                              .Join(_caricamentoAttivitaInBackroundService.GetAttivitaAperte(),
                                    lo => lo.Bolla,
                                    aa => aa.Bolla,
                                    (lo, aa) =>
                                    {
                                        aa.Causale = lo.Causale;
                                        aa.CausaleEstesa = lo.CausaleEstesa;
                                        aa.InizioAttivita = lo.InizioAttivita;
                                        return aa;
                                    }));

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
                                                                SaldoAcconto = Costanti.ACCONTO
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
                    DescrizioneFase = stdMesIndTsk.ID_Ind3464
                });
            }

            return attivitaIndirette;
        }

        public List<string>? GetIdOperatoriConBollaAperta(string bolla)
        {
            IList<mesDiaOpe> attivitaAperte = GetAttivitaAperte();
            IList<mesRcpActOpe>? riepilogoAttivitaOperatore = _jmesApiClient.ChiamaQueryGetJmes<mesRcpActOpe>();

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
            DateTime ieriAlle2045 = DateTime.Today.AddHours(-4).AddMinutes(45);
            DateTime oggiAlle2115 = DateTime.Today.AddHours(21).AddMinutes(15);

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
                                                                                           FineAttivita = x.me.TssEnd
                                                                                       })
                                                                                 .ToList();

            return attivitaOperatoreDellUltimaGiornata;
        }
    }
}
