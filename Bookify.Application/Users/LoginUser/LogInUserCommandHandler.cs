using Bookify.Application.Abstractions.Authentication;
using Bookify.Application.Abstractions.Messaging;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Users;

namespace Bookify.Application.Users.LogInUser;

internal sealed class LogInUserCommandHandler : ICommandHandler<LogInUserCommand, AccessTokenResponse>
{
	private readonly IJwtService _jwtService;

	public LogInUserCommandHandler ( IJwtService jwtService ) { _jwtService = jwtService; }

	public async Task<Result<AccessTokenResponse>> Handle ( LogInUserCommand request,
															CancellationToken cancellationToken )
	{
		var result = await _jwtService.GetAccessTokenAsync (
							 email : request.Email,
							 password : request.Password,
							 cancellationToken : cancellationToken
						 );

		if ( result.IsFailure )
			return Result.Failure<AccessTokenResponse> (
					error : UserErrors.InvalidCredentials
				);

		return new AccessTokenResponse (
				AccessToken : result.Value
			);
	}
}
