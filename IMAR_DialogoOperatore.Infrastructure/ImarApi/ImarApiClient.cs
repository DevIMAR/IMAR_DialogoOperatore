using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using IMAR_DialogoOperatore.Application.DTOs;
using IMAR_DialogoOperatore.Application.Interfaces.Clients;
using IMAR_DialogoOperatore.Domain.DTO;
using IMAR_DialogoOperatore.Domain.Entities.Imar_Produzione;
using IMAR_DialogoOperatore.Domain.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<CostiArticoloDTO>(responseBody);
        }

        public async Task<string> SendTaskAsana(TaskAsana taskAsana, string creatoreTask)
        {
            HttpClient client = new HttpClient();
            SetAutentication(client);
            var url = hostConnection + "pms/Asana/CreateTaskFromJson?createdBy=" + creatoreTask;
            string json = JsonConvert.SerializeObject(taskAsana);

            var buffer = Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage response = await client.PostAsync(url, byteContent);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task RegistraForzature(Forzatura forzatura)
        {
            HttpClient client = new HttpClient();
            SetAutentication(client);
            var url = hostConnection + "Forzatura/RegistraForzatura";
            string json = JsonConvert.SerializeObject(forzatura);

            var buffer = Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage response = await client.PostAsync(url, byteContent);
            response.EnsureSuccessStatusCode();
		}

		public async Task RimuoviSchedulazioneAttuale(string chiamante, List<ODPSchedulazione> schedulazioneAttuale)
		{
			HttpClient client = new HttpClient();
			SetAutentication(client);
			var urlSQL = hostConnection + "Schedulatore/RimuoviSchedulazioneRigaOrdine?chiamante=" + chiamante;
			string json = JsonConvert.SerializeObject(schedulazioneAttuale);

			var buffer = Encoding.UTF8.GetBytes(json);
			var byteContent = new ByteArrayContent(buffer);
			byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			HttpResponseMessage responseSQL = await client.PostAsync(urlSQL, byteContent);
			string responseBodySQL = await responseSQL.Content.ReadAsStringAsync();
		}

		public async Task<ForzaturaDTO> GetPreviewForzatura(string odc, string giornoForza, decimal allocazione)
		{
			HttpClient client = new HttpClient();
			SetAutentication(client);
			var url = hostConnection + "Forzatura/ForzaRigaOrdine/" + odc + "/" + giornoForza + "/" + allocazione;
			HttpResponseMessage response = await client.GetAsync(url);
			response.EnsureSuccessStatusCode();
			string responseBody = await response.Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<ForzaturaDTO>(responseBody);
		}

		public async Task<string> InserisciNuovaSchedulazione(List<GiornoSchedulazione> forzatura, string riga, DateTime fineSchedulazione)
		{
			HttpClient client = new HttpClient();
			SetAutentication(client);

			var urlSQL = hostConnection + $"Schedulatore/InserisciSchedulazioneRigaOrdine?rigaOrdine={riga}&fineSchedulazione={fineSchedulazione.ToShortDateString()}";
			string json = JsonConvert.SerializeObject(forzatura);

			var buffer = Encoding.UTF8.GetBytes(json);
			var byteContent = new ByteArrayContent(buffer);
			byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			HttpResponseMessage responseSQL = await client.PostAsync(urlSQL, byteContent);
			string responseBodySQL = await responseSQL.Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<string>(responseBodySQL);
		}

		public async Task<List<ODPSchedulazione>> GetSchedulazioneAttuale(string odc)
        {
            HttpClient client = new HttpClient();
            SetAutentication(client);

            List<ODPSchedulazione> schedulazioneAttualeSQL;
            var urlSQL = hostConnection + "Schedulatore/GetSchedulazioneRigaOrdine/" + odc;
            HttpResponseMessage responseSQL = await client.GetAsync(urlSQL);
            responseSQL.EnsureSuccessStatusCode();
            string responseBodySQL = await responseSQL.Content.ReadAsStringAsync();
            JObject jsonObjSQL = JObject.Parse(responseBodySQL);
            JArray jsonRootArraySQL = (JArray)jsonObjSQL["result"];
            schedulazioneAttualeSQL = jsonRootArraySQL.ToObject<IList<ODPSchedulazione>>().ToList();

            return schedulazioneAttualeSQL;
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
