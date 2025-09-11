using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Application.Interfaces.Clients;
using IMAR_DialogoOperatore.Application.Interfaces.Services.Activities;
using IMAR_DialogoOperatore.Application.Interfaces.UoW;
using IMAR_DialogoOperatore.Domain.Entities.Imar_Connect;
using IMAR_DialogoOperatore.Domain.Entities.JMES;
using IMAR_DialogoOperatore.Domain.Models;
using System.Diagnostics;
using System.Globalization;

namespace IMAR_DialogoOperatore.Infrastructure.Services
{
    public class OperatoreService : IOperatoreService
    {
        private readonly ISynergyJmesUoW _synergyJmesUoW;
        private readonly IImarConnectUoW _imarConnectUoW;
        private readonly IJmesApiClient _jmesApiClient;

        private readonly IAttivitaService _attivitaService;
        private readonly IMacchinaService _macchinaService;

        public Operatore Operatore { get; set; }

        public OperatoreService(
            ISynergyJmesUoW synergyJmesUoW,
            IImarConnectUoW imarConnectUoW,
            IJmesApiClient jmesApiClient,
            IAttivitaService attivitaService,
            IMacchinaService macchinaService)
        {
            _synergyJmesUoW = synergyJmesUoW;
            _imarConnectUoW = imarConnectUoW;
            _jmesApiClient = jmesApiClient;

            _attivitaService = attivitaService;
            _macchinaService = macchinaService;
        }

        public Operatore? OttieniOperatore(int? badge)
        {
            if (badge == null)
                return null;

            string format = "yyyyMMddHHmmss";
            CultureInfo provider = CultureInfo.InvariantCulture;

            IList<mesOpeClk>? ingressiUscite;
            IList<stdTblResBrk>? iniziFiniPause;

            GetTimbratureOperatore(out ingressiUscite, out iniziFiniPause);

            AngRes? risorsa = _synergyJmesUoW.AngRes.Get(x => !string.IsNullOrEmpty(x.ResNam + x.ResSur))
                                                    .ToList()
                                                    .SingleOrDefault(x => x.ResCod.TrimStart('0').Trim() == badge.ToString().TrimStart('0').Trim());
            
            if (risorsa == null)
                return null;

            Operatore = GetOperatoreFromAngRes(risorsa);

            Operatore.Ingresso = DateTime.ParseExact(ingressiUscite.Where(x => x.ID_AngRes368 == risorsa.ResCod).Select(x => x.ID_ClkRes2365).Max() ?? "19000101000000", format, provider);
            Operatore.Uscita = DateTime.ParseExact(ingressiUscite.Where(x => x.ID_AngRes368 == risorsa.ResCod).Select(x => x.ID_ClkRes2366).Max() ?? "19000101000000", format, provider);
            Operatore.InizioPausa = DateTime.ParseExact(iniziFiniPause.Where(x => x.ID_Res368 == risorsa.ResCod).Select(x => x.ID_ResBrk2426).Max() ?? "19000101000000", format, provider);
            Operatore.FinePausa = DateTime.ParseExact(iniziFiniPause.Where(x => x.ID_Res368 == risorsa.ResCod).Select(x => x.ID_ResBrk2427).Max() ?? "19000101000000", format, provider);

            Operatore.AttivitaAperte = _attivitaService.OttieniAttivitaOperatore(Operatore);

            AssegnaMacchinaAdOperatore();

            Operatore.Stato = GetStatus();

            return Operatore;
        }

        private Operatore GetOperatoreFromAngRes(AngRes risorsa)
        {
            return new Operatore
            {
                Badge = risorsa.ResCod.TrimStart('0'),
                Nome = risorsa.ResNam,
                Cognome = risorsa.ResSur,
                IdJMes = (int)risorsa.Uid
            };
        }

        public void GetTimbratureOperatore(out IList<mesOpeClk>? ingressiUscite, out IList<stdTblResBrk>? iniziFiniPause)
        {
            ingressiUscite = _jmesApiClient.ChiamaQueryGetJmes<mesOpeClk>();
            iniziFiniPause = _jmesApiClient.ChiamaQueryGetJmes<stdTblResBrk>();
        }

