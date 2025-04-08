using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Domain.Models;
using IMAR_DialogoOperatore.Interfaces.ViewModels;
using System.Collections.ObjectModel;

namespace IMAR_DialogoOperatore.ViewModels
{
    public class OperatoreViewModel : ViewModelBase, IOperatoreViewModel
	{
		private readonly Operatore? _operatore;

		private string _stato;
		private IList<Attivita> _attivitaAperte;

		public int? Badge => _operatore?.Badge != null ? Int32.Parse(_operatore?.Badge) : null;
		public string? Nome => _operatore?.Nome;
		public string? Cognome => _operatore?.Cognome;
		public Macchina? MacchinaAssegnata { get; set; }
		public string Stato
		{
			get { return _stato; }
			set
			{
				_stato = value;
				OnNotifyStateChanged();
			}
		}
		public IList<Attivita> AttivitaAperte
		{
			get { return _attivitaAperte; }
			set
			{
				_attivitaAperte = value;
				OnNotifyStateChanged();
			}
        }

        public OperatoreViewModel(Operatore? operatore)
        {
			_operatore = operatore;

			Stato = string.IsNullOrEmpty(_operatore.Stato) ? Costanti.ASSENTE : _operatore.Stato;
			AttivitaAperte = _operatore.AttivitaAperte == null ?  new ObservableCollection<Attivita>() : new ObservableCollection<Attivita>(_operatore.AttivitaAperte);
        }
    }
}
