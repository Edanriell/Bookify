using Bookify.Modules.Users.Domain.Users;

namespace Bookify.Modules.Users.Application.Abstractions.Authentication;

public interface IAuthenticationService
{
	Task<string> RegisterAsync(
		User user,
		string password,
		CancellationToken cancellationToken = default);
}
