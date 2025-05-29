using Asp.Versioning;
using Bookify.Application.Users.GetLoggedInUser;
using Bookify.Application.Users.LogInUser;
using Bookify.Application.Users.RegisterUser;
using Bookify.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Api.Controllers.Users;

[ ApiController ]
[ ApiVersion (
		version : ApiVersions.V1
	) ]
[ Route (
		template : "api/v{version:apiVersion}/users"
	) ]
public class UsersController : ControllerBase
{
	private readonly ISender _sender;

	public UsersController ( ISender sender ) { _sender = sender; }

	[ HttpGet (
			template : "me"
		) ]
	[ HasPermission (
			permission : Permissions.UsersRead
		) ]
	public async Task<IActionResult> GetLoggedInUser ( CancellationToken cancellationToken )
	{
		var query = new GetLoggedInUserQuery();

		var result = await _sender.Send (
							 request : query,
							 cancellationToken : cancellationToken
						 );

		return Ok (
				value : result.Value
			);
	}

	[ AllowAnonymous ]
	[ HttpPost (
			template : "register"
		) ]
	public async Task<IActionResult> Register ( RegisterUserRequest request,
												CancellationToken cancellationToken )
	{
		var command = new RegisterUserCommand (
				Email : request.Email,
				FirstName : request.FirstName,
				LastName : request.LastName,
				Password : request.Password
			);

		var result = await _sender.Send (
							 request : command,
							 cancellationToken : cancellationToken
						 );

		if ( result.IsFailure )
			return BadRequest (
					error : result.Error
				);

		return Ok (
				value : result.Value
			);
	}

	[ AllowAnonymous ]
	[ HttpPost (
			template : "login"
		) ]
	public async Task<IActionResult> LogIn ( LogInUserRequest request,
											 CancellationToken cancellationToken )
	{
		var command = new LogInUserCommand (
				Email : request.Email,
				Password : request.Password
			);

		var result = await _sender.Send (
							 request : command,
							 cancellationToken : cancellationToken
						 );

		if ( result.IsFailure )
			return Unauthorized (
					value : result.Error
				);

		return Ok (
				value : result.Value
			);
	}
}
