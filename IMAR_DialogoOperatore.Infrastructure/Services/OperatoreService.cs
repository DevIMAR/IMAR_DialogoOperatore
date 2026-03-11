using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Application.Interfaces.Clients;
using IMAR_DialogoOperatore.Application.Interfaces.Services.Activities;
using IMAR_DialogoOperatore.Application.Interfaces.UoW;
using IMAR_DialogoOperatore.Domain.Entities.JMES;
using IMAR_DialogoOperatore.Application.Interfaces.Utilities;
using IMAR_DialogoOperatore.Domain.Models;
using System.Diagnostics;
using System.Globalization;

namespace IMAR_DialogoOperatore.Infrastructure.Services
{
	public class OperatoreService : IOperatoreService
	{
		private readonly ISynergyJmesUoW _synergyJmesUoW;
		private readonly IJmesApiClient _jmesApiClient;

		private readonly IAttivitaService _attivitaService;
		private readonly IMacchinaService _macchinaService;
		private readonly ILoggingService _loggingService;

		public Operatore Operatore { get; set; }

		public OperatoreService(
			ISynergyJmesUoW synergyJmesUoW,
			IJmesApiClient jmesApiClient,
			IAttivitaService attivitaService,
			IMacchinaService macchinaService,
			ILoggingService loggingService)
		{
			_synergyJmesUoW = synergyJmesUoW;
			_jmesApiClient = jmesApiClient;

			_attivitaService = attivitaService;
			_macchinaService = macchinaService;
			_loggingService = loggingService;
		}

