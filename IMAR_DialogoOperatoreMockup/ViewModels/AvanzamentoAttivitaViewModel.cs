using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Application.Interfaces.ViewModels;
using IMAR_DialogoOperatore.Interfaces.Observers;

namespace IMAR_DialogoOperatore.ViewModels
{
    public class AvanzamentoAttivitaViewModel : ViewModelBase
	{
		private readonly IAvanzamentoObserver _avanzamentoObserver;
		private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;

		private string _quantitaProdotta;
		private string _quantitaScartata;
		private bool _isFaseCompletabile;

		public object? AttivitaSelezionata => _dialogoOperatoreObserver.AttivitaSelezionata;

		public string QuantitaProdotta 
		{ 
			get {  return _quantitaProdotta; }
			set
			{
				_quantitaProdotta = value;

				if (Int32.TryParse(_quantitaProdotta, out int quantitaProdotta) && AttivitaSelezionata != null)
				{
					IsFaseCompletabile = Int32.Parse(_quantitaProdotta) >= ((IAttivitaViewModel)AttivitaSelezionata).QuantitaResidua;
					_avanzamentoObserver.QuantitaProdotta = quantitaProdotta;
				}

				OnNotifyStateChanged();
			}
		}
        public string QuantitaScartata
		{ 
			get {  return _quantitaScartata; }
			set
			{
				_quantitaScartata = value;

				if (Int32.TryParse(_quantitaScartata, out int quantitaScartata) && AttivitaSelezionata != null)
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

			QuantitaProdotta = "0";
			QuantitaScartata = "0";

			_dialogoOperatoreObserver.OnAttivitaSelezionataChanged += AttivitaStore_OnAttivitaSelezionataChanged;
			_avanzamentoObserver.OnQuantitaProdottaChanged += AvanzamentoStore_OnQuantitaChanged;
			_avanzamentoObserver.OnQuantitaScartataChanged += AvanzamentoStore_OnQuantitaChanged;
		}

		private void AvanzamentoStore_OnQuantitaChanged()
		{
			if (QuantitaProdotta != _avanzamentoObserver.QuantitaProdotta.ToString())
				QuantitaProdotta = _avanzamentoObserver.QuantitaProdotta.ToString();

			if (QuantitaScartata != _avanzamentoObserver.QuantitaScartata.ToString())
				QuantitaScartata = _avanzamentoObserver.QuantitaScartata.ToString();
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
