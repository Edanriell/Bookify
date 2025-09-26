using Bookify.Application.Abstractions.Messaging;

namespace Bookify.Modules.Users.Application.Users.UpdateUser;

public sealed record UpdateUserProfileCommand(Guid UserId, string FirstName, string LastName) : ICommand;
