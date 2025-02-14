using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Domain.Models;
using IMAR_DialogoOperatore.Helpers;
using IMAR_DialogoOperatore.Interfaces.Helpers;
using IMAR_DialogoOperatore.Observers;
using IMAR_DialogoOperatore.ViewModels;
using NSubstitute;

namespace IMAR_DialogoOperatore.Test.Utilities
{
	public class InterruzioneAttivitaUtilityTest
	{
		private readonly DialogoOperatoreObserver _dialogoOperatoreStore;
		private readonly PopupObserver _popupStore;
		private AttivitaViewModel _mockAttivita;
		private IInterruzioneAttivitaHelper _interruzioneAttivitaHelper;

		public InterruzioneAttivitaUtilityTest()
		{
			_dialogoOperatoreStore = Substitute.For<DialogoOperatoreObserver>();
			_dialogoOperatoreStore.IsOperazioneAnnullata = false;

			_popupStore = Substitute.For<PopupObserver>();
			_popupStore.IsPopupVisible = true;
			_popupStore.IsConfermato = false;

			_mockAttivita = new AttivitaViewModel(new Attivita
			{
				Causale = Costanti.IN_LAVORO
			});

			_interruzioneAttivitaHelper = new InterruzioneAttivitaHelper(_dialogoOperatoreStore, _popupStore);
		}

		[Fact]
		public async Task GestisciInterruzioneAttivita_ChiusuraLavoro_AttendeChiusuraAttivita()
		{
			// Arrange

			// Act
			var task = _interruzioneAttivitaHelper.GestisciInterruzioneAttivita(_mockAttivita, isUscita: true);

			_popupStore.IsConfermato = true;
			_popupStore.IsPopupVisible = false;
			await task;

			// Assert
			Assert.Equal(Costanti.FINE_LAVORO, _dialogoOperatoreStore.OperazioneInCorso);
			Assert.Equal(_mockAttivita, _dialogoOperatoreStore.AttivitaSelezionata);
			_popupStore.Received().OnIsPopupVisibleChanged += Arg.Any<Action>();
		}

		[Fact]
		public async Task GestisciInterruzioneAttivita_AvanzamentoLavoro_AttendeAvanzamentoAttivita()
		{
			// Arrange

			// Act
			var task = _interruzioneAttivitaHelper.GestisciInterruzioneAttivita(_mockAttivita, isUscita: false);

			_popupStore.IsConfermato = true;
			_popupStore.IsPopupVisible = false;
			await task;

			// Assert
			Assert.Equal(Costanti.AVANZAMENTO, _dialogoOperatoreStore.OperazioneInCorso);
			Assert.Equal(_mockAttivita, _dialogoOperatoreStore.AttivitaSelezionata);
			_popupStore.Received().OnIsPopupVisibleChanged += Arg.Any<Action>();
		}

		[Fact]
		public async Task GestisciInterruzioneAttivita_Attrezzaggio_AttendeFineAttrezzaggio()
		{
			// Arrange
			_mockAttivita.Causale = Costanti.IN_ATTREZZAGGIO;

			// Act
			var task = _interruzioneAttivitaHelper.GestisciInterruzioneAttivita(_mockAttivita, isUscita: true);

			_dialogoOperatoreStore.OperazioneInCorso = Costanti.NESSUNA;
			await task;

			// Assert
			Assert.Equal(Costanti.NESSUNA, _dialogoOperatoreStore.OperazioneInCorso);
			Assert.Equal(_mockAttivita, _dialogoOperatoreStore.AttivitaSelezionata);
			_dialogoOperatoreStore.Received().OnOperazioneInCorsoChanged += Arg.Any<Action>();
		}

		[Fact]
		public async Task DialogoOperatoreStore_OnIsOperazioneAnnullataChanged_AnnullamentoSuccess()
		{
			// Arrange
			_dialogoOperatoreStore.IsOperazioneAnnullata = true;

			// Act
			var task = _interruzioneAttivitaHelper.GestisciInterruzioneAttivita(_mockAttivita, isUscita: false);

			_dialogoOperatoreStore.IsOperazioneAnnullata = true;
			await task;

			// Assert
			Assert.True(_dialogoOperatoreStore.IsOperazioneAnnullata);
			Assert.True(task.IsCompleted);
		}

		[Fact]
		public async Task PopupStore_OnIsPopupVisibleChanged_ConfirmsAndClosesActivity()
		{
			// Arrange
			_popupStore.IsPopupVisible = true;
			_popupStore.IsConfermato = true;

			// Act
			var task = _interruzioneAttivitaHelper.GestisciInterruzioneAttivita(_mockAttivita, isUscita: true);

			_popupStore.IsPopupVisible = false;
			await task;

			// Assert
			Assert.True(task.IsCompleted);
		}
	}
}
