﻿using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Application.Interfaces.Utilities;
using IMAR_DialogoOperatore.Commands;
using IMAR_DialogoOperatore.Interfaces.Helpers;
using IMAR_DialogoOperatore.Interfaces.Observers;
using IMAR_DialogoOperatore.Interfaces.ViewModels;
using System.Windows.Input;

namespace IMAR_DialogoOperatore.ViewModels
{
    public class AttivitaDetailsViewModel : ViewModelBase
	{
		private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;
		private readonly ICercaAttivitaObserver _cercaAttivitaObserver;
		private readonly IAutoLogoutUtility _autoLogoutUtility;
		private IAttivitaViewModel? _attivitaSelezionata;

		private string? _bolla;
		private string? _odp;
		private IEnumerable<string>? _fasiPerAttivita;
		private string _faseSelezionata;
		private bool _isRiaperturaAttiva;
		private bool _isAperturaLavoroAutomaticaAttiva;

        public ICercaAttivitaHelper CercaAttivitaHelper { get; private set; }
		public ICommand ApriListaIndirette { get; private set; }

        public string? Articolo => _attivitaSelezionata != null ? _attivitaSelezionata.Articolo : string.Empty;
		public string? DescrizioneArticolo => _attivitaSelezionata != null ? _attivitaSelezionata.DescrizioneArticolo : string.Empty;
		public string? DescrizioneFase => _attivitaSelezionata != null ? _attivitaSelezionata.DescrizioneFase : string.Empty;
		public bool IsAvanzamento => _dialogoOperatoreObserver.OperazioneInCorso == Costanti.AVANZAMENTO || _dialogoOperatoreObserver.OperazioneInCorso == Costanti.FINE_LAVORO;
		public bool IsFineAttivitaInUscita => (_dialogoOperatoreObserver.OperazioneInCorso == Costanti.FINE_LAVORO || _dialogoOperatoreObserver.OperazioneInCorso == Costanti.FINE_ATTREZZAGGIO) && _dialogoOperatoreObserver.IsUscita;
		public bool IsFineAttrezzaggio => _dialogoOperatoreObserver.OperazioneInCorso == Costanti.FINE_ATTREZZAGGIO && !_dialogoOperatoreObserver.IsUscita;
		public double QuantitaOrdine => _attivitaSelezionata != null ? (double)_attivitaSelezionata.QuantitaOrdine : 0;
		public double QuantitaProdottaPrecedentemente => _attivitaSelezionata != null ? QuantitaOrdine - (double)_attivitaSelezionata.QuantitaResidua : 0;
		public string CompletamentoFase => QuantitaProdottaPrecedentemente.ToString() + "/" + QuantitaOrdine.ToString();
		public string QuantitaResidua => _attivitaSelezionata != null ? _attivitaSelezionata.QuantitaResidua.ToString() : "0";
		public string QuantitaScartataTotale => _attivitaSelezionata != null ? _attivitaSelezionata.QuantitaScartata.ToString() : "0";
		public string StatoAttivita => (_attivitaSelezionata != null && _attivitaSelezionata.SaldoAcconto == Costanti.SALDO) ? Costanti.ATTIVITA_COMPLETATA : Costanti.ATTIVITA_NON_COMPLETATA;
		public bool IsAttivitaSelezionata => _dialogoOperatoreObserver.AttivitaSelezionata?.Bolla == Bolla &&
											 _dialogoOperatoreObserver.AttivitaSelezionata?.Odp == Odp;


        public string? Bolla
		{
			get { return _bolla; }
			set
			{
				if (_bolla == value)
					return;

				_bolla = value;

                OnNotifyStateChanged();
			}
		}

		public string? Odp
		{
			get { return _odp; }
			set
			{
				if (_odp == value)
					return;

				_odp = value;

                OnNotifyStateChanged();
			}
		}

