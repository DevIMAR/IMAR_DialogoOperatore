using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using IMAR_DialogoOperatore.Application.Interfaces.Utilities;

namespace IMAR_DialogoOperatore.Infrastructure.Utilities
{
	public class HttpClientUtility : IHttpClientUtility
	{
		public async Task<HttpClient> BuildAuthenticatedClient(string urlLogin)
		{
			HttpClient client = new HttpClient();

			var resultToken = await client.GetStringAsync(urlLogin).ConfigureAwait(false);
			JObject getResult = JsonConvert.DeserializeObject(resultToken) as JObject;
			var mesToken = getResult.GetValue("result");

			client.DefaultRequestHeaders.Add("token", mesToken.ToString());

			return client;
		}
	}
}