        private void AssegnaMacchinaAdOperatore()
        {
            Attivita? attivitaDirettaApertaDaOperatore = Operatore.AttivitaAperte.FirstOrDefault(x => !x.Bolla.Contains("AI"));
            if (attivitaDirettaApertaDaOperatore != null)
                Operatore.MacchinaAssegnata = _macchinaService.GetMacchinaFittiziaByFirstAttivitaAperta(attivitaDirettaApertaDaOperatore, Operatore.IdJMes);
            else
                Operatore.MacchinaAssegnata = _macchinaService.GetPrimaMacchinaFittiziaNonUtilizzata();
        }

        private string GetStatus()
        {
            if (Operatore.Uscita >= Operatore.Ingresso)
                return Costanti.ASSENTE;

            if (Operatore.InizioPausa > Operatore.Ingresso && Operatore.InizioPausa >= Operatore.FinePausa)
                return Costanti.IN_PAUSA;

            return Costanti.PRESENTE;
        }

        public string? RimuoviAttivitaDaOperatore(Operatore operatore, Attivita attivitaDaRimuovere, int quantitaProdotta = 0, int quantitaScartata = 0, bool isSospeso = false, bool? isAttrezzaggio = null)
        {
            if (isAttrezzaggio == null)
                isAttrezzaggio = attivitaDaRimuovere.Causale == Costanti.IN_ATTREZZAGGIO;

            string? errore = null;

            if (isAttrezzaggio == true)
            {
                errore = GestisciAttrezzaggio(operatore, attivitaDaRimuovere, isSospeso);
            }
            else
            {
                errore = GestisciLavoro(operatore, attivitaDaRimuovere, quantitaProdotta, quantitaScartata, isSospeso);
            }

            return errore;
        }

        private string? GestisciAttrezzaggio(Operatore operatore, Attivita attivitaDaRimuovere, bool isSospeso)
        {
            string? errore;

            if (isSospeso)
                errore = _jmesApiClient.RegistrazioneOperazioneSuDb(() => _jmesApiClient.MesEquipSuspension(operatore.Badge, (double)attivitaDaRimuovere.CodiceJMes));
            else
                errore = _jmesApiClient.RegistrazioneOperazioneSuDb(() => _jmesApiClient.MesEquipEnd(operatore.Badge, (double)attivitaDaRimuovere.CodiceJMes));
            return errore;
        }

        private string? GestisciLavoro(Operatore operatore, Attivita attivitaDaRimuovere, int quantitaProdotta, int quantitaScartata, bool isSospeso)
        {
            string? errore = null;
            Interfaccia attivita = _imarConnectUoW.InterfacciaRepository.Get(i => i.BadgeOperatore == operatore.Badge)
                                                                                        .Single(i => i.Bolla == attivitaDaRimuovere.Bolla &&
                                                                                                     i.TimestampEnd == null);

            Interfaccia ultimoAggiornamento = _imarConnectUoW.InterfacciaRepository.Get(i => i.Bolla == attivita.Bolla)
                                                                                   .OrderByDescending(i => i.TimestampEnd)
                                                                                   .First();

            if (isSospeso)
            {
                SospendiLavoro(operatore, attivitaDaRimuovere, quantitaProdotta, quantitaScartata, attivita);
            }
            else
            {
                errore = ChiudiLavoro(attivitaDaRimuovere, quantitaProdotta, quantitaScartata, attivita, ultimoAggiornamento);
            }

            _imarConnectUoW.Save();

            return errore;
        }

        private void SospendiLavoro(Operatore operatore, Attivita attivitaDaRimuovere, int quantitaProdotta, int quantitaScartata, Interfaccia attivita)
        {
            _attivitaService.AvanzaAttivita(operatore, attivitaDaRimuovere, quantitaProdotta, quantitaScartata);
            attivita.Sospeso = true;
        }