		public async Task<Operatore?> OttieniOperatoreAsync(int? badge)
		{
			if (badge == null)
				return null;

			var swTotale = Stopwatch.StartNew();

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

			Operatore.Ingresso = ingressiUscite != null && ingressiUscite.Any(x => x.ResUid == risorsa.Uid) ? ingressiUscite.Where(x => x.ResUid == risorsa.Uid).Max(x => x.EdtInnTss ?? x.ClkInnTss) ?? fallback : fallback;
			Operatore.Uscita = ingressiUscite != null && ingressiUscite.Any(x => x.ResUid == risorsa.Uid) ? ingressiUscite.Where(x => x.ResUid == risorsa.Uid).Max(x => x.EdtOutTss ?? x.ClkOutTss) ?? fallback : fallback;
			Operatore.InizioPausa = iniziFiniPause != null && iniziFiniPause.Any(x => x.ResUid == risorsa.Uid) ? iniziFiniPause.Where(x => x.ResUid == risorsa.Uid).Max(x => x.TssStr) ?? fallback : fallback;
			Operatore.FinePausa = iniziFiniPause != null && iniziFiniPause.Any(x => x.ResUid == risorsa.Uid) ? iniziFiniPause.Where(x => x.ResUid == risorsa.Uid).Max(x => x.TssEnd) ?? fallback : fallback;

			var swAttivita = Stopwatch.StartNew();
			Operatore.AttivitaAperte = await _attivitaService.OttieniAttivitaOperatoreAsync(Operatore);
			_loggingService.LogInfo($"[TIMING] OttieniOperatoreAsync.AttivitaAperte: {swAttivita.ElapsedMilliseconds}ms");

			var swMacchina = Stopwatch.StartNew();
			await AssegnaMacchinaAdOperatoreAsync();
			_loggingService.LogInfo($"[TIMING] OttieniOperatoreAsync.AssegnaMacchina: {swMacchina.ElapsedMilliseconds}ms");

			Operatore.Stato = GetStatus();

			_loggingService.LogInfo($"[TIMING] OttieniOperatoreAsync totale: {swTotale.ElapsedMilliseconds}ms");
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

		private async Task AssegnaMacchinaAdOperatoreAsync()
		{
			Operatore.MacchineAssegnate = new List<Macchina?>();

			Macchina? macchina;
			Attivita? attivitaDirettaApertaDaOperatore = Operatore.AttivitaAperte.FirstOrDefault(x => !x.Bolla.Contains("AI"));
			if (attivitaDirettaApertaDaOperatore != null)
				macchina = await _macchinaService.GetMacchinaFittiziaByFirstAttivitaApertaAsync(attivitaDirettaApertaDaOperatore, Operatore.IdJMes);
			else
				macchina = await _macchinaService.GetPrimaMacchinaFittiziaNonUtilizzataAsync();

			if (macchina != null)
				Operatore.MacchineAssegnate.Add(macchina);
		}

		private string GetStatus()
		{
			if (Operatore.Uscita >= Operatore.Ingresso)
				return Costanti.ASSENTE;

			if (Operatore.InizioPausa > Operatore.Ingresso && Operatore.InizioPausa >= Operatore.FinePausa)
				return Costanti.IN_PAUSA;

			return Costanti.PRESENTE;
		}

		public async Task<string?> RimuoviAttivitaDaOperatoreAsync(Operatore operatore, Attivita attivitaDaRimuovere, int? quantitaProdotta, int? quantitaScartata, bool isSospeso = false, bool? isAttrezzaggio = null)
		{
			if (isAttrezzaggio == null)
				isAttrezzaggio = attivitaDaRimuovere.Causale == Costanti.IN_ATTREZZAGGIO;

			string? errore = null;

			if (isAttrezzaggio == true)
			{
				if (isSospeso)
					errore = await _jmesApiClient.RegistrazioneOperazioneSuDbAsync(() => _jmesApiClient.MesEquipSuspensionAsync(operatore.Badge, (double)attivitaDaRimuovere.CodiceJMes));
				else
				{
					errore = await _jmesApiClient.RegistrazioneOperazioneSuDbAsync(() => _jmesApiClient.MesEquipEndAsync(operatore.Badge, (double)attivitaDaRimuovere.CodiceJMes));

                    if (errore == null)
						errore = await _jmesApiClient.RegistrazioneOperazioneSuDbAsync(() => _jmesApiClient.MesEquipRemoveAsync(operatore.Badge, (double)attivitaDaRimuovere.CodiceJMes));
				}
			}
			else
			{
				if (isSospeso)
					errore = await _jmesApiClient.RegistrazioneOperazioneSuDbAsync(() => _jmesApiClient.MesWorkSuspensionAsync(operatore.Badge, attivitaDaRimuovere, (int)quantitaProdotta, (int)quantitaScartata));
				else
					errore = await _jmesApiClient.RegistrazioneOperazioneSuDbAsync(() => _jmesApiClient.MesWorkEndAsync(operatore.Badge, attivitaDaRimuovere, (int)quantitaProdotta, (int)quantitaScartata));
			}

			return errore;
		}

		public async Task<string?> AggiungiAttivitaAdOperatoreAsync(bool isAttrezzaggio, Operatore operatore, Attivita attivitaDaAggiungere, bool isAttivitaIndiretta)
		{
			if (attivitaDaAggiungere == null)
				return "Nessuna attività da aprire selezionata";

			string? errore;
			bool isCambioCausaleApertura = _attivitaService.ConfrontaCausaliAttivita(operatore.AttivitaAperte, attivitaDaAggiungere.Bolla, attivitaDaAggiungere.Causale);

			if (isAttrezzaggio)
			{
				if (isCambioCausaleApertura)
					return "Non è possibile aprire l'attrezzaggio di una fase se se ne è già aperto il lavoro!";

				errore = await GestisciAperturaAttrezzaggioAsync(operatore, attivitaDaAggiungere);
			}
			else
				errore = await GestisciAperturaLavoroAsync(operatore, attivitaDaAggiungere, isCambioCausaleApertura, isAttivitaIndiretta);

			if (errore != null)
				return errore;

			return null;
		}

		private async Task<string?> GestisciAperturaAttrezzaggioAsync(Operatore operatore, Attivita attivita)
		{
			string? errore = null;

			Macchina? macchinaDaAttivitaAttrezzata = await _macchinaService.GetMacchinaFittiziaDaAttivitaAttrezzataAsync(attivita);
			errore = await _jmesApiClient.RegistrazioneOperazioneSuDbAsync(() => _jmesApiClient.MesEquipStartAsync(operatore, attivita.Bolla, macchinaDaAttivitaAttrezzata));

			return errore;
		}

		private async Task<string?> GestisciAperturaLavoroAsync(Operatore operatore, Attivita attivitaDaAggiungere, bool isCambioCausaleApertura, bool isAttivitaIndiretta)
		{
			string? errore = null;

			if (isAttivitaIndiretta)
				errore = await _jmesApiClient.RegistrazioneOperazioneSuDbAsync(() => _jmesApiClient.MesWorkStartIndirettaAsync(operatore.Badge, attivitaDaAggiungere.Bolla));
			else
				errore = await GestisciAperturaAttivitaDirettaAsync(operatore, attivitaDaAggiungere, isCambioCausaleApertura);

			return errore;
		}

		private async Task<string?> GestisciAperturaAttivitaDirettaAsync(Operatore operatore, Attivita attivitaDaAggiungere, bool isCambioCausaleApertura)
		{
			string? errore = null;

			if (isCambioCausaleApertura)
			{
				errore = await GestisciCambioCausaleAsync(operatore, attivitaDaAggiungere);
				if (errore != null)
					return errore;
			}

			Macchina? macchinaDaAttivitaAttrezzata = await _macchinaService.GetMacchinaFittiziaDaAttivitaAttrezzataAsync(attivitaDaAggiungere);
			if (macchinaDaAttivitaAttrezzata != null)
			{
				operatore.MacchineAssegnate.Add(macchinaDaAttivitaAttrezzata);
				attivitaDaAggiungere.MacchinaFittizia = macchinaDaAttivitaAttrezzata;
			}
			else
			{
				Macchina? macchina = operatore.MacchineAssegnate.FirstOrDefault();
				if (macchina == null)
					return "Nessuna macchina fittizia disponibile per avviare il lavoro";
				attivitaDaAggiungere.MacchinaFittizia = macchina;
			}

			errore = await _jmesApiClient.RegistrazioneOperazioneSuDbAsync(() => _jmesApiClient.MesWorkStartAsync(operatore, attivitaDaAggiungere));

			return errore;
		}

		private async Task<string?> GestisciCambioCausaleAsync(Operatore operatore, Attivita attivitaDaAggiungere)
		{
			Attivita AttivitaOperatoreDaRimuovere = operatore.AttivitaAperte.Single(x => x.Bolla == attivitaDaAggiungere.Bolla);
			return await RimuoviAttivitaDaOperatoreAsync(operatore, AttivitaOperatoreDaRimuovere, null, null, isAttrezzaggio: true);
		}

		public Operatore GetOperatoreDaIdJMes(string idJMesOperatore)
		{
			AngRes res = _synergyJmesUoW.AngRes.Get(x => x.Uid == decimal.Parse(idJMesOperatore)).Single();
			return GetOperatoreFromAngRes(res);
		}
	}
}
