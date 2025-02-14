namespace IMAR_DialogoOperatore.Application.Interfaces.Utilities
{
	public interface IHttpClientUtility
	{
		public Task<HttpClient> BuildAuthenticatedClient(string urlLogin);
	}
}
