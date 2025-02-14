using IMAR_DialogoOperatore.Application.Interfaces.Services.External;
using RestSharp;
using RestSharp.Authenticators;

namespace IMAR_DialogoOperatore.Services
{
    public class MorpheusApiService : IMorpheusApiService
	{
		private readonly IRestClient _restClient;
		private const string STRINGA_CONNESSIONE = @"http://morpheusapi.imarsrl.com";

		public MorpheusApiService()
		{
			var options = new RestClientOptions(STRINGA_CONNESSIONE)
			{
				Authenticator = new HttpBasicAuthenticator("user_morpheus", "jLKQ2AhnUzvfdJyE")
			};

			_restClient = new RestClient(options);
		}

		public string? GetDocumentaleDaArticolo(string articolo)
		{
			if (articolo.Equals(string.Empty))
				return null;

			return STRINGA_CONNESSIONE + $"/Gateway/ImarApi/GetDisegnoArticolo/?codiceArticolo={articolo}";
		}

		public string? GetDocumentaleDaOdp(string odp)
		{
			if (odp.Equals(string.Empty))
				return null;

			return STRINGA_CONNESSIONE + $"/Gateway/ImarApi/Documentale/Odp/?codiceOdp={odp}";
		}
	}
}
