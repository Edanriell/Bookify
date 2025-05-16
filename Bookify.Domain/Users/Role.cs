namespace Bookify.Domain.Users;

// Role-based Authorization
public sealed class Role
{
	// Contains predetermined roles
	// This is the same role that we were referencing in our controller, only now
	// we are defining it as part of our domain which is going to allow us to manage
	// our roles from the domain layer and even seed them using EF core.
	public static readonly Role Registered = new(1, "Registered");

	public Role(int id, string name)
	{
		Id = id;
		Name = name;
	}

	public int Id { get; init; }

	public string Name { get; init; } = string.Empty;

	// Navigation property, which points to the users that have this role
	public ICollection<User> Users { get; init; } = new List<User>();
}