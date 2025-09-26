using Bookify.Application.Abstractions.Messaging;

namespace Bookify.Modules.Users.Application.Users.RegisterUser;

public sealed record RegisterUserCommand(
	string Email,
	string FirstName,
	string LastName,
	string Password) : ICommand<Guid>;
