using Bookify.Domain.Abstractions;

namespace Bookify.Modules.Users.Application.Abstractions.Authentication;

public interface IJwtService
{
	Task<Result<string>> GetAccessTokenAsync(
		string email,
		string password,
		CancellationToken cancellationToken = default);
}
