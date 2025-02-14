using IMAR_DialogoOperatore.Application.Interfaces.Utilities;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace IMAR_DialogoOperatore.Infrastructure.Utilities
{
	public class JSonUtility : IJSonUtility
	{
		public ByteArrayContent BuildJsonContent(object json)
		{
			var content = JsonConvert.SerializeObject(json);
			var buffer = System.Text.Encoding.UTF8.GetBytes(content);

			var byteContent = new ByteArrayContent(buffer);
			byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			return byteContent;
		}
	}
}
