using IMAR_DialogoOperatore.Application.Interfaces.Services.Activities;
using IMAR_DialogoOperatore.Domain.Models;
using IMAR_DialogoOperatore.Helpers;
using IMAR_DialogoOperatore.Interfaces.Helpers;
using IMAR_DialogoOperatore.Interfaces.Observers;
using IMAR_DialogoOperatore.Mappers;
using IMAR_DialogoOperatore.ViewModels;
using NSubstitute;

namespace IMAR_DialogoOperatore.Test.Utilities
{
    public class CercaAttivitaUtilityTest
	{
		private ICercaAttivitaHelper _cercaAttivitaHelper;
		private IDialogoOperatoreObserver _dialogoOperatoreObserver;
		private ICercaAttivitaObserver _cercaAttivitaObserver;
		private IAttivitaService _attivitaService;
		private List<Attivita> _mockAttivita;
		private AttivitaMapper _attivitaMapper;

		public CercaAttivitaUtilityTest()
		{
			_dialogoOperatoreObserver = Substitute.For<IDialogoOperatoreObserver>();
			_cercaAttivitaObserver = Substitute.For <ICercaAttivitaObserver>();
			_attivitaService = Substitute.For<IAttivitaService>();
			_attivitaMapper = Substitute.For<AttivitaMapper>();

			_mockAttivita = new List<Attivita>
		{
			new Attivita { Bolla = "B001", Odp = "O001", Fase = "F001" },
			new Attivita { Bolla = "B002", Odp = "O001", Fase = "F002" },
			new Attivita { Bolla = "B003", Odp = "O002", Fase = "F003" }
		};

			_attivitaService.Attivita.Returns(_mockAttivita);

			_cercaAttivitaHelper = new CercaAttivitaHelper(_dialogoOperatoreObserver, _cercaAttivitaObserver, _attivitaService, _attivitaMapper);
		}

		[Fact]
		public void CercaAttivita_CallsCercaAttivitaDaBolla_WhenBollaIsProvided()
		{
			//Arrange

			// Act
			_cercaAttivitaHelper.CercaAttivita(bolla: "B001");

			// Assert
			Assert.True(_cercaAttivitaObserver.IsAttivitaCercata);
			Assert.Equal("B001", _dialogoOperatoreObserver.AttivitaSelezionata?.Bolla);
		}

		[Fact]
		public void CercaAttivita_CallsCercaAttivitaDaOdp_WhenOdpIsProvided()
		{
			//Arrange

			// Act
			_cercaAttivitaHelper.CercaAttivita(odp: "O002");

			// Assert
			Assert.True(_cercaAttivitaObserver.IsAttivitaCercata);
			Assert.Equal("O002", _dialogoOperatoreObserver.AttivitaSelezionata?.Odp);
		}

		[Fact]
		public void CercaAttivitaDaBolla_SetsAttivitaSelezionata_WhenBollaExists()
		{
			//Arrange

			// Act
			_cercaAttivitaHelper.CercaAttivitaDaBolla("B001");

			// Assert
			Assert.True(_cercaAttivitaObserver.IsAttivitaCercata);
			Assert.Equal("B001", _dialogoOperatoreObserver.AttivitaSelezionata?.Bolla);
		}

		[Fact]
		public void CercaAttivitaDaBolla_DoesNotSetAttivitaSelezionata_WhenBollaDoesNotExist()
		{
			//Arrange

			// Act
			_cercaAttivitaHelper.CercaAttivitaDaBolla("B004");

			// Assert
			Assert.True(_cercaAttivitaObserver.IsAttivitaCercata);
			Assert.Null(_dialogoOperatoreObserver.AttivitaSelezionata);
		}

		[Fact]
		public void CercaAttivitaDaOdp_SetsAttivitaTrovate_WhenOdpExists()
		{
			//Arrange

			// Act
			_cercaAttivitaHelper.CercaAttivitaDaOdp("O001");

			// Assert
			Assert.True(_cercaAttivitaObserver.IsAttivitaCercata);
			Assert.Equal(2, _cercaAttivitaObserver.AttivitaTrovate.Count());
			Assert.Equal("O001", _dialogoOperatoreObserver.AttivitaSelezionata?.Odp);
		}

		[Fact]
		public void CercaAttivitaDaOdp_SetsAttivitaTrovateToEmpty_WhenOdpDoesNotExist()
		{
			//Arrange

			// Act
			_cercaAttivitaHelper.CercaAttivitaDaOdp("O003");

			// Assert
			Assert.True(_cercaAttivitaObserver.IsAttivitaCercata);
			Assert.Empty(_cercaAttivitaObserver.AttivitaTrovate);
			Assert.Null(_dialogoOperatoreObserver.AttivitaSelezionata);
		}

		[Fact]
		public void CercaAttivitaDaFase_SetsAttivitaSelezionata_WhenFaseExists()
		{
			//Arrange
			_cercaAttivitaObserver.AttivitaTrovate = _attivitaMapper.ListaAttivitaToListaAttivitaViewModel(_mockAttivita);

			// Act
			_cercaAttivitaHelper.CercaAttivitaDaFase("F002");

			// Assert
			Assert.True(_cercaAttivitaObserver.IsAttivitaCercata);
			Assert.Equal("F002", _dialogoOperatoreObserver.AttivitaSelezionata?.Fase);
		}

		[Fact]
		public void CercaAttivitaDaFase_DoesNotAttivitaSelezionata_WhenFaseDoesNotExist()
		{
			//Arrange
			_cercaAttivitaObserver.AttivitaTrovate = _attivitaMapper.ListaAttivitaToListaAttivitaViewModel(_mockAttivita);

			// Act & Assert
			Assert.Throws<InvalidOperationException>(() => _cercaAttivitaHelper.CercaAttivitaDaFase("F004"));
		}

		[Fact]
		public void CercaAttivitaDaFase_ThrowsException_WhenMultipleFasiExist()
		{
			// Arrange
			_mockAttivita.Add(new Attivita { Bolla = "B004", Odp = "O001", Fase = "F002" });
			_cercaAttivitaObserver.AttivitaTrovate = _attivitaMapper.ListaAttivitaToListaAttivitaViewModel(_mockAttivita);

			// Act & Assert
			Assert.Throws<InvalidOperationException>(() => _cercaAttivitaHelper.CercaAttivitaDaFase("F002"));
		}
	}

}
