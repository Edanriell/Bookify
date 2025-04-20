using Bookify.Domain.Abstractions;
using Bookify.Domain.Users.Events;

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

		// When we persist the user object in the database, we are also going to publish the UserCreatedDomainEvent
		// In result someone can subscribe to this event and execute some behavior asynchronously.
		// An example of that could be sending an welcome email to the user when they register to the system or
		// doing some background work to further set up the user so our system could be used properly.
		user.RaiseDomainEvent(new UserCreatedDomainEvent(user.Id));

		return user;
	}
}