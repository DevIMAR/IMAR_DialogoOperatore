using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Application.Interfaces.ViewModels;
using IMAR_DialogoOperatore.Commands;
using IMAR_DialogoOperatore.Interfaces.Observers;
using System.Windows.Input;

namespace IMAR_DialogoOperatore.ViewModels
{
	public class PulsantieraGeneraleViewModel : ViewModelBase
	{
		private IDialogoOperatoreObserver _dialogoOperatoreStore;
		private IOperatoreViewModel? _operatoreSelezionato;
		public IAttivitaViewModel? AttivitaSelezionata => _dialogoOperatoreStore.AttivitaSelezionata;
		public bool IsAssente => OperatoreSelezionato != null ? OperatoreSelezionato.Stato == Costanti.ASSENTE : true;
		public bool IsInPausa => OperatoreSelezionato != null ? OperatoreSelezionato.Stato == Costanti.IN_PAUSA : false;

		public ICommand IngressoUscitaCommand { get; set; }
		public ICommand InizioFinePausaCommand {  get; set; }
		public ICommand InizioLavoroCommand { get; set; }
		public ICommand AvanzamentoCommand { get; set; }
		public ICommand InizioAttrezzaggioCommand { get; set; }
		public ICommand FineAttrezzaggioCommand { get; set; }
		public ICommand FineLavoroCommand { get; set; }
		public ICommand AnnulaOperazioneCommand { get; set; }

		public IOperatoreViewModel? OperatoreSelezionato 
		{
			get { return _operatoreSelezionato; }
			set
			{
				UnsubscribeEventoOperatore();

				UpdateOperatoreSelezionato(value);

				OnNotifyStateChanged();
			}
		}

		public PulsantieraGeneraleViewModel(
			IDialogoOperatoreObserver dialogoOperatoreStore,
			IngressoUscitaCommand ingressoUscitaCommand,
			InizioFinePausaCommand inizioFinePausaCommand,
			InizioLavoroCommand inizioLavoroCommand,
			AvanzamentoCommand avanzamentoCommand,
			InizioAttrezzaggioCommand inizioAttrezzaggioCommand,
			FineAttrezzaggioCommand fineAttrezzaggioCommand,
			FineLavoroCommand fineLavoroCommand,
			AnnullaOperazioneCommand annullaOperazioneCommand)
        {
			_dialogoOperatoreStore = dialogoOperatoreStore;

            IngressoUscitaCommand = ingressoUscitaCommand;
			InizioFinePausaCommand = inizioFinePausaCommand;
			InizioLavoroCommand = inizioLavoroCommand;
			AvanzamentoCommand = avanzamentoCommand;
			InizioAttrezzaggioCommand = inizioAttrezzaggioCommand;
			FineAttrezzaggioCommand = fineAttrezzaggioCommand;
			FineLavoroCommand = fineLavoroCommand;
			AnnulaOperazioneCommand = annullaOperazioneCommand;

			_dialogoOperatoreStore.OnAttivitaSelezionataChanged += DialogoOperatoreStore_OnAttivitaSelezionataChanged;
			_dialogoOperatoreStore.OnOperatoreSelezionatoChanged += DialogoOperatoreStore_OnOperatoreSelezionatoChanged;
        }

		private void DialogoOperatoreStore_OnAttivitaSelezionataChanged()
		{
			OnNotifyStateChanged();
		}

		private void DialogoOperatoreStore_OnOperatoreSelezionatoChanged()
		{
			OperatoreSelezionato = _dialogoOperatoreStore.OperatoreSelezionato;
		}

		private void OperatoreSelezionato_NotifyStateChanged()
		{
			OnNotifyStateChanged();
		}

		private void UnsubscribeEventoOperatore()
		{
			if (OperatoreSelezionato != null)
				((OperatoreViewModel)OperatoreSelezionato).NotifyStateChanged -= OperatoreSelezionato_NotifyStateChanged;
		}

		private void UpdateOperatoreSelezionato(IOperatoreViewModel? value)
		{
			_operatoreSelezionato = value;

			if (OperatoreSelezionato != null)
			{
				((OperatoreViewModel)OperatoreSelezionato).NotifyStateChanged += OperatoreSelezionato_NotifyStateChanged;
			}
		}

		public override void Dispose()
		{
			_dialogoOperatoreStore.OnAttivitaSelezionataChanged -= DialogoOperatoreStore_OnAttivitaSelezionataChanged;
			_dialogoOperatoreStore.OnOperatoreSelezionatoChanged -= DialogoOperatoreStore_OnOperatoreSelezionatoChanged;
		}
	}
}
