using IMAR_DialogoOperatore.Commands;
using IMAR_DialogoOperatore.Helpers;
using IMAR_DialogoOperatore.Interfaces.Helpers;
using IMAR_DialogoOperatore.Interfaces.Mappers;
using IMAR_DialogoOperatore.Interfaces.Observers;
using IMAR_DialogoOperatore.Managers;
using IMAR_DialogoOperatore.Mappers;
using IMAR_DialogoOperatore.Observers;
using IMAR_DialogoOperatore.Utilities;
using IMAR_DialogoOperatore.ViewModels;

namespace IMAR_DialogoOperatore
{
    public static class DependencyInjection
	{
		public static IServiceCollection AddDialogoOperatoreServices(this IServiceCollection services)
		{
			services.AddScoped<IAttivitaIndirettaObserver, AttivitaIndirettaObserver>();
			services.AddScoped<IAvanzamentoObserver, AvanzamentoObserver>();
			services.AddScoped<ICercaAttivitaObserver, CercaAttivitaObserver>();
			services.AddScoped<IDialogoOperatoreObserver, DialogoOperatoreObserver>();
			services.AddScoped<IPopupObserver, PopupObserver>();
			services.AddScoped<ISegnalazioneObserver, SegnalazioneObserver>();
			services.AddScoped<ITaskCompilerObserver, TaskCompilerObserver>();

			services.AddScoped<ICercaAttivitaHelper, CercaAttivitaHelper>();
			services.AddScoped<IConfermaOperazioneHelper, ConfermaOperazioneHelper>();
			services.AddScoped<IInterruzioneAttivitaHelper, InterruzioneAttivitaHelper>();
			services.AddScoped<IPopupConfermaHelper, PopupConfermaHelper>();
			services.AddScoped<ITaskCompilerHelper, AsanaTaskCompilerHelper>();

			services.AddScoped<IAttivitaMapper, AttivitaMapper>();
			services.AddScoped<IOperatoreMapper, OperatoreMapper>();
			services.AddScoped<ITimbraturaMapper, TimbraturaMapper>();

			services.AddScoped<ToastDisplayerUtility>();

			services.AddScoped<LogoutTimerManager>();

			services.AddScoped<AttivitaDetailsViewModel>();
			services.AddScoped<AttivitaGridViewModel>();
			services.AddScoped<AvanzamentoAttivitaViewModel>();
			services.AddScoped<CompilatoreTaskViewModel>();
			services.AddScoped<CronologiaAttivitaGridViewModel>();
			services.AddScoped<DialogoOperatoreViewModel>();
			services.AddScoped<DocumentaleViewModel>();
			services.AddScoped<FasiIndiretteGridViewModel>();
			services.AddScoped<FasiIndirettePopupViewModel>();
			services.AddScoped<FormSegnalazioneDifformitaViewModel>();
			services.AddScoped<GestoreAttivitaViewModel>();
			services.AddScoped<HeaderToolbarViewModel>();
			services.AddScoped<InfoBaseAttivitaViewModel>();
			services.AddScoped<InfoOperatoreViewModel>();
			services.AddScoped<InfoTaskOperatoreViewModel>();
			services.AddScoped<PopupDiConfermaViewModel>();
			services.AddScoped<PopupTimbratureViewModel>();
			services.AddScoped<PulsantieraGeneraleViewModel>();
			services.AddScoped<TaskPopupViewModel>();
			services.AddScoped<TimbratureGridViewModel>();

			services.AddScoped<AnnullaOperazioneCommand>();
			services.AddScoped<AvanzamentoCommand>();
			services.AddScoped<ConfermaCommand>();
			services.AddScoped<FineAttrezzaggioCommand>();
			services.AddScoped<FineLavoroCommand>();
			services.AddScoped<IngressoUscitaCommand>();
			services.AddScoped<InizioAttrezzaggioCommand>();
			services.AddScoped<InizioFinePausaCommand>();
			services.AddScoped<InizioLavoroCommand>();
			services.AddScoped<InviaTaskCommand>();
			services.AddScoped<MostraIndiretteCommand>();
			services.AddScoped<RispostaPopupDiConfermaCommand>();
			services.AddScoped<ShowEntrateUscitePauseCommand>();
			services.AddScoped<ShowTaskPopupCommand>();

			return services;
		}
	}
}
