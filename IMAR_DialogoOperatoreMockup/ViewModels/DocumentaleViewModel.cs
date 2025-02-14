using IMAR_DialogoOperatore.Application.Interfaces.Services.External;

namespace IMAR_DialogoOperatore.ViewModels
{
    public class DocumentaleViewModel
	{
		private readonly IMorpheusApiService _morpheusApiService;

		public IMorpheusApiService MorpheusApiService => _morpheusApiService;

        public DocumentaleViewModel(
			IMorpheusApiService morpheusApiService)
		{
			_morpheusApiService = morpheusApiService;
		}
    }
}
