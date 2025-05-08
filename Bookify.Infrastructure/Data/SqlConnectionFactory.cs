using System.Data;
using Bookify.Application.Abstractions.Data;
using Npgsql;

namespace Bookify.Infrastructure.Data;

internal sealed class SqlConnectionFactory : ISqlConnectionFactory
{
	private readonly string _connectionString;

	public SqlConnectionFactory(string connectionString)
	{
		_connectionString = connectionString;
	}

	public IDbConnection CreateConnection()
	{
		// Creating a new Npgsql connection
		var connection = new NpgsqlConnection(_connectionString);
		// Opening connection
		connection.Open();
 
		return connection;
	}
}