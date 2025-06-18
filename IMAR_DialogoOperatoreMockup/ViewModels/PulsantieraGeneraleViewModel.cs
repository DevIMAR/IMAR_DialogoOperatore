using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Commands;
using IMAR_DialogoOperatore.Interfaces.Observers;
using IMAR_DialogoOperatore.Interfaces.ViewModels;
using System.Windows.Input;

namespace IMAR_DialogoOperatore.ViewModels
{
    public class PulsantieraGeneraleViewModel : ViewModelBase
	{
		private IDialogoOperatoreObserver _dialogoOperatoreObserver;
		private IOperatoreViewModel? _operatoreSelezionato;
		public IAttivitaViewModel? AttivitaSelezionata => _dialogoOperatoreObserver.AttivitaSelezionata;
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
			IDialogoOperatoreObserver dialogoOperatoreObserver,
			IngressoUscitaCommand ingressoUscitaCommand,
			InizioFinePausaCommand inizioFinePausaCommand,
			InizioLavoroCommand inizioLavoroCommand,
			AvanzamentoCommand avanzamentoCommand,
			InizioAttrezzaggioCommand inizioAttrezzaggioCommand,
			FineAttrezzaggioCommand fineAttrezzaggioCommand,
			FineLavoroCommand fineLavoroCommand,
			AnnullaOperazioneCommand annullaOperazioneCommand)
        {
			_dialogoOperatoreObserver = dialogoOperatoreObserver;

            IngressoUscitaCommand = ingressoUscitaCommand;
			InizioFinePausaCommand = inizioFinePausaCommand;
			InizioLavoroCommand = inizioLavoroCommand;
			AvanzamentoCommand = avanzamentoCommand;
			InizioAttrezzaggioCommand = inizioAttrezzaggioCommand;
			FineAttrezzaggioCommand = fineAttrezzaggioCommand;
			FineLavoroCommand = fineLavoroCommand;
			AnnulaOperazioneCommand = annullaOperazioneCommand;

			_dialogoOperatoreObserver.OnAttivitaSelezionataChanged += DialogoOperatoreObserver_OnAttivitaSelezionataChanged;
			_dialogoOperatoreObserver.OnOperatoreSelezionatoChanged += DialogoOperatoreObserver_OnOperatoreSelezionatoChanged;
            _dialogoOperatoreObserver.OnIsDettaglioAttivitaOpenChanged += DialogoOperatoreObserver_OnIsDettaglioAttivitaOpenChanged;
        }

		private void DialogoOperatoreObserver_OnAttivitaSelezionataChanged()
		{
			OnNotifyStateChanged();
		}

		private void DialogoOperatoreObserver_OnOperatoreSelezionatoChanged()
		{
			OperatoreSelezionato = _dialogoOperatoreObserver.OperatoreSelezionato;
		}

		private void DialogoOperatoreObserver_OnIsDettaglioAttivitaOpenChanged()
        {
            OnNotifyStateChanged();
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
			_dialogoOperatoreObserver.OnAttivitaSelezionataChanged -= DialogoOperatoreObserver_OnAttivitaSelezionataChanged;
			_dialogoOperatoreObserver.OnOperatoreSelezionatoChanged -= DialogoOperatoreObserver_OnOperatoreSelezionatoChanged;
            _dialogoOperatoreObserver.OnIsDettaglioAttivitaOpenChanged -= DialogoOperatoreObserver_OnIsDettaglioAttivitaOpenChanged;
        }
	}
}
