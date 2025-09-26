using Bookify.Application.Abstractions.Messaging;

namespace Bookify.Modules.Users.Application.Users.LogInUser;

public sealed record LogInUserCommand(string Email, string Password)
	: ICommand<AccessTokenResponse>;
