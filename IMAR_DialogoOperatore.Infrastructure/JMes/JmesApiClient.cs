using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Application.Interfaces.Clients;
using IMAR_DialogoOperatore.Application.Interfaces.Utilities;
using IMAR_DialogoOperatore.Domain.Models;
using Newtonsoft.Json.Linq;
using System.Text;

namespace IMAR_DialogoOperatore.Infrastructure.JMes
{
    public class JmesApiClient : IJmesApiClient
    {
        private const string SERVER = "http://i-s-023.imargroup.local:8080/synergy-ws/ws/";
        private const string JMES_LOGIN_PATH = "base/appLogin?app=imarConnect-jmes&key=";
        private const string DIAOPE_LOGIN_PATH = "base/appLogin?app=imarConnect-DiaOpe&key=";
        private const string JMES_LOGIN_TOKEN = "1hWP47cnCCmH9Ob5WxFttYl2FP4O5lvqPfWIHJm9dnfN4Jx2OJQcn296rRxVZ4FEuPqAYy6PJx09bKl1qHgEqN6LM";
        private const string DIAOPE_LOGIN_TOKEN = "ekmeJ216zWHSdHXuT2kxTqp6l3aWL2WUEKaODo57TzC5inv4I1FbQ5UNioMaOFg8b3f31WHl2FUWJ7AEUz5VhTfQDbljBFB3sadY";
        private const string WIZARD_WORK_PATH = "spec/sys/wzd/start";
        private const string QUERY_WORK_PATH = "spec/sys/qry/exec";
        private const string GET_QUERY_ID_PATH = "spec/sys/qry/byName";

        private HttpClient _jmesClient;
        private HttpClient _diaopeClient;
        private IJSonUtility _jsonUtility;
        private IHttpClientUtility _httpClientUtility;
        private readonly IJMesApiClientErrorUtility _jMesApiClientErrorUtility;

        public JmesApiClient(
            IJSonUtility jSonUtility,
            IHttpClientUtility httpClientUtility,
            IJMesApiClientErrorUtility jMesApiClientErrorUtility)
        {
            _jsonUtility = jSonUtility;
            _httpClientUtility = httpClientUtility;

            _jMesApiClientErrorUtility = jMesApiClientErrorUtility;

            Task jmesClientTask = Task.Run(async () => _jmesClient = await _httpClientUtility.BuildAuthenticatedClient(SERVER + JMES_LOGIN_PATH + JMES_LOGIN_TOKEN));
            Task diaopeClientTask = Task.Run(async () => _diaopeClient = await _httpClientUtility.BuildAuthenticatedClient(SERVER + DIAOPE_LOGIN_PATH + DIAOPE_LOGIN_TOKEN));

            Task.WaitAll(jmesClientTask, diaopeClientTask);
        }

        public IList<T>? ChiamaQueryGetJmes<T>()
        {
            int? queryId = GetQueryId(typeof(T).Name);
            if (queryId == null)
                return null;

            JObject json = GetQueryResults(queryId);

            return json["result"]?.ToObject<List<T>>();
        }

        public IList<T>? ChiamaQueryVirtualJmes<T>()
        {
            int? queryId = GetQueryId(typeof(T).Name);
            if (queryId == null)
                return null;

            JObject json = VirtualQueryResults(queryId);

            return json["result"]?.ToObject<List<T>>();
        }

        private int? GetQueryId(string queryName)
        {
            try
            {
                var urlStartWork = SERVER + GET_QUERY_ID_PATH + "/" + queryName + "?token=" + GetToken();

                var result = _diaopeClient.GetAsync(urlStartWork).GetAwaiter().GetResult();
                var jsonData = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var json = JObject.Parse(jsonData);

                return json["result"]?.ToObject<int>();
            }
            catch (Exception)
            {
                return null;
            }
        }

        private string? GetToken()
        {
            string urlGetToken = SERVER + DIAOPE_LOGIN_PATH + DIAOPE_LOGIN_TOKEN;

            var result = _diaopeClient.GetAsync(urlGetToken).GetAwaiter().GetResult();
            var jsonData = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var json = JObject.Parse(jsonData);

            return json["result"]?.ToObject<string>();
        }

