using IMAR_DialogoOperatore.Application.Interfaces.Mappers;
using IMAR_DialogoOperatore.Commands;
using IMAR_DialogoOperatore.Helpers;
using IMAR_DialogoOperatore.Infrastructure.Mappers;
using IMAR_DialogoOperatore.Interfaces.Helpers;
using IMAR_DialogoOperatore.Interfaces.Mappers;
using IMAR_DialogoOperatore.Interfaces.Observers;
using IMAR_DialogoOperatore.Mappers;
using IMAR_DialogoOperatore.Observers;
using IMAR_DialogoOperatore.ViewModels;

namespace IMAR_DialogoOperatore
{
    public static class DependencyInjection
	{
		public static IServiceCollection AddDialogoOperatoreServices(this IServiceCollection services)
		{
			services.AddScoped<IAvanzamentoObserver, AvanzamentoObserver>();
			services.AddScoped<ICercaAttivitaObserver, CercaAttivitaObserver>();
			services.AddScoped<IDialogoOperatoreObserver, DialogoOperatoreObserver>();
			services.AddScoped<IPopupObserver, PopupObserver>();

			services.AddScoped<ICercaAttivitaHelper, CercaAttivitaHelper>();
			services.AddScoped<IConfermaOperazioneHelper, ConfermaOperazioneHelper>();
			services.AddScoped<IInterruzioneAttivitaHelper, InterruzioneAttivitaHelper>();
			services.AddScoped<IPopupConfermaHelper, PopupConfermaHelper>();

			services.AddScoped<IOperatoreMapper, OperatoreMapper>();
			services.AddScoped<IAttivitaMapper, AttivitaMapper>();
			services.AddScoped<IStatoAttivitaMapper, StatoAttivitaMapper>();

			services.AddScoped<DialogoOperatoreViewModel>();
			services.AddScoped<InfoOperatoreViewModel>();
			services.AddScoped<AttivitaGridViewModel>();
			services.AddScoped<GestoreAttivitaViewModel>();
			services.AddScoped<AttivitaDetailsViewModel>();
			services.AddScoped<AvanzamentoAttivitaViewModel>();
			services.AddScoped<PulsantieraGeneraleViewModel>();
			services.AddScoped<PopupDiConfermaViewModel>();
			services.AddScoped<DocumentaleViewModel>();

			services.AddScoped<AnnullaOperazioneCommand>();
			services.AddScoped<AvanzamentoCommand>();
			services.AddScoped<ConfermaCommand>();
			services.AddScoped<FineAttrezzaggioCommand>();
			services.AddScoped<FineLavoroCommand>();
			services.AddScoped<IngressoUscitaCommand>();
			services.AddScoped<InizioAttrezzaggioCommand>();
			services.AddScoped<InizioFinePausaCommand>();
			services.AddScoped<InizioLavoroCommand>();
			services.AddScoped<RispostaPopupDiConfermaCommand>();

			return services;
		}
	}
}