		public IEnumerable<string>? FasiPerAttivita
		{
			get { return _fasiPerAttivita; }
			private set
			{
				_fasiPerAttivita = value;
				OnNotifyStateChanged();
			}
		}
		public string FaseSelezionata
		{
			get { return _faseSelezionata; }
			set
			{
				_faseSelezionata = value;

                CercaAttivitaHelper.CercaAttivitaDaFase(value);

                OnNotifyStateChanged();
			}
		}

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
			IDialogoOperatoreObserver dialogoOperatoreStore,
			ICercaAttivitaObserver cercaAttivitaStore,
			ICercaAttivitaHelper cercaAttivitaUtility,
			IAutoLogoutUtility autoLogoutUtility,
            MostraIndiretteCommand mostraIndiretteCommand)
        {
            _dialogoOperatoreObserver = dialogoOperatoreStore;
			_cercaAttivitaObserver = cercaAttivitaStore;

            CercaAttivitaHelper = cercaAttivitaUtility;
            _autoLogoutUtility = autoLogoutUtility;

            ApriListaIndirette = mostraIndiretteCommand;

            _dialogoOperatoreObserver.OnAttivitaSelezionataChanged += DialogoOperatoreStore_OnAttivitaSelezionataChanged;
			_dialogoOperatoreObserver.OnOperazioneInCorsoChanged += DialogoOperatoreStore_OnOperazioneInCorsoChanged;
            _dialogoOperatoreObserver.OnIsRiaperturaAttivaChanged += DialogoOperatoreObserver_OnIsRiaperturaAttivaChanged;
            _dialogoOperatoreObserver.OnIsDettaglioAttivitaOpenChanged += DialogoOperatoreObserver_OnIsDettaglioAttivitaOpenChanged;

            _cercaAttivitaObserver.OnAttivitaTrovateChanged += CercaAttivitaStore_OnAttivitaTrovateChanged;
        }

        private void DialogoOperatoreObserver_OnIsRiaperturaAttivaChanged()
        {
            _isRiaperturaAttiva = _dialogoOperatoreObserver.IsRiaperturaAttiva;
        }

        private void DialogoOperatoreStore_OnOperazioneInCorsoChanged()
		{
			OnNotifyStateChanged();
		}

		private void CercaAttivitaStore_OnAttivitaTrovateChanged()
		{
			IEnumerable<IAttivitaViewModel>? attivitaTrovate = _cercaAttivitaObserver.AttivitaTrovate;

			if (!attivitaTrovate.Any())
				return;

			FasiPerAttivita = _cercaAttivitaObserver.AttivitaTrovate.Select(x => x.Fase);
        }

        private void DialogoOperatoreObserver_OnIsDettaglioAttivitaOpenChanged()
        {
            Bolla = null;
            Odp = null;
			_cercaAttivitaObserver.FaseCercata = string.Empty;

            _autoLogoutUtility.StartLogoutTimer(300);
        }

        private void DialogoOperatoreStore_OnAttivitaSelezionataChanged()
		{
			_attivitaSelezionata = _dialogoOperatoreObserver.AttivitaSelezionata;

			if (_attivitaSelezionata == null)
            {
                _fasiPerAttivita = new List<string>();
				_faseSelezionata = null;
				_isRiaperturaAttiva = false;
				_isAperturaLavoroAutomaticaAttiva = false;

				return;
			}

			if (!_cercaAttivitaObserver.IsAttivitaCercata)
				_fasiPerAttivita = new List<string> { _attivitaSelezionata.Fase };

			_faseSelezionata = _attivitaSelezionata.Fase;
			_odp = _attivitaSelezionata.Odp ?? string.Empty;
            _bolla = _attivitaSelezionata.Bolla ?? string.Empty;

			OnNotifyStateChanged();
		}

        public override void Dispose()
        {
            _dialogoOperatoreObserver.OnAttivitaSelezionataChanged -= DialogoOperatoreStore_OnAttivitaSelezionataChanged;
            _dialogoOperatoreObserver.OnOperazioneInCorsoChanged -= DialogoOperatoreStore_OnOperazioneInCorsoChanged;
            _dialogoOperatoreObserver.OnIsRiaperturaAttivaChanged -= DialogoOperatoreObserver_OnIsRiaperturaAttivaChanged;
            _dialogoOperatoreObserver.OnIsDettaglioAttivitaOpenChanged -= DialogoOperatoreObserver_OnIsDettaglioAttivitaOpenChanged;
        }
	}
}
