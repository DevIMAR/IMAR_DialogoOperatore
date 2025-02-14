using IMAR_DialogoOperatore.Application.DTOs;
using IMAR_DialogoOperatore.Application.Interfaces.Utilities;
using System.Net.Http.Json;

namespace IMAR_DialogoOperatore.Infrastructure.Utilities
{
    public class JMesApiClientErrorUtility : IJMesApiClientErrorUtility
    {
        public string? GestioneEventualeErrore(HttpResponseMessage result)
        {
            var jsonData = result.Content.ReadFromJsonAsync<JMesResultDto>().GetAwaiter().GetResult(); ;

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
