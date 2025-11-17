using IMAR_DialogoOperatore.Application.DTOs;
using IMAR_DialogoOperatore.Application.Interfaces.Clients;
using IMAR_DialogoOperatore.Domain.Entities.Imar_Produzione;
using IMAR_DialogoOperatore.Domain.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        public async Task<string> SendTaskAsana(TaskAsana taskAsana, string creatoreTask)
        {
            HttpClient client = new HttpClient();
            SetAutentication(client);
            var url = hostConnection + "pms/Asana/CreateTaskFromJson?createdBy=" + creatoreTask;
            string json = JsonConvert.SerializeObject(taskAsana);

            var buffer = System.Text.Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage response = client.PostAsync(url, byteContent).Result;
            return await response.Content.ReadAsStringAsync();
        }

        public void RegistraForzature(Forzatura forzatura)
        {
            HttpClient client = new HttpClient();
            SetAutentication(client);
            var url = hostConnection + "Forzatura/RegistraForzatura";
            string json = JsonConvert.SerializeObject(forzatura);

            var buffer = System.Text.Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage response = client.PostAsync(url, byteContent).Result;
            response.EnsureSuccessStatusCode();
        }

        public List<ODPSchedulazione> GetSchedulazioneAttuale(string odc)
        {
            HttpClient client = new HttpClient();
            SetAutentication(client);

            List<ODPSchedulazione> schedulazioneAttualeSQL;
            var urlSQL = hostConnection + "Schedulatore/GetSchedulazioneRigaOrdine/" + odc;
            HttpResponseMessage responseSQL = client.GetAsync(urlSQL).Result;
            responseSQL.EnsureSuccessStatusCode();
            string responseBodySQL = responseSQL.Content.ReadAsStringAsync().Result;
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
