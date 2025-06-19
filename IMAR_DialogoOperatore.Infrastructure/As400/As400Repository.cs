using Dapper;
using IMAR_DialogoOperatore.Application.Interfaces.Repositories;
using System.Data.Odbc;

namespace IMAR_DialogoOperatore.Infrastructure.As400
{
    public class As400Repository : IAs400Repository
	{
		private As400Context _as400Context;

		public As400Repository(As400Context as400Context)
		{
			_as400Context = as400Context;
        }

		public IEnumerable<T> ExecuteQuery<T>(string query)
        {
            using (var connection = (OdbcConnection)_as400Context.CreateConnection())
            {
                connection.Open();
                return connection.Query<T>(query).ToList();
            }
        }

		public IEnumerable<T> GetAll<T>()
		{
			string query = $" select * from IMA90DAT.{typeof(T).Name} ";
            return ExecuteQuery<T>(query);
		}
	}
}
