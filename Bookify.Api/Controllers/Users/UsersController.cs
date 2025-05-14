using Bookify.Application.Users.RegisterUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Api.Controllers.Users;

[ApiController]
[Microsoft.AspNetCore.Components.Route("api/user")]
public class UsersController : ControllerBase
{
	// We are injecting the mediator service into our controller, which is called ISender,
	// and it comes from MediatR it used to send our commands or queries from this controller.
	private readonly ISender _sender;

	public UsersController(ISender sender)
	{
		_sender = sender;
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
}