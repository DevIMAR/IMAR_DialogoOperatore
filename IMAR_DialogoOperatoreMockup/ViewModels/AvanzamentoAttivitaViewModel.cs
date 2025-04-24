using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Interfaces.Observers;
using IMAR_DialogoOperatore.Interfaces.ViewModels;

namespace IMAR_DialogoOperatore.ViewModels
{
    public class AvanzamentoAttivitaViewModel : ViewModelBase
	{
		private readonly IAvanzamentoObserver _avanzamentoObserver;
		private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;

		private uint? _quantitaProdotta;
		private uint? _quantitaScartata;
		private bool _isFaseCompletabile;

		public object? AttivitaSelezionata => _dialogoOperatoreObserver.AttivitaSelezionata;

		public uint? QuantitaProdotta 
		{ 
			get {  return _quantitaProdotta; }
			set
			{
				_quantitaProdotta = value;

				if (UInt32.TryParse(_quantitaProdotta.ToString(), out uint quantitaProdotta) && AttivitaSelezionata != null)
				{
					IsFaseCompletabile = Int32.Parse(_quantitaProdotta.ToString()) >= ((IAttivitaViewModel)AttivitaSelezionata).QuantitaResidua;
					_avanzamentoObserver.QuantitaProdotta = quantitaProdotta;
				}

				OnNotifyStateChanged();
			}
		}
        public uint? QuantitaScartata
		{ 
			get {  return _quantitaScartata; }
			set
			{
				_quantitaScartata = value;

				if (UInt32.TryParse(_quantitaScartata.ToString(), out uint quantitaScartata) && AttivitaSelezionata != null)
					_avanzamentoObserver.QuantitaScartata = quantitaScartata;

				OnNotifyStateChanged();
			}
		}
		public bool IsFaseCompletabile
		{
			get { return _isFaseCompletabile; }
			set 
			{ 
				_isFaseCompletabile = _dialogoOperatoreObserver.AttivitaSelezionata.SaldoAcconto == Costanti.SALDO ? true : value;
				_avanzamentoObserver.SaldoAcconto = _isFaseCompletabile ? Costanti.SALDO : Costanti.ACCONTO;

                OnNotifyStateChanged();
			}
		}

		public AvanzamentoAttivitaViewModel(
			IDialogoOperatoreObserver dialogoOperatoreObserver, 
			IAvanzamentoObserver avanzamentoObserver)
		{
			_dialogoOperatoreObserver = dialogoOperatoreObserver;
			_avanzamentoObserver = avanzamentoObserver;

			QuantitaProdotta = 0;
			QuantitaScartata = 0;

			_dialogoOperatoreObserver.OnAttivitaSelezionataChanged += AttivitaStore_OnAttivitaSelezionataChanged;
            _dialogoOperatoreObserver.OnIsDettaglioAttivitaOpenChanged += DialogoOperatoreObserver_OnIsDettaglioAttivitaOpenChanged;

			_avanzamentoObserver.OnQuantitaProdottaChanged += AvanzamentoStore_OnQuantitaChanged;
			_avanzamentoObserver.OnQuantitaScartataChanged += AvanzamentoStore_OnQuantitaChanged;
		}

        private void DialogoOperatoreObserver_OnIsDettaglioAttivitaOpenChanged()
        {
            QuantitaProdotta = 0;
            QuantitaScartata = 0;
        }

        private void AvanzamentoStore_OnQuantitaChanged()
		{
			if (QuantitaProdotta != _avanzamentoObserver.QuantitaProdotta)
				QuantitaProdotta = _avanzamentoObserver.QuantitaProdotta;

			if (QuantitaScartata != _avanzamentoObserver.QuantitaScartata)
				QuantitaScartata = _avanzamentoObserver.QuantitaScartata;
		}

		private void AttivitaStore_OnAttivitaSelezionataChanged()
		{
			IAttivitaViewModel attivitaSelezionata = (IAttivitaViewModel)AttivitaSelezionata;

			if (attivitaSelezionata != null)
				IsFaseCompletabile = Int32.Parse(QuantitaProdotta.ToString()) >= attivitaSelezionata.QuantitaResidua;

			OnNotifyStateChanged();
		}

		public override void Dispose()
		{
			_dialogoOperatoreObserver.OnAttivitaSelezionataChanged -= AttivitaStore_OnAttivitaSelezionataChanged;
			_avanzamentoObserver.OnQuantitaProdottaChanged -= AvanzamentoStore_OnQuantitaChanged;
		}
	}
}
