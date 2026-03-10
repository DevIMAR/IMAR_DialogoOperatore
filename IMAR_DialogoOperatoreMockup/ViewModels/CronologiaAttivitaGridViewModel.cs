using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Application.Interfaces.Services.Activities;
using IMAR_DialogoOperatore.Interfaces.Mappers;
using IMAR_DialogoOperatore.Interfaces.Observers;

namespace IMAR_DialogoOperatore.ViewModels
{
    public class CronologiaAttivitaGridViewModel : ViewModelBase
    {
		private readonly IAttivitaService _attivitaService;
		private readonly ITimbratureService _timbratureService;
		private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;
		private readonly IAttivitaMapper _attivitaMapper;
		private readonly ITimbraturaMapper _timbraturaMapper;
		private readonly ITaskCompilerObserver _taskCompilerObserver;

		private List<TimbraturaAttivitaViewModel> _cronologiaEventi;
		private List<EventoRaggrupatoViewModel> _eventiRaggruppati;

		public List<TimbraturaAttivitaViewModel> CronologiaEventi
        {
			get { return _cronologiaEventi; }
			set
			{
				_cronologiaEventi = value;
				OnNotifyStateChanged();
			}
		}

		public List<EventoRaggrupatoViewModel> EventiRaggruppati
		{
			get { return _eventiRaggruppati; }
			set
			{
				_eventiRaggruppati = value;
				OnNotifyStateChanged();
			}
		}

		public CronologiaAttivitaGridViewModel(
			IAttivitaService attivitaService,
			ITimbratureService timbratureService,
			IDialogoOperatoreObserver dialogoOperatoreObserver,
			IAttivitaMapper attivitaMapper,
			ITimbraturaMapper timbraturaMapper,
            ITaskCompilerObserver taskCompilerObserver)
			: base ()
        {
			_attivitaService = attivitaService;
			_timbratureService = timbratureService;

			_dialogoOperatoreObserver = dialogoOperatoreObserver;

            _attivitaMapper = attivitaMapper;
			_timbraturaMapper = timbraturaMapper;

            _taskCompilerObserver = taskCompilerObserver;

            CronologiaEventi = new List<TimbraturaAttivitaViewModel>();
            EventiRaggruppati = new List<EventoRaggrupatoViewModel>();

            _taskCompilerObserver.OnIsPopupVisibleChanged += TaskCompilerObserver_OnIsPopupVisibleChanged;
        }

        private void TaskCompilerObserver_OnIsPopupVisibleChanged()
        {
            if (!_taskCompilerObserver.IsPopupVisible)
            {
                CronologiaEventi = new List<TimbraturaAttivitaViewModel>();
                EventiRaggruppati = new List<EventoRaggrupatoViewModel>();
            }
        }

        public void GetCronologiaEventi()
        {
            if (_dialogoOperatoreObserver.OperatoreSelezionato == null)
                return;

            GetCronologiaTimbrature();
			GetCronologiaAttivita();
        }

        public void GetCronologiaTimbrature()
        {
            if (_dialogoOperatoreObserver.OperatoreSelezionato == null)
                return;

            var timbrature = _timbraturaMapper.ListTimbratureToListTimbraturaAttivitaViewModel(
											_timbratureService.GetTimbratureOperatore(
												_dialogoOperatoreObserver.OperatoreSelezionato.Badge.ToString()));

            CronologiaEventi = CronologiaEventi.Concat(timbrature).ToList();
        }

        public void GetCronologiaAttivita()
        {
            if (_dialogoOperatoreObserver.OperatoreSelezionato == null)
                return;

            var attivita = _attivitaMapper.ListAttivitaToListTimbraturaAttivitaViewModel(
											_attivitaService.GetAttivitaOperatoreDellUltimaGiornata(
												(int)_dialogoOperatoreObserver.OperatoreSelezionato.IdJMes));

            CronologiaEventi = CronologiaEventi.Concat(attivita).ToList();
        }

        /// <summary>
        /// Carica le attività raggruppate (Inizio+Fine in una riga) + timbrature per il TaskPopup.
        /// </summary>
        public void GetEventiRaggruppati()
        {
            if (_dialogoOperatoreObserver.OperatoreSelezionato == null)
                return;

            var attivitaList = _attivitaService.GetAttivitaOperatoreDellUltimaGiornata(
                (int)_dialogoOperatoreObserver.OperatoreSelezionato.IdJMes);

            var eventiRaggruppati = _attivitaMapper.ListAttivitaToListEventiRaggruppati(attivitaList).ToList();

            // Aggiungi timbrature (Ingresso/Uscita/Pause)
            var timbratureList = _timbratureService.GetTimbratureOperatore(
                _dialogoOperatoreObserver.OperatoreSelezionato.Badge.ToString());

            // Raggruppa Inizio Pausa + Fine Pausa in una sola riga
            var iniziPausa = timbratureList
                .Where(t => t.Causale == Costanti.INIZIO_PAUSA)
                .OrderBy(t => t.Timestamp)
                .ToList();
            var finiPausa = timbratureList
                .Where(t => t.Causale == Costanti.FINE_PAUSA)
                .OrderBy(t => t.Timestamp)
                .ToList();

            for (int i = 0; i < iniziPausa.Count; i++)
            {
                eventiRaggruppati.Add(new EventoRaggrupatoViewModel
                {
                    CausaleEstesa = "Pausa",
                    OraInizio = iniziPausa[i].Timestamp,
                    OraFine = i < finiPausa.Count ? finiPausa[i].Timestamp : null
                });
            }

            // Aggiungi Ingresso/Uscita come righe singole
            foreach (var timbratura in timbratureList.Where(t =>
                t.Causale != Costanti.INIZIO_PAUSA && t.Causale != Costanti.FINE_PAUSA))
            {
                eventiRaggruppati.Add(new EventoRaggrupatoViewModel
                {
                    CausaleEstesa = timbratura.Causale,
                    OraInizio = timbratura.Timestamp
                });
            }

            EventiRaggruppati = eventiRaggruppati;
        }
    }
}
