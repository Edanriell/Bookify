using Bookify.Application.Abstractions.Messaging;

namespace Bookify.Modules.Users.Application.Users.GetLoggedInUser;

public sealed record GetLoggedInUserQuery : IQuery<UserResponse>;
