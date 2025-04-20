using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Users;

// Static Factory pattern
// Benefits of using a static factory method:
// 1) Hiding constructor which could have some other implementation details that we don't want to expose outside of the user entity
// 2) Encapsulation
public sealed class User : Entity
{
	// Accepts an Id and passes it to the base class constructor
	private User(Guid id, FirstName firstName, LastName lastName, Email email) : base(id)
	{
		FirstName = firstName;
		LastName = lastName;
		Email = email;
	}

	public FirstName FirstName { get; private set; }
	public LastName LastName { get; private set; }
	public Email Email { get; private set; }

	public static User Create(FirstName firstName, LastName lastName, Email email)
	{
		var user = new User(Guid.NewGuid(), firstName, lastName, email);

		return user;
	}
}