        private string? ChiudiLavoro(Attivita attivitaDaRimuovere, int quantitaProdotta, int quantitaScartata, Interfaccia attivita, Interfaccia ultimoAggiornamento)
        {
            string? errore = null;

            attivita.QuantitaProdotta = quantitaProdotta;
            attivita.QuantitaProdottaTotale = ultimoAggiornamento.QuantitaProdottaTotale + quantitaProdotta;
            attivita.QuantitaScartata = quantitaScartata;
            attivita.QuantitaScartataTotale = ultimoAggiornamento.QuantitaScartataTotale + quantitaScartata;
            attivita.TimestampEnd = DateTime.Now;

            attivita.Acconto = attivitaDaRimuovere.SaldoAcconto == Costanti.ACCONTO;

            errore = _attivitaService.ControllaAttivitaConcorrentiEGestisci(attivita, Operatore);

            return errore;
        }

        public string? AggiungiAttivitaAdOperatore(bool isAttrezzaggio, Operatore operatore, Attivita attivitaDaAggiungere, bool isAttivitaIndiretta)
        {
            if (attivitaDaAggiungere == null)
                return "Nessuna attività da aprire selezionata";

            string? errore;
            bool isCambioCausaleApertura = _attivitaService.ConfrontaCausaliAttivita(operatore.AttivitaAperte, attivitaDaAggiungere.Bolla, attivitaDaAggiungere.Causale);

            if (isAttrezzaggio)
            {
                if (isCambioCausaleApertura)
                    return "Non è possibile aprire l'attrezzaggio di una fase se se ne è già aperto il lavoro!";

                errore = _jmesApiClient.RegistrazioneOperazioneSuDb(() => _jmesApiClient.MesEquipStart(operatore, attivitaDaAggiungere.Bolla));
            }
            else
                errore = GestisciAperturaLavoro(operatore, attivitaDaAggiungere, isCambioCausaleApertura, isAttivitaIndiretta);

            if (errore != null)
                return errore;

            return null;
        }

        private string? GestisciAperturaLavoro(Operatore operatore, Attivita attivitaDaAggiungere, bool isCambioCausaleApertura, bool isAttivitaIndiretta)
        {
            string? errore = null;

            if (isAttivitaIndiretta)
                errore = _jmesApiClient.RegistrazioneOperazioneSuDb(() => _jmesApiClient.MesWorkStartIndiretta(operatore.Badge, attivitaDaAggiungere.Bolla));
            else
                errore = GestisciAperturaAttivitaDiretta(operatore, attivitaDaAggiungere, isCambioCausaleApertura);

            return errore;
        }

        private string? GestisciAperturaAttivitaDiretta(Operatore operatore, Attivita attivitaDaAggiungere, bool isCambioCausaleApertura)
        {
            string? errore = null;

            if (isCambioCausaleApertura)
            {
                errore = GestisciCambioCausale(operatore, attivitaDaAggiungere);
                if (errore != null)
                    return errore;
            }

            _imarConnectUoW.InterfacciaRepository.Insert(new Interfaccia
            {
                Id = Guid.NewGuid(),
                BadgeOperatore = operatore.Badge,
                Bolla = attivitaDaAggiungere.Bolla,
                TimestampStart = DateTime.Now
            });

            _imarConnectUoW.Save();

            return errore;
        }

        private string? GestisciCambioCausale(Operatore operatore, Attivita attivitaDaAggiungere)
        {
            Attivita AttivitaOperatoreDaRimuovere = operatore.AttivitaAperte.Single(x => x.Bolla == attivitaDaAggiungere.Bolla);
            return RimuoviAttivitaDaOperatore(operatore, AttivitaOperatoreDaRimuovere, isAttrezzaggio: true);
        }

        public Operatore GetOperatoreDaIdJMes(string idJMesOperatore)
        {
            AngRes res = _synergyJmesUoW.AngRes.Get(x => x.Uid == decimal.Parse(idJMesOperatore)).Single();
            return GetOperatoreFromAngRes(res);
        }
    }
}
