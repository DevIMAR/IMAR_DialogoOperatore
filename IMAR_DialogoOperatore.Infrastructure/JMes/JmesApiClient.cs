using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Application.Interfaces.Clients;
using IMAR_DialogoOperatore.Application.Interfaces.Utilities;
using IMAR_DialogoOperatore.Domain.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text;

namespace IMAR_DialogoOperatore.Infrastructure.JMes
{
    public class JmesApiClient : IJmesApiClient
    {
        private const string JMES_LOGIN_PATH = "base/appLogin?app=imarConnect-jmes&key=";
        private const string DIAOPE_LOGIN_PATH = "base/appLogin?app=imarConnect-DiaOpe&key=";
        private const string WIZARD_WORK_PATH = "spec/sys/wzd/start";
        private const string QUERY_WORK_PATH = "spec/sys/qry/exec";
        private const string GET_QUERY_ID_PATH = "spec/sys/qry/byName";

        private readonly string _server;
        private readonly string _jmesLoginToken;
        private readonly string _diaopeLoginToken;

        private HttpClient _jmesClient;
        private HttpClient _diaopeClient;
        private IJSonUtility _jsonUtility;
        private IHttpClientUtility _httpClientUtility;
        private readonly IJMesApiClientErrorUtility _jMesApiClientErrorUtility;
        private readonly ILoggingService _loggingService;

        private readonly SemaphoreSlim _initSemaphore = new(1, 1);
        private bool _initialized;

        public JmesApiClient(
            IJSonUtility jSonUtility,
            IHttpClientUtility httpClientUtility,
            IJMesApiClientErrorUtility jMesApiClientErrorUtility,
            ILoggingService loggingService,
            IConfiguration configuration)
        {
            _jsonUtility = jSonUtility;
            _httpClientUtility = httpClientUtility;
            _jMesApiClientErrorUtility = jMesApiClientErrorUtility;
            _loggingService = loggingService;

            _server = configuration["JmesApi:Server"]!;
            _jmesLoginToken = configuration["JmesApi:JmesLoginToken"]!;
            _diaopeLoginToken = configuration["JmesApi:DiaopeLoginToken"]!;
        }

        private async Task EnsureInitializedAsync()
        {
            if (_initialized) return;

            await _initSemaphore.WaitAsync();
            try
            {
                if (_initialized) return;

                var sw = Stopwatch.StartNew();

                var jmesTask = _httpClientUtility.BuildAuthenticatedClient(_server + JMES_LOGIN_PATH + _jmesLoginToken);
                var diaopeTask = _httpClientUtility.BuildAuthenticatedClient(_server + DIAOPE_LOGIN_PATH + _diaopeLoginToken);

                await Task.WhenAll(jmesTask, diaopeTask);

                _jmesClient = jmesTask.Result;
                _diaopeClient = diaopeTask.Result;
                _initialized = true;

                _loggingService.LogInfo($"JmesApiClient.EnsureInitializedAsync completato in {sw.ElapsedMilliseconds}ms");
            }
            finally
            {
                _initSemaphore.Release();
            }
        }

        public async Task<IList<T>?> ChiamaQueryGetJmesAsync<T>()
        {
            var sw = Stopwatch.StartNew();

            await EnsureInitializedAsync();

            int? queryId = await GetQueryIdAsync(typeof(T).Name);
            if (queryId == null)
                return null;

            JObject json = await GetQueryResultsAsync(queryId);

            _loggingService.LogInfo($"JmesApiClient.ChiamaQueryGetJmesAsync<{typeof(T).Name}> completato in {sw.ElapsedMilliseconds}ms");
            return json["result"]?.ToObject<List<T>>();
        }

        public async Task<IList<T>?> ChiamaQueryVirtualJmesAsync<T>()
        {
            var sw = Stopwatch.StartNew();

            await EnsureInitializedAsync();

            int? queryId = await GetQueryIdAsync(typeof(T).Name);
            if (queryId == null)
                return null;

            JObject json = await VirtualQueryResultsAsync(queryId);

            _loggingService.LogInfo($"JmesApiClient.ChiamaQueryVirtualJmesAsync<{typeof(T).Name}> completato in {sw.ElapsedMilliseconds}ms");
            return json["result"]?.ToObject<List<T>>();
        }

        private async Task<int?> GetQueryIdAsync(string queryName)
        {
            try
            {
                await EnsureInitializedAsync();

                var urlStartWork = _server + GET_QUERY_ID_PATH + "/" + queryName + "?token=" + await GetTokenAsync();

                var result = await _diaopeClient.GetAsync(urlStartWork);
                var jsonData = await result.Content.ReadAsStringAsync();
                var json = JObject.Parse(jsonData);

                return json["result"]?.ToObject<int>();
            }
            catch (Exception ex)
            {
                _loggingService.LogError($"Errore nel recupero QueryId per query '{queryName}'", ex);
                return null;
            }
        }

