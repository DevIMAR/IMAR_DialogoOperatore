using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Application.Interfaces.Clients;
using IMAR_DialogoOperatore.Application.Interfaces.Services.Activities;
using IMAR_DialogoOperatore.Application.Interfaces.UoW;
using IMAR_DialogoOperatore.Domain.Entities.JMES;
using IMAR_DialogoOperatore.Domain.Models;
using System.Globalization;

namespace IMAR_DialogoOperatore.Infrastructure.Services
{
	public class OperatoreService : IOperatoreService
	{
		private readonly ISynergyJmesUoW _synergyJmesUoW;
		private readonly IJmesApiClient _jmesApiClient;

		private readonly IAttivitaService _attivitaService;
		private readonly IMacchinaService _macchinaService;

		public Operatore Operatore { get; set; }

		public OperatoreService(
			ISynergyJmesUoW synergyJmesUoW,
			IJmesApiClient jmesApiClient,
			IAttivitaService attivitaService,
			IMacchinaService macchinaService)
		{
			_synergyJmesUoW = synergyJmesUoW;
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

			IQueryable<TblResClk>? ingressiUscite = null;
			IQueryable<TblResBrk>? iniziFiniPause = null;
			AngRes? risorsa = null;
			DateTime fallback = new DateTime(1900, 1, 1);

			GetTimbratureOperatore(out ingressiUscite, out iniziFiniPause);

			risorsa = _synergyJmesUoW.AngRes.Get(x => !string.IsNullOrEmpty(x.ResNam + x.ResSur))
											.ToList()
											.SingleOrDefault(x => x.ResCod.TrimStart('0').Trim() == badge.ToString().TrimStart('0').Trim());

			if (risorsa == null)
				return null;

			Operatore = GetOperatoreFromAngRes(risorsa);

			Operatore.Ingresso = ingressiUscite != null && ingressiUscite.Any(x => x.ResUid == risorsa.Uid) ? ingressiUscite.Where(x => x.ResUid == risorsa.Uid).Max(x => x.ClkInnTss) ?? fallback : fallback;
			Operatore.Uscita = ingressiUscite != null && ingressiUscite.Any(x => x.ResUid == risorsa.Uid) ? ingressiUscite.Where(x => x.ResUid == risorsa.Uid).Max(x => x.ClkOutTss) ?? fallback : fallback;
			Operatore.InizioPausa = iniziFiniPause != null && iniziFiniPause.Any(x => x.ResUid == risorsa.Uid) ? iniziFiniPause.Where(x => x.ResUid == risorsa.Uid).Max(x => x.TssStr) ?? fallback : fallback;
			Operatore.FinePausa = iniziFiniPause != null && iniziFiniPause.Any(x => x.ResUid == risorsa.Uid) ? iniziFiniPause.Where(x => x.ResUid == risorsa.Uid).Max(x => x.TssEnd) ?? fallback : fallback;
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

		public void GetTimbratureOperatore(out IQueryable<TblResClk>? ingressiUscite, out IQueryable<TblResBrk>? iniziFiniPause)
		{
			ingressiUscite = _synergyJmesUoW.TblResClk.Get();
			iniziFiniPause = _synergyJmesUoW.TblResBrk.Get();
		}

		private void AssegnaMacchinaAdOperatore()
		{
			Operatore.MacchineAssegnate = new List<Macchina?>();

			Attivita? attivitaDirettaApertaDaOperatore = Operatore.AttivitaAperte.FirstOrDefault(x => !x.Bolla.Contains("AI"));
			if (attivitaDirettaApertaDaOperatore != null)
				Operatore.MacchineAssegnate.Add(_macchinaService.GetMacchinaFittiziaByFirstAttivitaAperta(attivitaDirettaApertaDaOperatore, Operatore.IdJMes));
			else
				Operatore.MacchineAssegnate.Add(_macchinaService.GetPrimaMacchinaFittiziaNonUtilizzata());
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

			if (isAttrezzaggio == true)
			{
				if (isSospeso)
					errore = _jmesApiClient.RegistrazioneOperazioneSuDb(() => _jmesApiClient.MesEquipSuspension(operatore.Badge, (double)attivitaDaRimuovere.CodiceJMes));
				else
				{
					errore = _jmesApiClient.RegistrazioneOperazioneSuDb(() => _jmesApiClient.MesEquipEnd(operatore.Badge, (double)attivitaDaRimuovere.CodiceJMes));

                    if (errore == null)
						errore = _jmesApiClient.RegistrazioneOperazioneSuDb(() => _jmesApiClient.MesEquipRemove(operatore.Badge, (double)attivitaDaRimuovere.CodiceJMes));
				}
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

				errore = GestisciAperturaAttrezzaggio(operatore, attivitaDaAggiungere);
			}
			else
				errore = GestisciAperturaLavoro(operatore, attivitaDaAggiungere, isCambioCausaleApertura, isAttivitaIndiretta);

			if (errore != null)
				return errore;

			return null;
		}

		private string? GestisciAperturaAttrezzaggio(Operatore operatore, Attivita attivita)
		{
			string? errore = null;

			Macchina? macchinaDaAttivitaAttrezzata = _macchinaService.GetMacchinaFittiziaDaAttivitaAttrezzata(attivita);
			errore = _jmesApiClient.RegistrazioneOperazioneSuDb(() => _jmesApiClient.MesEquipStart(operatore, attivita.Bolla, macchinaDaAttivitaAttrezzata));

			return errore;
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

			Macchina? macchinaDaAttivitaAttrezzata = _macchinaService.GetMacchinaFittiziaDaAttivitaAttrezzata(attivitaDaAggiungere);
			if (macchinaDaAttivitaAttrezzata != null)
			{
				operatore.MacchineAssegnate.Add(macchinaDaAttivitaAttrezzata);
				attivitaDaAggiungere.MacchinaFittizia = macchinaDaAttivitaAttrezzata;
			}
			else
				attivitaDaAggiungere.MacchinaFittizia = operatore.MacchineAssegnate.First();

			errore = _jmesApiClient.RegistrazioneOperazioneSuDb(() => _jmesApiClient.MesWorkStart(operatore, attivitaDaAggiungere));

			return errore;
		}

		private string? GestisciCambioCausale(Operatore operatore, Attivita attivitaDaAggiungere)
		{
			Attivita AttivitaOperatoreDaRimuovere = operatore.AttivitaAperte.Single(x => x.Bolla == attivitaDaAggiungere.Bolla);
			return RimuoviAttivitaDaOperatore(operatore, AttivitaOperatoreDaRimuovere, null, null, isAttrezzaggio: true);
		}

		public Operatore GetOperatoreDaIdJMes(string idJMesOperatore)
		{
			AngRes res = _synergyJmesUoW.AngRes.Get(x => x.Uid == decimal.Parse(idJMesOperatore)).Single();
			return GetOperatoreFromAngRes(res);
		}
	}
}
