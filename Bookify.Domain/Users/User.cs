using Bookify.Domain.Abstractions;
using Bookify.Domain.Users.Events;

namespace Bookify.Domain.Users;

// Static Factory pattern
// Benefits of using a static factory method:
// 1) Hiding constructor which could have some other implementation details that we don't want to expose outside of the user entity
// 2) Encapsulation
public sealed class User : Entity
{
	// Role-based Authorization
	// Contains roles which user has.
	// Default value is a empty list.
	// This will allow us to encapsulate the roles for our
	// user so that they can only be added or removed using the user entity.
	// Which makes our user a kind of an aggregate and where we are going to 
	// assign the role is right after creating the user. 
	private readonly List<Role> _roles = new();

	// Accepts an Id and passes it to the base class constructor
	private User(Guid id, FirstName firstName, LastName lastName, Email email)
		: base(id)
	{
		FirstName = firstName;
		LastName = lastName;
		Email = email;
	}

	// Fix for migrations
	private User()
	{
	}

	public FirstName FirstName { get; private set; }

	public LastName LastName { get; private set; }

	public Email Email { get; private set; }

	// We are assigning the initial value of string empty, because value is not set through the constructor.
	public string IdentityId { get; private set; } = string.Empty;

	// Role-based Authorization
	// We are exposing another property that is read only collection of roles
	// The value of this property is roles backing field, and we are calling
	// method ToList() to make a copy that's going to be returned as the read only value.
	public IReadOnlyCollection<Role> Roles => _roles.ToList();

	public static User Create(FirstName firstName, LastName lastName, Email email)
	{
		var user = new User(Guid.NewGuid(), firstName, lastName, email);

		// When we persist the user object in the database, we are also going to publish the UserCreatedDomainEvent
		// In result someone can subscribe to this event and execute some behavior asynchronously.
		// An example of that could be sending an welcome email to the user when they register to the system or
		// doing some background work to further set up the user so our system could be used properly.
		user.RaiseDomainEvent(new UserCreatedDomainEvent(user.Id));

		// Role-based Authorization
		// Adding a Registered role to the user, on user creation
		// If we need to support more than one type of role we can pass in the role as
		// another argument to the create method for example right after the email
		// or we can have dedicated factory methods for each type of role that we want to support.
		user._roles.Add(Role.Registered);

		return user;
	}

	public void SetIdentityId(string identityId)
	{
		IdentityId = identityId;
	}
}