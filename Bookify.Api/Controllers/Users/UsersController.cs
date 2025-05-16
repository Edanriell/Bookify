using Bookify.Application.Users.GetLoggedInUser;
using Bookify.Application.Users.LogInUser;
using Bookify.Application.Users.RegisterUser;
using Bookify.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Api.Controllers.Users;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
	// We are injecting the mediator service into our controller, which is called ISender,
	// and it comes from MediatR it used to send our commands or queries from this controller.
	private readonly ISender _sender;

	public UsersController(ISender sender)
	{
		_sender = sender;
	}

	[HttpGet("me")]
	// Role-based Authorization
	// We use authorize attribute to specify the role that we require authenticated users to have
	// to be able to call this endpoint. Otherwise they are going to get the
	// forbidden response telling them that they do not have the required access. 
	// IMPORTANT! ASP.NET Core uses a specific claim on the JSON Web Token to determine if the
	// user has this claim. We need to set it manually! ASP.NET Core has support for modifying a users
	// claims we can do this through the IClaimsTransformation interface.  
//	[Authorize(Roles = "Registered")]
	// We also could use [HasRole("Registered)]
//	[Authorize(Roles = Roles.Registered)]
	// Permission-based Authorization
	// Permission based authorization is a way to apply policy based
	// authorization in ASP.NET Core
	// Roles based authorization is a very coarse grained approach to authorization, where users
	// either belong to a role or they do not. Permission based authorization is much more fine
	// grained than this, because we can create custom permissions for each use case or endpoint
	// in our API. We can also decide which role or which group of users will have access to
	// particular permissions, so in that regard, it is much more flexible solution. It is also very
	// helpful if we can manage permissions from the database. 
//	[Authorize(Policy = "users:read")]
//	[HasPermission("users:read")]
	// IMPORTANT! If we want we can have role based and permission based authorization, but thus doesent really
	// make sense because the permissions are connected to the roles, so we can just leave out the roles and only
	// rely on permission based authorization. 
	[HasPermission(Permissions.UsersRead)]
	public async Task<IActionResult> GetLoggedInUser(CancellationToken cancellationToken)
	{
		var query = new GetLoggedInUserQuery();

		var result = await _sender.Send(query, cancellationToken);

		return Ok(result.Value);
	}

	// We are allowing anonymous requests so that anyone can
	// register to the system and it's going to accept
	// a register user request in the request body. 
	[AllowAnonymous]
	[HttpPost("register")]
	public async Task<IActionResult> Register(
		RegisterUserRequest request,
		CancellationToken cancellationToken)
	{
		// In our body of the endpoint, we are mapping our request into the command.
		var command = new RegisterUserCommand(
			request.Email,
			request.FirstName,
			request.LastName,
			request.Password);

		// We are sending the command and returning
		var result = await _sender.Send(command, cancellationToken);

		// If the result is a failure, we want to return a 400 bad request containing the error. 
		if (result.IsFailure) return BadRequest(result.Error);

		// An okay response from the route.
		return Ok(result.Value);
	}

	[AllowAnonymous]
	[HttpPost("login")]
	public async Task<IActionResult> LogIn(
		LogInUserRequest request,
		CancellationToken cancellationToken)
	{
		var command = new LogInUserCommand(request.Email, request.Password);

		var result = await _sender.Send(command, cancellationToken);

		if (result.IsFailure) return Unauthorized(result.Error);

		return Ok(result.Value);
	}
}