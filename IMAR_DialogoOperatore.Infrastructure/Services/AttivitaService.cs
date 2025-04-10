using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Application.Interfaces.Clients;
using IMAR_DialogoOperatore.Application.Interfaces.Mappers;
using IMAR_DialogoOperatore.Application.Interfaces.Services.Activities;
using IMAR_DialogoOperatore.Application.Interfaces.Utilities;
using IMAR_DialogoOperatore.Domain.Models;
using IMAR_DialogoOperatore.Infrastructure.Services;

namespace IMAR_DialogoOperatore.Services
{
    public class AttivitaService : IAttivitaService
    {
        private readonly IMacchinaService _macchinaService;
        private readonly IJmesApiClient _jmesApiClient;
        private readonly IJMesApiClientErrorUtility _jMesApiClientErrorUtility;
        private readonly IStatoAttivitaMapper _statoAttivitaMapper;
        private readonly CaricamentoAttivitaInBackroundService _caricamentoAttivitaInBackroundService;

        public Attivita Attivita { get; private set; }
        public IList<Attivita> AttivitaTrovate { get; private set; }

        public AttivitaService(
            IMacchinaService macchinaService,
            IJmesApiClient jmesApiClient,
            IJMesApiClientErrorUtility jMesApiClientErrorUtility,
            IStatoAttivitaMapper statoAttivitaMapper,
            CaricamentoAttivitaInBackroundService caricamentoAttivitaInBackroundService)
        {
            _macchinaService = macchinaService;

            _jmesApiClient = jmesApiClient;
            _jMesApiClientErrorUtility = jMesApiClientErrorUtility;

            _statoAttivitaMapper = statoAttivitaMapper;

            _caricamentoAttivitaInBackroundService = caricamentoAttivitaInBackroundService;
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

        public Attivita? CercaAttivitaPerBolla(string bolla)
        {
            Attivita? attivita = null;

            if (bolla.Length == 5)
                attivita = OttieniAttivitaIndiretta(bolla);
            else
                attivita = OttieniAttivitaAperta(bolla);

            return attivita;
        }

        private Attivita? OttieniAttivitaIndiretta(string bolla)
        {
            stdMesIndTsk? stdMesIndTsk = _caricamentoAttivitaInBackroundService.GetAttivitaIndirette()
                                                    .SingleOrDefault(x => x.ID_Ind3463 == bolla);
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

        private Attivita? OttieniAttivitaAperta(string bolla)
        {

            vrtManNotActive? vrtManNotActive = _caricamentoAttivitaInBackroundService.GetAttivitaAperte()
                                                    .SingleOrDefault(x => x.notCod == bolla);
            if (vrtManNotActive == null)
                return null;

            return CreateNuovaAttivita(vrtManNotActive);
        }

        public IList<Attivita> GetAttivitaPerOdp(string odp)
        {
            IEnumerable<vrtManNotActive> attivitaFiltrate = _caricamentoAttivitaInBackroundService.GetAttivitaAperte()
                                                                .Where(x => x.prdOrdUid == odp);

            PopolaAttivitaTrovate(attivitaFiltrate);
            return AttivitaTrovate;
        }

        private void PopolaAttivitaTrovate(IEnumerable<vrtManNotActive> attivitaFiltrate)
        {
            AttivitaTrovate = new List<Attivita>();

            foreach (vrtManNotActive attivita in attivitaFiltrate)
                AttivitaTrovate.Add(CreateNuovaAttivita(attivita));
        }

        public string? AvanzaAttivita(Operatore operatore, Attivita attivitaDaAvanzare, int quantitaProdotta, int quantitaScartata)
        {
            attivitaDaAvanzare.QuantitaProdotta += quantitaProdotta;
            attivitaDaAvanzare.QuantitaScartata += quantitaScartata;

            if (attivitaDaAvanzare.SaldoAcconto != "S")
                attivitaDaAvanzare.QuantitaResidua = attivitaDaAvanzare.QuantitaOrdine - attivitaDaAvanzare.QuantitaProdotta;

            HttpResponseMessage result = _jmesApiClient.MesAdvanceDeclaration(operatore.Badge, attivitaDaAvanzare, quantitaProdotta, quantitaScartata);

            string? errore = _jMesApiClientErrorUtility.GestioneEventualeErrore(result);
            if (errore != null)
                return errore;

            return null;
        }

        private Attivita CreateNuovaAttivita(vrtManNotActive vrtManNotActive)
        {
            Attivita nuovaAttivita = new Attivita
            {
                Bolla = vrtManNotActive.notCod,
                Odp = vrtManNotActive.prdOrdUid,
                Fase = vrtManNotActive.prdPhsCod,
                Articolo = vrtManNotActive.itmCod,
                DescrizioneFase = vrtManNotActive.prdPhsDsc,
                DescrizioneArticolo = vrtManNotActive.itmDsc,
                QuantitaOrdine = (int)vrtManNotActive.prdOrdQty,
                QuantitaProdotta = (int)vrtManNotActive.qtyPrd,
                QuantitaScartata = (int)vrtManNotActive.qtyRej,
                Macchina = new Macchina
                {
                    CentroDiLavoro = vrtManNotActive.macUid.Substring(0, 3),
                    CodiceMacchina = vrtManNotActive.macUid.Substring(2, 3),
                    CodiceJMes = _macchinaService.GetCodiceJmesByCodice(vrtManNotActive.macUid)
                },

                //Placeholder
                SaldoAcconto = "A"
            };

            nuovaAttivita.QuantitaResidua = nuovaAttivita.QuantitaOrdine - nuovaAttivita.QuantitaProdotta;

            return nuovaAttivita;
        }

        public IList<Attivita> OttieniAttivitaOperatore(string badgeOperatore)
        {
            IList<mesEvtOpe>? attivitaOperatore = OttieniTutteAttivitaOperatore(badgeOperatore);
            IList<mesTskForOpe>? attivitaIndiretteOperatore = OttieniAttivitaIndiretteOperatore(badgeOperatore);
            if (attivitaOperatore == null && attivitaIndiretteOperatore == null)
                return new List<Attivita>();

            IList<mesDiaOpe>? attivitaAperte = _jmesApiClient.ChiamaQueryGetJmes<mesDiaOpe>();
            if (attivitaAperte == null)
                return new List<Attivita>();

            IList<Attivita> attivitaOperatoreAperte = GetAttivitaOperatoreAperte(attivitaOperatore, attivitaAperte, attivitaIndiretteOperatore);

            return attivitaOperatoreAperte;
        }

        private IList<mesEvtOpe>? OttieniTutteAttivitaOperatore(string badgeOperatore)
        {
            IList<mesEvtOpe>? attivitaOperatore = _jmesApiClient.ChiamaQueryGetJmes<mesEvtOpe>();
            if (attivitaOperatore == null)
                return null;

            attivitaOperatore = attivitaOperatore.Where(x => x.ID_Res368.TrimStart('0') == badgeOperatore.TrimStart('0')).ToList(); //ID_Res368 è il badge operatore nella query di SanMarco

            return attivitaOperatore;
        }

        private IList<mesTskForOpe>? OttieniAttivitaIndiretteOperatore(string badgeOperatore)
        {
            IList<mesTskForOpe>? attivitaOperatore = _jmesApiClient.ChiamaQueryGetJmes<mesTskForOpe>();
            if (attivitaOperatore == null)
                return null;

            attivitaOperatore = attivitaOperatore.Where(x => x.ID_Mac368.TrimStart('0') == badgeOperatore.TrimStart('0')).ToList(); //ID_Mac368 è il badge operatore nella query di SanMarco

            return attivitaOperatore;
        }

        private List<Attivita> GetAttivitaOperatoreAperte(IList<mesEvtOpe>? attivitaOperatore, IList<mesDiaOpe> attivitaAperte, IList<mesTskForOpe>? attivitaIndiretteOperatore)
        {
            List<Attivita> attivitaOperatoreAperte = new List<Attivita>();

            if (attivitaOperatore != null)
                attivitaOperatoreAperte.AddRange(OttieniAttivitaOperatoreAperte(attivitaOperatore, attivitaAperte));

            if (attivitaIndiretteOperatore != null)
                attivitaOperatoreAperte.AddRange(OttieniAttivitaIndiretteOperatoreAperte(attivitaAperte, attivitaIndiretteOperatore));

            foreach (Attivita attivita in attivitaOperatoreAperte)
                attivita.Macchina = _macchinaService.GetMacchinaRealeByAttivita(attivita);

            return attivitaOperatoreAperte;
        }

        private List<Attivita> OttieniAttivitaOperatoreAperte(IList<mesEvtOpe> attivitaOperatore, IList<mesDiaOpe> attivitaAperte)
        {
            List<Attivita> attivitaOperatoreAperte = attivitaAperte
                                                    .Join(attivitaOperatore,
                                                            aa => aa.ID_Evt3240,
                                                            ao => ao.ID_Ope3278,
                                                            (aa, ao) => new Attivita
                                                            {
                                                                Bolla = aa.ID_Det3350,
                                                                Odp = aa.ID_Det3353,
                                                                Fase = aa.ID_Det3355,
                                                                DescrizioneFase = aa.ID_Det3356,
                                                                Articolo = aa.ID_Det3358,
                                                                DescrizioneArticolo = aa.ID_Det3359,
                                                                QuantitaOrdine = (int)aa.ID_Det3375,
                                                                QuantitaProdotta = (int)aa.ID_Det3371,
                                                                QuantitaScartata = (int)aa.ID_Det3372,
                                                                QuantitaResidua = (int)aa.ID_Det3374,
                                                                CodiceJMes = aa.ID_Det3348,
                                                                Causale = _statoAttivitaMapper.FromJMesStatus(aa.ID_Sts3130),

                                                                //Placeholder
                                                                SaldoAcconto = "A"
                                                            })
                                                    .Where(x => x.Causale == Costanti.IN_LAVORO ||
                                                                x.Causale == Costanti.IN_ATTREZZAGGIO ||
                                                                x.Causale == Costanti.LAVORO_SOSPESO ||
                                                                x.Causale == Costanti.ATTREZZAGGIO_SOSPESO)
                                                    .ToList();
            return attivitaOperatoreAperte;
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
                                                                DescrizioneFase = aa.ID_Det3356,
                                                                CodiceJMes = aa.ID_Det3348,
                                                                Causale = _statoAttivitaMapper.FromJMesStatus(aa.ID_Sts3130),

                                                                //Placeholder
                                                                SaldoAcconto = "A"
                                                            })
                                                    .Where(x => x.Causale == Costanti.IN_LAVORO ||
                                                                x.Causale == Costanti.IN_ATTREZZAGGIO ||
                                                                x.Causale == Costanti.LAVORO_SOSPESO ||
                                                                x.Causale == Costanti.ATTREZZAGGIO_SOSPESO)
                                                    .ToList();
            return attivitaOperatoreAperte;
        }
    }
}