        private JObject GetQueryResults(int? queryId)
        {
            var urlStartWork = SERVER + QUERY_WORK_PATH + "/" + queryId + "?token=" + GetToken();

            var emptyContent = new StringContent("{}", Encoding.UTF8, "application/json");

            var result = _diaopeClient.PostAsync(urlStartWork, emptyContent).GetAwaiter().GetResult();

            var jsonData = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var json = JObject.Parse(jsonData);
            return json;
        }

        private JObject VirtualQueryResults(int? queryId)
        {
            var urlStartWork = SERVER + QUERY_WORK_PATH + "/" + queryId + "?token=" + GetToken();

            var entity = new
            {
                entity = new
                {
                    filters = new object[] { },
                    sorts = new object[] { }
                }
            };

            var result = _diaopeClient.PostAsync(urlStartWork, _jsonUtility.BuildJsonContent(entity)).GetAwaiter().GetResult();

            var jsonData = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var json = JObject.Parse(jsonData);
            return json;
        }

        public string? RegistrazioneOperazioneSuDb(Func<HttpResponseMessage> operazione)
        {
            HttpResponseMessage result = operazione();

            string? errore = _jMesApiClientErrorUtility.GestioneEventualeErrore(result);
            if (errore != null)
                return errore;

            return null;
        }

        public HttpResponseMessage MesAdvanceDeclaration(Operatore operatore, Attivita attivita, int quantitaProdotta, int quantitaScartata)
        {
            string wizardPath = "?wzdCod=MesAdvanceDeclaration";

            var entity = new
            {
                entity = new
                {
                    paramsIO =
                    new
                    {
                        qck = true,
                        clkBdgCod = operatore.Badge,
                        notCod = attivita.Bolla,
                        tssStr = DateTime.Now.ToString("yyyyMMddHHmmss"),
                        producedQuantity = quantitaProdotta,
                        rejectedQuantity = quantitaScartata,
                        defDecAdv = attivita.SaldoAcconto == Costanti.ACCONTO,
                        clkMacUid = operatore.MacchinaAssegnata.CodiceJMes
                    }
                }
            };

            var urlStartWork = SERVER + WIZARD_WORK_PATH + wizardPath;
            var result = _jmesClient.PostAsync(urlStartWork, _jsonUtility.BuildJsonContent(entity)).GetAwaiter().GetResult();
            return result;
        }

        public HttpResponseMessage MesWorkStart(Operatore operatore, string bolla)
        {
            string wizardPath = "?wzdCod=MesWorkStart";

            var entity = new
            {
                entity = new
                {
                    paramsIO = new
                    {
                        qck = true,
                        clkBdgCod = operatore.Badge,
                        notCod = bolla,
                        clkMacUid = operatore.MacchinaAssegnata.CodiceJMes
                    }
                }
            };

            var urlStartWork = SERVER + WIZARD_WORK_PATH + wizardPath;
            var result = _jmesClient.PostAsync(urlStartWork, _jsonUtility.BuildJsonContent(entity)).GetAwaiter().GetResult();
            return result;
        }

        public HttpResponseMessage MesWorkStartNotPln(Operatore operatore, string bolla, string codiceFase)
        {
            string wizardPath = "?wzdCod=MesWorkStart";

            var entity = new
            {
                entity = new
                {
                    paramsIO = new
                    {
                        qck = true,
                        clkBdgCod = operatore.Badge,
                        notPlnCod = codiceFase,
                        notPlnNotCod = bolla,
                        clkMacUid = operatore.MacchinaAssegnata.CodiceJMes
                    }
                }
            };

            var urlStartWork = SERVER + WIZARD_WORK_PATH + wizardPath;
            var result = _jmesClient.PostAsync(urlStartWork, _jsonUtility.BuildJsonContent(entity)).GetAwaiter().GetResult();
            return result;
        }

        public HttpResponseMessage MesWorkStartIndiretta(string badge, string codiceAttivitaIndiretta)
        {
            string wizardPath = "?wzdCod=MesWorkStartIndirect";

            var entity = new
            {
                entity = new
                {
                    paramsIO = new
                    {
                        qck = true,
                        clkBdgCod = badge,
                        indCod = codiceAttivitaIndiretta
                    }
                }
            };

            var urlStartWork = SERVER + WIZARD_WORK_PATH + wizardPath;
            var result = _jmesClient.PostAsync(urlStartWork, _jsonUtility.BuildJsonContent(entity)).GetAwaiter().GetResult();
            return result;
        }

