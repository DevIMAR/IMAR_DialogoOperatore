using IMAR_DialogoOperatore.Application.Interfaces.ViewModels;
using IMAR_DialogoOperatore.Interfaces.Mappers;
using IMAR_DialogoOperatore.Interfaces.Observers;

namespace IMAR_DialogoOperatore.ViewModels
{
	public class AttivitaGridViewModel : ViewModelBase
	{
		private readonly IDialogoOperatoreObserver _dialogoOperatoreStore;
		private readonly IAttivitaMapper _attivitaMapper;

		private IEnumerable<IAttivitaViewModel>? _attivita;
		private object? _attivitaSelezionata;

		public IEnumerable<IAttivitaViewModel>? Attivita 
		{
			get { return _attivita; }
			set
			{
				_attivita = value;
				OnNotifyStateChanged();
			}
		}
		public object? AttivitaSelezionata 
		{
			get { return _attivitaSelezionata; }
			set
			{
				_attivitaSelezionata = value;

				if (AttivitaSelezionata != _dialogoOperatoreStore.AttivitaSelezionata)
					_dialogoOperatoreStore.AttivitaSelezionata = (IAttivitaViewModel?)AttivitaSelezionata;

				OnNotifyStateChanged();
			}
		}

        public AttivitaGridViewModel(
			IDialogoOperatoreObserver dialogoOperatoreStore,
			IAttivitaMapper attivitaMapper)
        {
			_dialogoOperatoreStore = dialogoOperatoreStore;
			_attivitaMapper = attivitaMapper;

			_dialogoOperatoreStore.OnOperatoreSelezionatoChanged += DialogoOperatoreStore_OnOperatoreSelezionatoChanged;
			_dialogoOperatoreStore.OnAttivitaSelezionataChanged += DialogoOperatoreStore_OnAttivitaSelezionataChanged;
        }

		private void DialogoOperatoreStore_OnAttivitaSelezionataChanged()
		{
			AttivitaSelezionata = _dialogoOperatoreStore.AttivitaSelezionata;
		}

		private void DialogoOperatoreStore_OnOperatoreSelezionatoChanged()
		{
			IOperatoreViewModel? operatore = _dialogoOperatoreStore.OperatoreSelezionato;
			Attivita = operatore != null ? _attivitaMapper.ListaAttivitaToListaAttivitaViewModel(operatore.AttivitaAperte) : Enumerable.Empty<IAttivitaViewModel>();
		}

		public override void Dispose()
		{
			_dialogoOperatoreStore.OnOperatoreSelezionatoChanged -= DialogoOperatoreStore_OnOperatoreSelezionatoChanged;
			_dialogoOperatoreStore.OnAttivitaSelezionataChanged -= DialogoOperatoreStore_OnAttivitaSelezionataChanged;
		}
	}
}