        private async Task<string?> GetTokenAsync()
        {
            await EnsureInitializedAsync();

            string urlGetToken = _server + DIAOPE_LOGIN_PATH + _diaopeLoginToken;

            var result = await _diaopeClient.GetAsync(urlGetToken);
            var jsonData = await result.Content.ReadAsStringAsync();
            var json = JObject.Parse(jsonData);

            return json["result"]?.ToObject<string>();
        }

        private async Task<JObject> GetQueryResultsAsync(int? queryId)
        {
            await EnsureInitializedAsync();

            var urlStartWork = _server + QUERY_WORK_PATH + "/" + queryId + "?token=" + await GetTokenAsync();

            var emptyContent = new StringContent("{}", Encoding.UTF8, "application/json");

            var result = await _diaopeClient.PostAsync(urlStartWork, emptyContent);

            var jsonData = await result.Content.ReadAsStringAsync();
            var json = JObject.Parse(jsonData);
            return json;
        }

        private async Task<JObject> VirtualQueryResultsAsync(int? queryId)
        {
            await EnsureInitializedAsync();

            var urlStartWork = _server + QUERY_WORK_PATH + "/" + queryId + "?token=" + await GetTokenAsync();

            var entity = new
            {
                entity = new
                {
                    filters = new object[] { },
                    sorts = new object[] { }
                }
            };

            var result = await _diaopeClient.PostAsync(urlStartWork, _jsonUtility.BuildJsonContent(entity));

            var jsonData = await result.Content.ReadAsStringAsync();
            var json = JObject.Parse(jsonData);
            return json;
        }

        public async Task<string?> RegistrazioneOperazioneSuDbAsync(Func<Task<HttpResponseMessage>> operazione)
        {
            var sw = Stopwatch.StartNew();

            HttpResponseMessage result = await operazione();

            var (errore, _) = await _jMesApiClientErrorUtility.GestioneEventualeErroreAsync(result);
            if (errore != null)
            {
                _loggingService.LogInfo($"JmesApiClient.RegistrazioneOperazioneSuDbAsync completato in {sw.ElapsedMilliseconds}ms (con errore)");
                return errore;
            }

            _loggingService.LogInfo($"JmesApiClient.RegistrazioneOperazioneSuDbAsync completato in {sw.ElapsedMilliseconds}ms");
            return null;
        }

