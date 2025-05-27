using Bookify.Domain.UnitTests.Infrastructure;
using Bookify.Domain.Users;
using Bookify.Domain.Users.Events;
using FluentAssertions;

namespace Bookify.Domain.UnitTests.Users;

public class UserTests : BaseTest
{
	[ Fact ]
	public void Create_Should_SetPropertyValue()
	{
		// Act
		var user = User.Create (
				firstName : UserData.FirstName,
				lastName : UserData.LastName,
				email : UserData.Email
			);

		// Assert
		user.FirstName.Should().
			Be (
					expected : UserData.FirstName
				);
		user.LastName.Should().
			Be (
					expected : UserData.LastName
				);
		user.Email.Should().
			Be (
					expected : UserData.Email
				);
	}

	[ Fact ]
	public void Create_Should_RaiseUserCreatedDomainEvent()
	{
		// Act
		var user = User.Create (
				firstName : UserData.FirstName,
				lastName : UserData.LastName,
				email : UserData.Email
			);

		// Assert
		var userCreatedDomainEvent = AssertDomainEventWasPublished<UserCreatedDomainEvent> (
				entity : user
			);

		userCreatedDomainEvent.UserId.Should().
			Be (
					expected : user.Id
				);
	}

	[ Fact ]
	public void Create_Should_AddRegisteredRoleToUser()
	{
		// Act
		var user = User.Create (
				firstName : UserData.FirstName,
				lastName : UserData.LastName,
				email : UserData.Email
			);

		// Assert
		user.Roles.Should().
			Contain (
					expected : Role.Registered
				);
	}
}
