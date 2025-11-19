using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.Odbc;

namespace IMAR_DialogoOperatore.Infrastructure.As400
{
	public class As400Context
	{
		private readonly string _connectionString;

		public As400Context(IConfiguration configuration)
		{
			_connectionString = configuration["ConnectionStrings:As400"];
		}

		public IDbConnection CreateConnection()
			 => new OdbcConnection(_connectionString);

	}
}
