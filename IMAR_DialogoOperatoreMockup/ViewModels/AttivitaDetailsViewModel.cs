using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Interfaces.Observers;
using IMAR_DialogoOperatore.Interfaces.ViewModels;

namespace IMAR_DialogoOperatore.ViewModels
{
    public class AttivitaDetailsViewModel : ViewModelBase
	{
		private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;
		private IAttivitaViewModel? _attivitaSelezionata;
		private bool _isRiaperturaAttiva;
		private bool _isAperturaLavoroAutomaticaAttiva;

		public bool IsAvanzamento => (_dialogoOperatoreObserver.OperazioneInCorso == Costanti.AVANZAMENTO || _dialogoOperatoreObserver.OperazioneInCorso == Costanti.FINE_LAVORO) && 
										(_attivitaSelezionata != null && (_attivitaSelezionata.DescrizioneArticolo != Costanti.FASE_INDIRETTA));
		public bool IsFineAttivitaInUscita => (_dialogoOperatoreObserver.OperazioneInCorso == Costanti.FINE_LAVORO || _dialogoOperatoreObserver.OperazioneInCorso == Costanti.FINE_ATTREZZAGGIO) && _dialogoOperatoreObserver.IsUscita && !IsAttivitaIndiretta();

		public bool IsFineAttrezzaggio => _dialogoOperatoreObserver.OperazioneInCorso == Costanti.FINE_ATTREZZAGGIO && !_dialogoOperatoreObserver.IsUscita;
		public double QuantitaOrdine => _attivitaSelezionata != null ? (double)_attivitaSelezionata.QuantitaOrdine : 0;
		public double QuantitaProdottaPrecedentemente => _attivitaSelezionata != null ? QuantitaOrdine - (double)_attivitaSelezionata.QuantitaResidua : 0;
		public string CompletamentoFase => QuantitaProdottaPrecedentemente.ToString() + "/" + QuantitaOrdine.ToString();
		public string QuantitaResidua => _attivitaSelezionata != null ? _attivitaSelezionata.QuantitaResidua.ToString() : "0";
		public string QuantitaScartataTotale => _attivitaSelezionata != null ? _attivitaSelezionata.QuantitaScartata.ToString() : "0";
		public string StatoAttivita => (_attivitaSelezionata != null && _attivitaSelezionata.SaldoAcconto == Costanti.SALDO) ? Costanti.ATTIVITA_COMPLETATA : Costanti.ATTIVITA_NON_COMPLETATA;


		public bool IsRiaperturaAttiva
		{
			get { return _isRiaperturaAttiva; }
			set
			{
				_isRiaperturaAttiva = value;

				_dialogoOperatoreObserver.IsRiaperturaAttiva = _isRiaperturaAttiva;

				OnNotifyStateChanged();
			}
		}

		public bool IsAperturaLavoroAutomaticaAttiva
		{
			get { return _isAperturaLavoroAutomaticaAttiva; }
			set
			{
                _isAperturaLavoroAutomaticaAttiva = value;

				_dialogoOperatoreObserver.IsAperturaLavoroAutomaticaAttiva = _isAperturaLavoroAutomaticaAttiva;

				OnNotifyStateChanged();
			}
		}

		public AttivitaDetailsViewModel(
			IDialogoOperatoreObserver dialogoOperatoreStore)
        {
            _dialogoOperatoreObserver = dialogoOperatoreStore;

            _dialogoOperatoreObserver.OnAttivitaSelezionataChanged += DialogoOperatoreStore_OnAttivitaSelezionataChanged;
            _dialogoOperatoreObserver.OnIsRiaperturaAttivaChanged += DialogoOperatoreObserver_OnIsRiaperturaAttivaChanged;
        }

        private void DialogoOperatoreObserver_OnIsRiaperturaAttivaChanged()
        {
            _isRiaperturaAttiva = _dialogoOperatoreObserver.IsRiaperturaAttiva;
        }

        private void DialogoOperatoreStore_OnAttivitaSelezionataChanged()
		{
			_attivitaSelezionata = _dialogoOperatoreObserver.AttivitaSelezionata;

			if (_attivitaSelezionata == null)
            {
				_isRiaperturaAttiva = false;
				_isAperturaLavoroAutomaticaAttiva = false;

				return;
			}

			OnNotifyStateChanged();
		}
		private bool IsAttivitaIndiretta()
		{
			return _dialogoOperatoreObserver.AttivitaSelezionata != null &&
					_dialogoOperatoreObserver.AttivitaSelezionata.DescrizioneArticolo != null &&
					_dialogoOperatoreObserver.AttivitaSelezionata.DescrizioneArticolo.Contains(Costanti.FASE_INDIRETTA);
		}

		public override void Dispose()
        {
            _dialogoOperatoreObserver.OnAttivitaSelezionataChanged -= DialogoOperatoreStore_OnAttivitaSelezionataChanged;
            _dialogoOperatoreObserver.OnIsRiaperturaAttivaChanged -= DialogoOperatoreObserver_OnIsRiaperturaAttivaChanged;
        }
	}
}
