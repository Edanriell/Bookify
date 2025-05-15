using System.Data;

namespace Bookify.Application.Abstractions.Data;

// Returns new database connect to our SQL database. 
public interface ISqlConnectionFactory
{
	IDbConnection CreateConnection();
} 