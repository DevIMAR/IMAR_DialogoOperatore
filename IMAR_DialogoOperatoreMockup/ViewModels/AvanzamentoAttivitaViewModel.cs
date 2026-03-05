using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Interfaces.Observers;
using IMAR_DialogoOperatore.Interfaces.ViewModels;
using IMAR_DialogoOperatore.Observers;

namespace IMAR_DialogoOperatore.ViewModels
{
    public class AvanzamentoAttivitaViewModel : ViewModelBase
	{
		private readonly IAvanzamentoObserver _avanzamentoObserver;
		private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;
		private readonly ITaskCompilerObserver _taskCompilerObserver;

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

				if (UInt32.TryParse(_quantitaProdotta.ToString(), out uint quantitaProdotta) || value == null)
				{
					_avanzamentoObserver.QuantitaProdotta = quantitaProdotta;
					if (AttivitaSelezionata != null)
					{
						var attivita = (IAttivitaViewModel)AttivitaSelezionata;
						int quantitaTotaleFase = attivita.QuantitaProdotta + (int)quantitaProdotta + attivita.QuantitaScartata + (int)(_avanzamentoObserver.QuantitaScartata ?? 0);
						IsFaseCompletabile = quantitaTotaleFase >= attivita.QuantitaOrdineOriginale;
					}
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

				if (UInt32.TryParse(_quantitaScartata.ToString(), out uint quantitaScartata) || value == null)
				{
					_avanzamentoObserver.QuantitaScartata = quantitaScartata;
					if (AttivitaSelezionata != null)
					{
						var attivita = (IAttivitaViewModel)AttivitaSelezionata;
						int quantitaTotaleFase = attivita.QuantitaProdotta + (int)(_avanzamentoObserver.QuantitaProdotta ?? 0) + attivita.QuantitaScartata + (int)quantitaScartata;
						IsFaseCompletabile = quantitaTotaleFase >= attivita.QuantitaOrdineOriginale;
					}
				}

				OnNotifyStateChanged();
			}
		}
		public bool IsFaseCompletabile
		{
			get { return _isFaseCompletabile; }
			set 
			{ 
				_isFaseCompletabile = _dialogoOperatoreObserver.AttivitaSelezionata?.SaldoAcconto == Costanti.SALDO ? true :
									  _taskCompilerObserver.EventoSelezionato?.SaldoAcconto == Costanti.SALDO ? true :
											(_dialogoOperatoreObserver?.AttivitaSelezionata?.QuantitaProdotta + _dialogoOperatoreObserver?.AttivitaSelezionata?.QuantitaScartata > 0
											|| _avanzamentoObserver.QuantitaProdotta + _avanzamentoObserver.QuantitaScartata > 0
											|| _taskCompilerObserver.EventoSelezionato?.QuantitaProdotta + _taskCompilerObserver.EventoSelezionato?.QuantitaScartata > 0)
											&& value;

				_avanzamentoObserver.SaldoAcconto = _isFaseCompletabile ? Costanti.SALDO : Costanti.ACCONTO;

                OnNotifyStateChanged();
			}
		}

		public AvanzamentoAttivitaViewModel(
			IDialogoOperatoreObserver dialogoOperatoreObserver, 
			IAvanzamentoObserver avanzamentoObserver,
			ITaskCompilerObserver taskCompilerObserver)
		{
			_dialogoOperatoreObserver = dialogoOperatoreObserver;
			_avanzamentoObserver = avanzamentoObserver;
			_taskCompilerObserver = taskCompilerObserver;

			QuantitaProdotta = 0;
			_avanzamentoObserver.QuantitaProdotta = 0;
            QuantitaScartata = 0;
			_avanzamentoObserver.QuantitaScartata = 0;


            _dialogoOperatoreObserver.OnAttivitaSelezionataChanged += AttivitaStore_OnAttivitaSelezionataChanged;
            _dialogoOperatoreObserver.OnIsDettaglioAttivitaOpenChanged += DialogoOperatoreObserver_OnIsDettaglioAttivitaOpenChanged;

			_avanzamentoObserver.OnQuantitaProdottaChanged += AvanzamentoStore_OnQuantitaChanged;
			_avanzamentoObserver.OnQuantitaScartataChanged += AvanzamentoStore_OnQuantitaChanged;
		}

        private void DialogoOperatoreObserver_OnIsDettaglioAttivitaOpenChanged()
        {
            QuantitaProdotta = 0;
            _avanzamentoObserver.QuantitaProdotta = 0;
            QuantitaScartata = 0;
            _avanzamentoObserver.QuantitaScartata = 0;
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
			{
				int quantitaResiduaOriginale = attivitaSelezionata.QuantitaOrdineOriginale - attivitaSelezionata.QuantitaProdotta;
				IsFaseCompletabile = Int32.Parse(QuantitaProdotta.ToString()) >= quantitaResiduaOriginale;
			}

			OnNotifyStateChanged();
		}

		public override void Dispose()
		{
			_dialogoOperatoreObserver.OnAttivitaSelezionataChanged -= AttivitaStore_OnAttivitaSelezionataChanged;
			_avanzamentoObserver.OnQuantitaProdottaChanged -= AvanzamentoStore_OnQuantitaChanged;
		}
	}
}
