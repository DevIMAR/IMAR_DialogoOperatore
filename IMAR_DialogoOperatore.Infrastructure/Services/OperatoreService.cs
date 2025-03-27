using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Application.Interfaces.Clients;
using IMAR_DialogoOperatore.Application.Interfaces.Services.Activities;
using IMAR_DialogoOperatore.Application.Interfaces.UoW;
using IMAR_DialogoOperatore.Domain.Entities.JMES;
using IMAR_DialogoOperatore.Domain.Models;
using System.Globalization;

namespace IMAR_DialogoOperatore.Infrastructure.Services
{
    public class OperatoreService : IOperatoriService
	{
		private readonly ISynergyJmesUoW _synergyJmesUoW;
		private readonly IJmesApiClient _jmesApiClient;

		private readonly IAttivitaService _attivitaService;

        public Operatore Operatore { get; set; }

		public OperatoreService(
			ISynergyJmesUoW synergyJmesUoW,
			IJmesApiClient jmesApiClient,
			IAttivitaService attivitaService)
		{
			_synergyJmesUoW = synergyJmesUoW;
			_jmesApiClient = jmesApiClient;

			_attivitaService = attivitaService;
		}

		public Operatore? OttieniOperatore(int? badge)
        {
			if (badge == null)
				return null;

            string format = "yyyyMMddHHmmss";
            CultureInfo provider = CultureInfo.InvariantCulture;

            IList<mesOpeClk>? ingressiUscite = _jmesApiClient.ChiamaQueryGetJmes<mesOpeClk>();
			IList<stdTblResBrk>? IniziFiniPause = _jmesApiClient.ChiamaQueryGetJmes<stdTblResBrk>();

			AngRes? risorsa = _synergyJmesUoW.AngRes.Get(x => !string.IsNullOrEmpty(x.ResNam + x.ResSur))
													.ToList()
													.SingleOrDefault(x => x.ResCod.TrimStart('0').Trim() == badge.ToString().TrimStart('0').Trim());
			if (risorsa == null)
				return null;

			Operatore = new Operatore
			{
				Badge = risorsa.ResCod.TrimStart('0'),
				Nome = risorsa.ResNam,
				Cognome = risorsa.ResSur,
				Ingresso = DateTime.ParseExact(ingressiUscite.Where(x => x.ID_AngRes368 == risorsa.ResCod).Select(x => x.ID_ClkRes2365).Max() ?? "19000101000000", format, provider),
				Uscita = DateTime.ParseExact(ingressiUscite.Where(x => x.ID_AngRes368 == risorsa.ResCod).Select(x => x.ID_ClkRes2366).Max() ?? "19000101000000", format, provider),
				InizioPausa = DateTime.ParseExact(IniziFiniPause.Where(x => x.ID_Res368 == risorsa.ResCod).Select(x => x.ID_ResBrk2426).Max() ?? "19000101000000", format, provider),
				FinePausa = DateTime.ParseExact(IniziFiniPause.Where(x => x.ID_Res368 == risorsa.ResCod).Select(x => x.ID_ResBrk2427).Max() ?? "19000101000000", format, provider),
				AttivitaAperte = _attivitaService.OttieniAttivitaOperatore(risorsa.ResCod)
			};

			Operatore.Stato = GetStatus();

			return Operatore;
		}

		private string GetStatus()
		{
			if (Operatore.Uscita >= Operatore.Ingresso)
				return Costanti.ASSENTE;

			if (Operatore.InizioPausa > Operatore.Ingresso && Operatore.InizioPausa >= Operatore.FinePausa)
				return Costanti.IN_PAUSA;

			return Costanti.PRESENTE;
		}

		public string? RimuoviAttivitaDaOperatore(Operatore operatore, Attivita attivitaDaRimuovere, int? quantitaProdotta, int? quantitaScartata, bool isSospeso = false, bool? isAttrezzaggio = null)
		{
			if (isAttrezzaggio == null)
				isAttrezzaggio = attivitaDaRimuovere.Causale == Costanti.IN_ATTREZZAGGIO;

            string? errore = null;

            if (isSospeso)
			{
                errore = _jmesApiClient.RegistrazioneOperazioneSuDb(() => _jmesApiClient.MesSuspensionStart(operatore.Badge, attivitaDaRimuovere.Macchina.CodiceJMes));
				return errore;
			}

            if (isAttrezzaggio == true)
            {
                if (isSospeso)
                    errore = _jmesApiClient.RegistrazioneOperazioneSuDb(() => _jmesApiClient.MesEquipSuspension(operatore.Badge, (double)attivitaDaRimuovere.CodiceJMes));
                else
                    errore = _jmesApiClient.RegistrazioneOperazioneSuDb(() => _jmesApiClient.MesEquipEnd(operatore.Badge, (double)attivitaDaRimuovere.CodiceJMes));
            }
            else
            {
                if (isSospeso)
                    errore = _jmesApiClient.RegistrazioneOperazioneSuDb(() => _jmesApiClient.MesWorkSuspension(operatore.Badge, attivitaDaRimuovere, (int)quantitaProdotta, (int)quantitaScartata));
                else
                    errore = _jmesApiClient.RegistrazioneOperazioneSuDb(() => _jmesApiClient.MesWorkEnd(operatore.Badge, attivitaDaRimuovere, (int)quantitaProdotta, (int)quantitaScartata));
            }

            return errore;
        }

		public string? AggiungiAttivitaAdOperatore(bool isAttrezzaggio, Operatore operatore, Attivita attivitaDaAggiungere)
        {
			if (attivitaDaAggiungere == null)
                return "Nessuna attività da aprire selezionata";

			string? errore;
			bool isCambioCausaleApertura = _attivitaService.ConfrontaCausaliAttivita(operatore.AttivitaAperte, attivitaDaAggiungere.Bolla, attivitaDaAggiungere.Causale);

            if (isAttrezzaggio)
            {
                if (isCambioCausaleApertura)
                    return "Non è possibile aprire l'attrezzaggio di una fase se se ne è già aperto il lavoro!";

                errore = _jmesApiClient.RegistrazioneOperazioneSuDb(() => _jmesApiClient.MesEquipStart(operatore.Badge, attivitaDaAggiungere.Bolla, attivitaDaAggiungere.Macchina.CodiceJMes));
            }
            else
                errore = GestisciAperturaLavoro(operatore, attivitaDaAggiungere, isCambioCausaleApertura);

            if (errore != null)
				return errore;

            return null;
        }

        private string? GestisciAperturaLavoro(Operatore operatore, Attivita attivitaDaAggiungere, bool isCambioCausaleApertura)
        {
            string? errore;

            if (string.IsNullOrEmpty(attivitaDaAggiungere.Odp))
                errore = _jmesApiClient.RegistrazioneOperazioneSuDb(() => _jmesApiClient.MesWorkStartIndiretta(operatore.Badge, attivitaDaAggiungere.Bolla));
            else
            {
                if (isCambioCausaleApertura)
                {
                    Attivita AttivitaOperatoreDaRimuovere = operatore.AttivitaAperte.Single(x => x.Bolla == attivitaDaAggiungere.Bolla);
                    errore = RimuoviAttivitaDaOperatore(operatore, AttivitaOperatoreDaRimuovere, null, null, isAttrezzaggio: true);

					if (errore != null)
						return errore;
				}

                errore = _jmesApiClient.RegistrazioneOperazioneSuDb(() => _jmesApiClient.MesWorkStart(operatore.Badge, attivitaDaAggiungere.Bolla));
            }

            return errore;
        }
    }
}
