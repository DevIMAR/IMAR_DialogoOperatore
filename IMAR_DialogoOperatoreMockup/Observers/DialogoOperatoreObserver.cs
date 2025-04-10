﻿using IMAR_DialogoOperatore.Interfaces.Observers;
using IMAR_DialogoOperatore.Interfaces.ViewModels;

namespace IMAR_DialogoOperatore.Observers
{
    public class DialogoOperatoreObserver : ObserverBase, IDialogoOperatoreObserver
	{
		private IEnumerable<IAttivitaViewModel>? _listaAttivita;
		private IAttivitaViewModel? _attivitaSelezionata;
		private IOperatoreViewModel? _operatoreSelezionato;
		private string? _operazioneInCorso;
		private bool _isUscita;
		private bool _isLoaderVisibile;
		private bool _isDettaglioAttivitaOpen;
		private bool _isOperazioneAnnullata;
		private bool _isRiaperturaAttiva;
		private bool _isOperazioneGestita;
		private bool _isAperturaLavoroAutomaticaAttiva;

		public IEnumerable<IAttivitaViewModel>? ListaAttivita
		{
			get { return _listaAttivita; }
			set
			{
				_listaAttivita = value;
				CallAction(OnListaAttivitaChanged);
			}
		}
		public IAttivitaViewModel? AttivitaSelezionata
		{
			get { return _attivitaSelezionata; }
			set
			{
				_attivitaSelezionata = value;
				CallAction(OnAttivitaSelezionataChanged);
			}
		}

		public IOperatoreViewModel? OperatoreSelezionato
		{
			get { return _operatoreSelezionato; }
			set
			{
				_operatoreSelezionato = value;
				CallAction(OnOperatoreSelezionatoChanged);
			}
		}
		public string? OperazioneInCorso
		{
			get { return _operazioneInCorso; }
			set
			{
				_operazioneInCorso = value;
				CallAction(OnOperazioneInCorsoChanged);
			}
		}
		public bool IsUscita
		{
			get { return _isUscita; }
			set
			{
                _isUscita = value;
				CallAction(OnIsUscitaChanged);
			}
        }
        public bool IsLoaderVisibile 
		{ 
			get { return _isLoaderVisibile; }
			set
			{
                _isLoaderVisibile = value;
                CallAction(OnIsLoaderVisibileChanged);
            }
		}
        public bool IsDettaglioAttivitaOpen
		{
			get { return _isDettaglioAttivitaOpen; }
			set
			{
				if (_isDettaglioAttivitaOpen == value)
					return;

				_isDettaglioAttivitaOpen = value;
				CallAction(OnIsDettaglioAttivitaOpenChanged);
			}
		}
		public bool IsOperazioneAnnullata
		{
			get { return _isOperazioneAnnullata; }
			set
			{
				_isOperazioneAnnullata = value;
				CallAction(OnIsOperazioneAnnullataChanged);
			}
		}
        public bool IsRiaperturaAttiva 
		{
			get { return _isRiaperturaAttiva; } 
			set
			{
				_isRiaperturaAttiva = value;
				CallAction(OnIsRiaperturaAttivaChanged);
            }
		}
        public bool IsOperazioneGestita
        {
			get { return _isOperazioneGestita; } 
			set
			{
                _isOperazioneGestita = value;
				CallAction(OnIsOperazioneGestitaChanged);
            }
		}

        public bool IsAperturaLavoroAutomaticaAttiva 
		{ 
			get { return _isAperturaLavoroAutomaticaAttiva; }
			set
			{
                _isAperturaLavoroAutomaticaAttiva = value;
				CallAction(OnIsAperturaLavoroAutomaticaAttivaChanged);
            }
		}

        public event Action? OnListaAttivitaChanged;
		public event Action? OnAttivitaSelezionataChanged;
		public event Action? OnOperatoreSelezionatoChanged;
		public event Action? OnOperazioneInCorsoChanged;
		public event Action? OnIsUscitaChanged;
        public event Action? OnIsLoaderVisibileChanged;
		public event Action? OnIsDettaglioAttivitaOpenChanged;
		public event Action? OnIsOperazioneAnnullataChanged;
        public event Action? OnIsRiaperturaAttivaChanged;
        public event Action? OnIsOperazioneGestitaChanged;
        public event Action? OnIsAperturaLavoroAutomaticaAttivaChanged;
    }
}