        public async Task<HttpResponseMessage> MesAdvanceDeclarationAsync(Operatore operatore, Attivita attivita, int quantitaProdotta, int quantitaScartata)
        {
            var sw = Stopwatch.StartNew();
            await EnsureInitializedAsync();

            string wizardPath = "?wzdCod=MesAdvanceDeclaration";

            var macchina = operatore.MacchineAssegnate.FirstOrDefault();
            if (macchina == null)
            {
                _loggingService.LogError($"MesAdvanceDeclarationAsync: operatore {operatore.Badge} non ha macchine assegnate, impossibile avanzare bolla {attivita.Bolla}");
                return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            }

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
                        clkMacUid = macchina.CodiceJMes
                    }
                }
            };

            var urlStartWork = _server + WIZARD_WORK_PATH + wizardPath;
            var result = await _jmesClient.PostAsync(urlStartWork, _jsonUtility.BuildJsonContent(entity));

            _loggingService.LogInfo($"JmesApiClient.MesAdvanceDeclarationAsync completato in {sw.ElapsedMilliseconds}ms");
            return result;
        }

        public async Task<HttpResponseMessage> MesWorkStartAsync(Operatore operatore, Attivita attivita)
        {
            var sw = Stopwatch.StartNew();
            await EnsureInitializedAsync();

            string wizardPath = "?wzdCod=MesWorkStart";

            var entity = new
            {
                entity = new
                {
                    paramsIO = new
                    {
                        qck = true,
                        clkBdgCod = operatore.Badge,
                        notCod = attivita.Bolla,
                        clkMacUid = attivita.MacchinaFittizia.CodiceJMes
                    }
                }
            };

            var urlStartWork = _server + WIZARD_WORK_PATH + wizardPath;
            var result = await _jmesClient.PostAsync(urlStartWork, _jsonUtility.BuildJsonContent(entity));

            _loggingService.LogInfo($"JmesApiClient.MesWorkStartAsync completato in {sw.ElapsedMilliseconds}ms");
            return result;
        }

        public async Task<HttpResponseMessage> MesWorkStartNotPlnAsync(Operatore operatore, Attivita attivita, string codiceFase)
        {
            var sw = Stopwatch.StartNew();
            await EnsureInitializedAsync();

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
                        notPlnNotCod = attivita.Bolla,
                        clkMacUid = attivita.MacchinaReale?.CodiceJMes ?? operatore.MacchineAssegnate.FirstOrDefault()?.CodiceJMes
					}
                }
            };

            var urlStartWork = _server + WIZARD_WORK_PATH + wizardPath;
            var result = await _jmesClient.PostAsync(urlStartWork, _jsonUtility.BuildJsonContent(entity));

            _loggingService.LogInfo($"JmesApiClient.MesWorkStartNotPlnAsync completato in {sw.ElapsedMilliseconds}ms");
            return result;
        }

        public async Task<HttpResponseMessage> MesWorkStartIndirettaAsync(string badge, string codiceAttivitaIndiretta)
        {
            var sw = Stopwatch.StartNew();
            await EnsureInitializedAsync();

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

            var urlStartWork = _server + WIZARD_WORK_PATH + wizardPath;
            var result = await _jmesClient.PostAsync(urlStartWork, _jsonUtility.BuildJsonContent(entity));

            _loggingService.LogInfo($"JmesApiClient.MesWorkStartIndirettaAsync completato in {sw.ElapsedMilliseconds}ms");
            return result;
        }

        public async Task<HttpResponseMessage> MesWorkEndAsync(string badge, Attivita attivita, int quantitaProdotta, int quantitaScartata)
        {
            var sw = Stopwatch.StartNew();
            await EnsureInitializedAsync();

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

            var urlStartWork = _server + WIZARD_WORK_PATH + wizardPath;
            var result = await _jmesClient.PostAsync(urlStartWork, _jsonUtility.BuildJsonContent(entity));

            _loggingService.LogInfo($"JmesApiClient.MesWorkEndAsync completato in {sw.ElapsedMilliseconds}ms");
            return result;
        }

        public async Task<HttpResponseMessage> MesWorkSuspensionAsync(string badge, Attivita attivita, int quantitaProdotta, int quantitaScartata)
        {
            var sw = Stopwatch.StartNew();
            await EnsureInitializedAsync();

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

            var urlStartWork = _server + WIZARD_WORK_PATH + wizardPath;
            var result = await _jmesClient.PostAsync(urlStartWork, _jsonUtility.BuildJsonContent(entity));

            _loggingService.LogInfo($"JmesApiClient.MesWorkSuspensionAsync completato in {sw.ElapsedMilliseconds}ms");
            return result;
        }

        public async Task<HttpResponseMessage> MesWorkResumeAsync(string badge, Attivita attivita)
        {
            var sw = Stopwatch.StartNew();
            await EnsureInitializedAsync();

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

            var urlStartWork = _server + WIZARD_WORK_PATH + wizardPath;
            var result = await _jmesClient.PostAsync(urlStartWork, _jsonUtility.BuildJsonContent(entity));

            _loggingService.LogInfo($"JmesApiClient.MesWorkResumeAsync completato in {sw.ElapsedMilliseconds}ms");
            return result;
        }

        public async Task<HttpResponseMessage> MesSuspensionStartAsync(string badge, int codiceJmesMacchina)
        {
            var sw = Stopwatch.StartNew();
            await EnsureInitializedAsync();

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

            var urlStartWork = _server + WIZARD_WORK_PATH + wizardPath;
            var result = await _jmesClient.PostAsync(urlStartWork, _jsonUtility.BuildJsonContent(entity));

            _loggingService.LogInfo($"JmesApiClient.MesSuspensionStartAsync completato in {sw.ElapsedMilliseconds}ms");
            return result;
        }

        public async Task<HttpResponseMessage> MesSuspensionEndAsync(string badge, int codiceJmesMacchina)
        {
            var sw = Stopwatch.StartNew();
            await EnsureInitializedAsync();

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

            var urlStartWork = _server + WIZARD_WORK_PATH + wizardPath;
            var result = await _jmesClient.PostAsync(urlStartWork, _jsonUtility.BuildJsonContent(entity));

            _loggingService.LogInfo($"JmesApiClient.MesSuspensionEndAsync completato in {sw.ElapsedMilliseconds}ms");
            return result;
        }

        public async Task<HttpResponseMessage> MesEquipStartAsync(Operatore operatore, string bolla, Macchina? macchina = null)
        {
            var sw = Stopwatch.StartNew();
            await EnsureInitializedAsync();

            macchina = macchina ?? operatore.MacchineAssegnate.FirstOrDefault();
            if (macchina == null)
            {
                _loggingService.LogError($"MesEquipStartAsync: operatore {operatore.Badge} non ha macchine assegnate per bolla {bolla}");
                return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            }

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
                        clkMacUid = macchina.CodiceJMes
                    }
                }
            };

            var urlStartWork = _server + WIZARD_WORK_PATH + wizardPath;
            var result = await _jmesClient.PostAsync(urlStartWork, _jsonUtility.BuildJsonContent(entity));

            _loggingService.LogInfo($"JmesApiClient.MesEquipStartAsync completato in {sw.ElapsedMilliseconds}ms");
            return result;
        }

        public async Task<HttpResponseMessage> MesEquipStartNotPlnAsync(Operatore operatore, string bolla, string codiceFase)
        {
            var sw = Stopwatch.StartNew();
            await EnsureInitializedAsync();

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
                        clkMacUid = operatore.MacchineAssegnate.FirstOrDefault()?.CodiceJMes
                    }
                }
            };

            var urlStartWork = _server + WIZARD_WORK_PATH + wizardPath;
            var result = await _jmesClient.PostAsync(urlStartWork, _jsonUtility.BuildJsonContent(entity));

            _loggingService.LogInfo($"JmesApiClient.MesEquipStartNotPlnAsync completato in {sw.ElapsedMilliseconds}ms");
            return result;
        }

        public async Task<HttpResponseMessage> MesEquipEndAsync(string badge, double? idJmesAttrezzaggio)
        {
            var sw = Stopwatch.StartNew();
            await EnsureInitializedAsync();

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

            var urlStartWork = _server + WIZARD_WORK_PATH + wizardPath;
            var result = await _jmesClient.PostAsync(urlStartWork, _jsonUtility.BuildJsonContent(entity));

            _loggingService.LogInfo($"JmesApiClient.MesEquipEndAsync completato in {sw.ElapsedMilliseconds}ms");
            return result;
        }

        public async Task<HttpResponseMessage> MesEquipRemoveAsync(string badge, double? idJmesAttrezzaggio)
        {
            var sw = Stopwatch.StartNew();
            await EnsureInitializedAsync();

            string wizardPath = "?wzdCod=MesEquipRemove";

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

            var urlStartWork = _server + WIZARD_WORK_PATH + wizardPath;
            var result = await _jmesClient.PostAsync(urlStartWork, _jsonUtility.BuildJsonContent(entity));

            _loggingService.LogInfo($"JmesApiClient.MesEquipRemoveAsync completato in {sw.ElapsedMilliseconds}ms");
            return result;
        }

        public async Task<HttpResponseMessage> MesEquipSuspensionAsync(string badge, double? idJmesAttrezzaggio)
        {
            var sw = Stopwatch.StartNew();
            await EnsureInitializedAsync();

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

            var urlStartWork = _server + WIZARD_WORK_PATH + wizardPath;
            var result = await _jmesClient.PostAsync(urlStartWork, _jsonUtility.BuildJsonContent(entity));

            _loggingService.LogInfo($"JmesApiClient.MesEquipSuspensionAsync completato in {sw.ElapsedMilliseconds}ms");
            return result;
        }

        public async Task<HttpResponseMessage> MesEquipResumeAsync(string badge, Attivita attivita)
        {
            var sw = Stopwatch.StartNew();
            await EnsureInitializedAsync();

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

            var urlStartWork = _server + WIZARD_WORK_PATH + wizardPath;
            var result = await _jmesClient.PostAsync(urlStartWork, _jsonUtility.BuildJsonContent(entity));

            _loggingService.LogInfo($"JmesApiClient.MesEquipResumeAsync completato in {sw.ElapsedMilliseconds}ms");
            return result;
        }

        public async Task<HttpResponseMessage> MesBreakStartAsync(string badge)
        {
            var sw = Stopwatch.StartNew();
            await EnsureInitializedAsync();

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

            var urlStartWork = _server + WIZARD_WORK_PATH + wizardPath;
            var result = await _jmesClient.PostAsync(urlStartWork, _jsonUtility.BuildJsonContent(entity));

            _loggingService.LogInfo($"JmesApiClient.MesBreakStartAsync completato in {sw.ElapsedMilliseconds}ms");
            return result;
        }

        public async Task<HttpResponseMessage> MesBreakEndAsync(string badge)
        {
            var sw = Stopwatch.StartNew();
            await EnsureInitializedAsync();

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

            var urlStartWork = _server + WIZARD_WORK_PATH + wizardPath;
            var result = await _jmesClient.PostAsync(urlStartWork, _jsonUtility.BuildJsonContent(entity));

            _loggingService.LogInfo($"JmesApiClient.MesBreakEndAsync completato in {sw.ElapsedMilliseconds}ms");
            return result;
        }

        public async Task<HttpResponseMessage> MesAutoClockAsync(string badge, bool isIngresso)
        {
            var sw = Stopwatch.StartNew();
            await EnsureInitializedAsync();

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

            var urlStartWork = _server + WIZARD_WORK_PATH + wizardPath;
            var result = await _jmesClient.PostAsync(urlStartWork, _jsonUtility.BuildJsonContent(entity));

            _loggingService.LogInfo($"JmesApiClient.MesAutoClockAsync completato in {sw.ElapsedMilliseconds}ms");
            return result;
        }
    }
}
