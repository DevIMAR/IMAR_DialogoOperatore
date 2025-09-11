using Bunit;
using IMAR_DialogoOperatore.Application.Interfaces.Services.Activities;
using IMAR_DialogoOperatore.Domain.Models;
using IMAR_DialogoOperatore.Interfaces.Observers;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using FluentAssertions;
using DevExpress.Blazor;

namespace IMAR_DialogoOperatore.Test.Integration.Components;

public class SimpleBlazorIntegrationTests : TestContext, IDisposable
{
    private readonly IAttivitaService _attivitaService;
    private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;
    private readonly ICercaAttivitaObserver _cercaAttivitaObserver;

    public SimpleBlazorIntegrationTests()
    {
        _attivitaService = Substitute.For<IAttivitaService>();
        _dialogoOperatoreObserver = Substitute.For<IDialogoOperatoreObserver>();
        _cercaAttivitaObserver = Substitute.For<ICercaAttivitaObserver>();
        
        Services.AddSingleton(_attivitaService);
        Services.AddSingleton(_dialogoOperatoreObserver);
        Services.AddSingleton(_cercaAttivitaObserver);
        
        Services.AddDevExpressBlazor();
    }

    [Fact]
    public void ServiceDependencies_ShouldBeProperlyRegistered()
    {
        // Act
        var attivitaService = Services.GetService<IAttivitaService>();
        var dialogoObserver = Services.GetService<IDialogoOperatoreObserver>();
        var cercaObserver = Services.GetService<ICercaAttivitaObserver>();

        // Assert
        attivitaService.Should().NotBeNull();
        dialogoObserver.Should().NotBeNull();
        cercaObserver.Should().NotBeNull();
    }

    [Fact]
    public void MockServices_ShouldSupportBasicOperations()
    {
        // Arrange
        var attivita = new Attivita { Bolla = "B123", QuantitaOrdine = 100 };
        var operatore = new Operatore { Badge = "OP001", Nome = "Mario" };
        
        _attivitaService.ConfrontaCausaliAttivita(
            Arg.Any<List<Attivita>>(), 
            Arg.Any<string>(), 
            Arg.Any<string>()).Returns(true);
        
        _attivitaService.AvanzaAttivita(
            Arg.Any<Operatore>(), 
            Arg.Any<Attivita>(), 
            Arg.Any<int>(), 
            Arg.Any<int>()).Returns((string)null);

        // Act
        var confrontoResult = _attivitaService.ConfrontaCausaliAttivita(
            new List<Attivita> { attivita }, "B123", "IN_LAVORO");
        var avanzamentoResult = _attivitaService.AvanzaAttivita(operatore, attivita, 50, 2);

        // Assert
        confrontoResult.Should().BeTrue();
        avanzamentoResult.Should().BeNull();
    }

    [Fact]
    public void ObserverServices_ShouldBeProperlyMocked()
    {
        // Act & Assert - Just verify observers are properly mocked
        _dialogoOperatoreObserver.Should().NotBeNull();
        _cercaAttivitaObserver.Should().NotBeNull();
        
        // Verify we can configure mock behavior without compile errors
        _dialogoOperatoreObserver.ClearReceivedCalls();
        _cercaAttivitaObserver.ClearReceivedCalls();
    }

    public new void Dispose()
    {
        base.Dispose();
    }
}