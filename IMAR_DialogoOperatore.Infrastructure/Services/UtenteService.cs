using Dapper;
using IMAR_DialogoOperatore.Application.Interfaces.Services;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace IMAR_DialogoOperatore.Infrastructure.Services
{
    public class UtenteService : IUtenteService
    {
        private readonly string _connectionString;

        public UtenteService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ImarUserAssetBase")!;
        }

        public async Task<string?> GetIdAsanaByBadgeAsync(string badge)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<string?>(
                "SELECT IdAsana FROM Utenti WHERE BadgeDipendente = @Badge",
                new { Badge = badge });
        }
    }
}
