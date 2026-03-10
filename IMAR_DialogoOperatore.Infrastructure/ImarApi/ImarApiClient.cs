using System.Net.Http.Headers;
using System.Text;
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
		private readonly IHttpClientFactory _httpClientFactory;

		public ImarApiClient(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}

        public async Task<CostiArticoloDTO> GetCostiArticolo(string codiceArticolo)
        {
            var client = _httpClientFactory.CreateClient("ImarApi");
            var url = "Articolo/GetCostiPerArticolo?articolo=" + codiceArticolo;
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<CostiArticoloDTO>(responseBody);
        }

        public async Task<string> SendTaskAsana(TaskAsana taskAsana, string creatoreTask)
        {
            var client = _httpClientFactory.CreateClient("ImarApi");
            var url = "pms/Asana/CreateTaskFromJson?createdBy=" + creatoreTask;
            string json = JsonConvert.SerializeObject(taskAsana);

            var buffer = Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage response = await client.PostAsync(url, byteContent);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task RegistraForzature(Forzatura forzatura)
        {
            var client = _httpClientFactory.CreateClient("ImarApi");
            var url = "Forzatura/RegistraForzatura";
            string json = JsonConvert.SerializeObject(forzatura);

            var buffer = Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage response = await client.PostAsync(url, byteContent);
            response.EnsureSuccessStatusCode();
		}

		public async Task RimuoviSchedulazioneAttuale(string chiamante, List<ODPSchedulazione> schedulazioneAttuale)
		{
			var client = _httpClientFactory.CreateClient("ImarApi");
			var url = "Schedulatore/RimuoviSchedulazioneRigaOrdine?chiamante=" + chiamante;
			string json = JsonConvert.SerializeObject(schedulazioneAttuale);

			var buffer = Encoding.UTF8.GetBytes(json);
			var byteContent = new ByteArrayContent(buffer);
			byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			await client.PostAsync(url, byteContent);
		}

		public async Task<ForzaturaDTO> GetPreviewForzatura(string odc, string giornoForza, decimal allocazione)
		{
			var client = _httpClientFactory.CreateClient("ImarApi");
			var url = "Forzatura/ForzaRigaOrdine/" + odc + "/" + giornoForza + "/" + allocazione;
			HttpResponseMessage response = await client.GetAsync(url);
			response.EnsureSuccessStatusCode();
			string responseBody = await response.Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<ForzaturaDTO>(responseBody);
		}

		public async Task<string> InserisciNuovaSchedulazione(List<GiornoSchedulazione> forzatura, string riga, DateTime fineSchedulazione)
		{
			var client = _httpClientFactory.CreateClient("ImarApi");
			var url = $"Schedulatore/InserisciSchedulazioneRigaOrdine?rigaOrdine={riga}&fineSchedulazione={fineSchedulazione.ToShortDateString()}";
			string json = JsonConvert.SerializeObject(forzatura);

			var buffer = Encoding.UTF8.GetBytes(json);
			var byteContent = new ByteArrayContent(buffer);
			byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			HttpResponseMessage response = await client.PostAsync(url, byteContent);
			string responseBody = await response.Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<string>(responseBody);
		}

		public async Task<List<ODPSchedulazione>> GetSchedulazioneAttuale(string odc)
        {
            var client = _httpClientFactory.CreateClient("ImarApi");
            var url = "Schedulatore/GetSchedulazioneRigaOrdine/" + odc;
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            JObject jsonObj = JObject.Parse(responseBody);
            JArray jsonRootArray = (JArray)jsonObj["result"];
            return jsonRootArray.ToObject<IList<ODPSchedulazione>>().ToList();
        }
    }
}
