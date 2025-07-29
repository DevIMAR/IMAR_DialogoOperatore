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

		public List<TimbraturaAttivitaViewModel> CronologiaEventi
        {
			get { return _cronologiaEventi; }
			set 
			{ 
				_cronologiaEventi = value; 
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

            _taskCompilerObserver.OnIsPopupVisibleChanged += TaskCompilerObserver_OnIsPopupVisibleChanged;
        }

        private void TaskCompilerObserver_OnIsPopupVisibleChanged()
        {
            if (!_taskCompilerObserver.IsPopupVisible)
                CronologiaEventi = new List<TimbraturaAttivitaViewModel>();
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
    }
}
