using Dapper;
using IMAR_DialogoOperatore.Application.Interfaces.Repositories;
using System.Data.Odbc;

namespace IMAR_DialogoOperatore.Infrastructure.As400
{
    public class As400Repository : IAs400Repository
	{
		private As400Context _as400Context;
		private OdbcConnection _connection;

		public As400Repository(As400Context as400Context)
		{
			_as400Context = as400Context;

            _connection = (OdbcConnection)_as400Context.CreateConnection();
        }

		public IEnumerable<T> ExecuteQuery<T>(string query)
		{
			var result = _connection.Query<T>(query);
			return result;
		}

		public IEnumerable<T> GetAll<T>()
		{
			string query = $" select * from IMA90DAT.{typeof(T).Name} ";
			var result = _connection.Query<T>(query);
			return result;
		}
	}
}
