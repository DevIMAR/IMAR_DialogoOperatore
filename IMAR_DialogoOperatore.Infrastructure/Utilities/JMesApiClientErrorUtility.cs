using IMAR_DialogoOperatore.Application.DTOs;
using IMAR_DialogoOperatore.Application.Interfaces.Utilities;
using System.Text.Json;

namespace IMAR_DialogoOperatore.Infrastructure.Utilities
{
    public class JMesApiClientErrorUtility : IJMesApiClientErrorUtility
    {
        private readonly ILoggingService _loggingService;

        public JMesApiClientErrorUtility(ILoggingService loggingService)
        {
            _loggingService = loggingService;
        }

        public async Task<(string? errore, JMesResultDto? dati)> GestioneEventualeErroreAsync(HttpResponseMessage result)
        {
            // Log della request inviata a JMes
            string requestBody = "[nessun body]";
            try
            {
                if (result.RequestMessage?.Content != null)
                    requestBody = await result.RequestMessage.Content.ReadAsStringAsync();
            }
            catch { /* content già disposto, ignora */ }

            var rawJson = await result.Content.ReadAsStringAsync();

            _loggingService.LogInfo($"JMES {result.RequestMessage?.Method} {result.RequestMessage?.RequestUri}" +
                $" | Request: {requestBody}" +
                $" | Response (HTTP {(int)result.StatusCode}): {(string.IsNullOrEmpty(rawJson) ? "[BODY VUOTO]" : rawJson)}");

            if (string.IsNullOrWhiteSpace(rawJson))
                return ($"L'API JMES ha restituito una risposta vuota (HTTP {(int)result.StatusCode})", null);

            JMesResultDto? jsonData = null;
            try
            {
                jsonData = JsonSerializer.Deserialize<JMesResultDto>(rawJson);
            }
            catch (JsonException ex)
            {
                _loggingService.LogError($"Errore deserializzazione risposta JMES: {rawJson}", ex);
                return ("Errore nella lettura della risposta JMES", null);
            }

            string? errore = GestioneEventualeErrore(jsonData);
            if (errore != null)
                _loggingService.LogError($"Errore JMES: {errore} | Risposta completa: {rawJson}");

            return (errore, jsonData);
        }

        public string? GestioneEventualeErrore(JMesResultDto? jsonData)
        {
            if (jsonData != null && jsonData.result.instanceRef.model.error)
                return ScritturaTestoErrore(jsonData);

            return null;
        }

        private string ScritturaTestoErrore(JMesResultDto? jsonData)
        {
            string errorMessage = "Operazione fallita a causa dei seguenti motivi:\n";
            foreach (object error in jsonData.result.instanceRef.model.errors)
                errorMessage += error.ToString() + "\n";

            return errorMessage;
        }
    }
}
