using IMAR_DialogoOperatore.Commands;
using IMAR_DialogoOperatore.Interfaces.Observers;
using IMAR_DialogoOperatore.Interfaces.ViewModels;
using IMAR_DialogoOperatore.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System.Windows.Input;

namespace IMAR_DialogoOperatore.Test.Helpers;

public static class BlazorTestHelper
{
    public static void ConfigureBlazorServices(IServiceCollection services)
    {
        // Mock Observers
        var dialogoOperatoreObserver = Substitute.For<IDialogoOperatoreObserver>();
        var cercaAttivitaObserver = Substitute.For<ICercaAttivitaObserver>();
        
        services.AddSingleton(dialogoOperatoreObserver);
        services.AddSingleton(cercaAttivitaObserver);

        // For testing purposes, we don't need to instantiate complex ViewModels
        // Just ensure the test framework knows the types exist

        // Add other commonly needed services
        services.AddLogging();
    }


    public static void SetupMockViewModelBehavior(GestoreAttivitaViewModel viewModel)
    {
        // Setup any specific behavior needed for tests
        // This can be extended based on test requirements
    }
}

