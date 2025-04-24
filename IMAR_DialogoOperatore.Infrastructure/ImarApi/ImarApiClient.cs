using IMAR_DialogoOperatore.Application.DTOs;
using IMAR_DialogoOperatore.Application.Interfaces.Clients;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace IMAR_DialogoOperatore.Infrastructure.ImarApi
{
	public class ImarApiClient : IImarApiClient
	{
		private const string hostConnection = "https://api.imarsrl.com/";
		private const string hostConnectionSQLTEST = "https://testapi.imarsrl.com/";
		private const string localhost = "https://localhost:44375/";

        public async Task<CostiArticoloDTO> GetCostiArticolo(string codiceArticolo)
        {
            HttpClient client = new HttpClient();
            SetAutentication(client);
            var url = hostConnection + "Articolo/GetCostiPerArticolo?articolo=" + codiceArticolo;
            HttpResponseMessage response = client.GetAsync(url).Result;
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<CostiArticoloDTO>(responseBody);
        }

        private void SetAutentication(HttpClient client)
        {
            var byteArray = Encoding.ASCII.GetBytes($"{"user_spc"}:{"BsaKqh8YzA%W5pLy"}");
            string encodeString = Convert.ToBase64String(byteArray);
            var clientAuthrizationHeader = new AuthenticationHeaderValue("Basic", encodeString);
            client.DefaultRequestHeaders.Authorization = clientAuthrizationHeader;
        }
    }
}