        public HttpResponseMessage MesWorkEnd(string badge, Attivita attivita, int quantitaProdotta, int quantitaScartata)
        {
            string wizardPath = "?wzdCod=MesWorkEnd";

            var entity = new
            {
                entity = new
                {
                    paramsIO = new
                    {
                        qck = true,
                        clkBdgCod = badge,
                        clkDiaOpeUid = attivita.CodiceJMes,
                        producedQuantity = quantitaProdotta,
                        rejectedQuantity = quantitaScartata,
                        defDecAdv = attivita.SaldoAcconto == Costanti.ACCONTO
                    }
                }
            };

            var urlStartWork = SERVER + WIZARD_WORK_PATH + wizardPath;
            var result = _jmesClient.PostAsync(urlStartWork, _jsonUtility.BuildJsonContent(entity)).GetAwaiter().GetResult();
            return result;
        }

        public HttpResponseMessage MesWorkSuspension(string badge, Attivita attivita, int quantitaProdotta, int quantitaScartata)
        {
            string wizardPath = "?wzdCod=MesWorkSuspension";

            var entity = new
            {
                entity = new
                {
                    paramsIO = new
                    {
                        qck = true,
                        clkBdgCod = badge,
                        clkDiaOpeUid = attivita.CodiceJMes,
                        sspUid = 1,
                        producedQuantity = quantitaProdotta,
                        rejectedQuantity = quantitaScartata,
                        defDecAdv = attivita.SaldoAcconto == Costanti.ACCONTO
                    }
                }
            };

            var urlStartWork = SERVER + WIZARD_WORK_PATH + wizardPath;
            var result = _jmesClient.PostAsync(urlStartWork, _jsonUtility.BuildJsonContent(entity)).GetAwaiter().GetResult();
            return result;
        }

        public HttpResponseMessage MesWorkResume(string badge, Attivita attivita)
        {
            string wizardPath = "?wzdCod=MesWorkResume";

            var entity = new
            {
                entity = new
                {
                    paramsIO = new
                    {
                        qck = true,
                        clkBdgCod = badge,
                        clkDiaOpeUid = attivita.CodiceJMes,
                    }
                }
            };

            var urlStartWork = SERVER + WIZARD_WORK_PATH + wizardPath;
            var result = _jmesClient.PostAsync(urlStartWork, _jsonUtility.BuildJsonContent(entity)).GetAwaiter().GetResult();
            return result;
        }

        public HttpResponseMessage MesSuspensionStart(string badge, int codiceJmesMacchina)
        {
            string wizardPath = "?wzdCod=MesSuspensionStart";

            var entity = new
            {
                entity = new
                {
                    paramsIO = new
                    {
                        qck = true,
                        clkBdgCod = badge,
                        clkMacUid = codiceJmesMacchina,
                        sspUid = 1
                    }
                }
            };

            var urlStartWork = SERVER + WIZARD_WORK_PATH + wizardPath;
            var result = _jmesClient.PostAsync(urlStartWork, _jsonUtility.BuildJsonContent(entity)).GetAwaiter().GetResult();
            return result;
        }

        public HttpResponseMessage MesSuspensionEnd(string badge, int codiceJmesMacchina)
        {
            string wizardPath = "?wzdCod=MesSuspensionEnd";

            var entity = new
            {
                entity = new
                {
                    paramsIO = new
                    {
                        qck = true,
                        clkMacUid = codiceJmesMacchina,
                        clkBdgCod = badge
                    }
                }
            };

            var urlStartWork = SERVER + WIZARD_WORK_PATH + wizardPath;
            var result = _jmesClient.PostAsync(urlStartWork, _jsonUtility.BuildJsonContent(entity)).GetAwaiter().GetResult();
            return result;
        }

        public HttpResponseMessage MesEquipStart(Operatore operatore, string bolla)
        {
            string wizardPath = "?wzdCod=MesEquipStart";

            var entity = new
            {
                entity = new
                {
                    paramsIO = new
                    {
                        qck = true,
                        clkBdgCod = operatore.Badge,
                        notCod = bolla,
                        clkMacUid = operatore.MacchinaAssegnata.CodiceJMes
                    }
                }
            };

            var urlStartWork = SERVER + WIZARD_WORK_PATH + wizardPath;
            var result = _jmesClient.PostAsync(urlStartWork, _jsonUtility.BuildJsonContent(entity)).GetAwaiter().GetResult();
            return result;
        }

