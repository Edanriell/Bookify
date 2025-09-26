using Bookify.Application.Abstractions.Messaging;
using Bookify.Domain.Abstractions;
using Bookify.Modules.Users.Application.Abstractions.Authentication;
using Bookify.Modules.Users.Domain.Users;

namespace Bookify.Modules.Users.Application.Users.LogInUser;

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
		Result<string> result = await _jwtService.GetAccessTokenAsync(
			request.Email,
			request.Password,
			cancellationToken);

		if (result.IsFailure)
		{
			return Result.Failure<AccessTokenResponse>(UserErrors.InvalidCredentials);
		}

		return new AccessTokenResponse(result.Value);
	}
}
