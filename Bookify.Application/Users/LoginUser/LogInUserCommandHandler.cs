using Bookify.Application.Abstractions.Authentication;
using Bookify.Application.Abstractions.Messaging;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Users;

namespace Bookify.Application.Users.LoginUser;

// Command is using an IJwtService, which is responsible for sending our credentials
// to the KeyCloak Api and obtaining an access token that we are going to return from our command handler.
internal sealed class LogInUserCommandHandler : ICommandHandler<LogInUserCommand, AccessTokenResponse>
{
	private readonly IJwtService _jwtService;

	public LogInUserCommandHandler(IJwtService jwtService)
	{
		_jwtService = jwtService;
	}

	public async Task<Result<AccessTokenResponse>> Handle(
		LogInUserCommand request,
		CancellationToken cancellationToken)
	{
		var result = await _jwtService.GetAccessTokenAsync(
						 request.Email,
						 request.Password,
						 cancellationToken);

		if (result.IsFailure) return Result.Failure<AccessTokenResponse>(UserErrors.InvalidCredentials);

		return new AccessTokenResponse(result.Value);
	}
}