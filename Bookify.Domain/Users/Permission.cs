namespace Bookify.Domain.Users;

// Permission-based Authorization
// Alternative approach to this would be using enums to define our roles and permissions
// and it might be better in terms of maintenance from a code perspective.
// However we prefer to manage our roles and permissions from the database because we can update roles and
// permissions without making changes to our code and redeploying the application. 
// When it comes to the permission entity, we want to associate it with particular role
// so the roles themselves will have a collection of permissions, which is why we have a permissions navigation property.
public sealed class Permission
{
	public static readonly Permission UsersRead = new(1, "users:read");

	public Permission(int id, string name)
	{
		Id = id;
		Name = name;
	}

	public int Id { get; init; }

	public string Name { get; init; }
}