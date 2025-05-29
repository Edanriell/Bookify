using Bookify.Domain.Abstractions;
using Bookify.Domain.Users.Events;

namespace Bookify.Domain.Users;

public sealed class User : Entity
{
	private readonly List<Role> _roles = new();

	private User ( Guid id, FirstName firstName, LastName lastName, Email email )
		: base (
				id : id
			)
	{
		FirstName = firstName;
		LastName = lastName;
		Email = email;
	}

	private User() { }

	public FirstName FirstName { get; private set; }

	public LastName LastName { get; private set; }

	public Email Email { get; private set; }

	public string IdentityId { get; private set; } = string.Empty;

	public IReadOnlyCollection<Role> Roles => _roles.ToList();

	public static User Create ( FirstName firstName, LastName lastName, Email email )
	{
		var user = new User (
				id : Guid.NewGuid(),
				firstName : firstName,
				lastName : lastName,
				email : email
			);

		user.RaiseDomainEvent (
				domainEvent : new UserCreatedDomainEvent (
						UserId : user.Id
					)
			);

		user._roles.Add (
				item : Role.Registered
			);

		return user;
	}

	public void SetIdentityId ( string identityId ) { IdentityId = identityId; }
}
