namespace IMAR_DialogoOperatore.Application.Interfaces.Services
{
    public interface IUtenteService
    {
        /// <summary>
        /// Restituisce l'IdAsana dell'utente con il badge specificato, o null se non trovato.
        /// </summary>
        Task<string?> GetIdAsanaByBadgeAsync(string badge);
    }
}