        public HttpResponseMessage MesEquipStartNotPln(Operatore operatore, string bolla, string codiceFase)
        {
            string wizardPath = "?wzdCod=MesEquipStart";

            var entity = new
            {
                entity = new
                {
                    paramsIO = new
                    {
                        qck = true,
                        clkBdgCod = operatore.Badge,
                        notPlnCod = codiceFase,
                        notPlnNotCod = bolla,
                        clkMacUid = operatore.MacchinaAssegnata.CodiceJMes
                    }
                }
            };

            var urlStartWork = SERVER + WIZARD_WORK_PATH + wizardPath;
            var result = _jmesClient.PostAsync(urlStartWork, _jsonUtility.BuildJsonContent(entity)).GetAwaiter().GetResult();
            return result;
        }

        public HttpResponseMessage MesEquipEnd(string badge, double? idJmesAttrezzaggio)
        {
            string wizardPath = "?wzdCod=MesEquipEnd";

            var entity = new
            {
                entity = new
                {
                    paramsIO = new
                    {
                        qck = true,
                        clkBdgCod = badge,
                        clkDiaOpeUid = idJmesAttrezzaggio
                    }
                }
            };

            var urlStartWork = SERVER + WIZARD_WORK_PATH + wizardPath;
            var result = _jmesClient.PostAsync(urlStartWork, _jsonUtility.BuildJsonContent(entity)).GetAwaiter().GetResult();
            return result;
        }

        public HttpResponseMessage MesEquipSuspension(string badge, double? idJmesAttrezzaggio)
        {
            string wizardPath = "?wzdCod=MesEquipSuspension";

            var entity = new
            {
                entity = new
                {
                    paramsIO = new
                    {
                        qck = true,
                        clkBdgCod = badge,
                        clkDiaOpeUid = idJmesAttrezzaggio,
                        sspUid = 1,
                    }
                }
            };

            var urlStartWork = SERVER + WIZARD_WORK_PATH + wizardPath;
            var result = _jmesClient.PostAsync(urlStartWork, _jsonUtility.BuildJsonContent(entity)).GetAwaiter().GetResult();
            return result;
        }

        public HttpResponseMessage MesEquipResume(string badge, Attivita attivita)
        {
            string wizardPath = "?wzdCod=MesEquipResume";

            var entity = new
            {
                entity = new
                {
                    paramsIO = new
                    {
                        qck = true,
                        clkBdgCod = badge,
                        clkDiaOpeUid = attivita.CodiceJMes,
                    }
                }
            };

            var urlStartWork = SERVER + WIZARD_WORK_PATH + wizardPath;
            var result = _jmesClient.PostAsync(urlStartWork, _jsonUtility.BuildJsonContent(entity)).GetAwaiter().GetResult();
            return result;
        }

        public HttpResponseMessage MesBreakStart(string badge)
        {
            string wizardPath = "?wzdCod=MesBreakStart";

            var entity = new
            {
                entity = new
                {
                    paramsIO = new
                    {
                        qck = true,
                        clkBdgCod = badge,
                        brkUid = 1
                    }
                }
            };

            var urlStartWork = SERVER + WIZARD_WORK_PATH + wizardPath;
            var result = _jmesClient.PostAsync(urlStartWork, _jsonUtility.BuildJsonContent(entity)).GetAwaiter().GetResult();
            return result;
        }

        public HttpResponseMessage MesBreakEnd(string badge)
        {
            string wizardPath = "?wzdCod=MesBreakEnd";

            var entity = new
            {
                entity = new
                {
                    paramsIO = new
                    {
                        qck = true,
                        clkBdgCod = badge,
                    }
                }
            };

            var urlStartWork = SERVER + WIZARD_WORK_PATH + wizardPath;
            var result = _jmesClient.PostAsync(urlStartWork, _jsonUtility.BuildJsonContent(entity)).GetAwaiter().GetResult();
            return result;
        }

        public HttpResponseMessage MesAutoClock(string badge, bool isIngresso)
        {
            string wizardPath = "?wzdCod=MesAutoClock";

            var entity = new
            {
                entity = new
                {
                    paramsIO = new
                    {
                        bdgCod = badge,
                        clkSid = isIngresso,
                        noChkBrk = true
                    }
                }
            };

            var urlStartWork = SERVER + WIZARD_WORK_PATH + wizardPath;
            var result = _jmesClient.PostAsync(urlStartWork, _jsonUtility.BuildJsonContent(entity)).GetAwaiter().GetResult();
            return result;
        }
    }
}
