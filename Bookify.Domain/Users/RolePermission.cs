namespace Bookify.Domain.Users;

// Permission-based Authorization
// Many to many role permission entity
// We are doing this so that we can see these values from our EF core migration
// but of course this is totally optional (We could manage this directly from the database)
public class RolePermission
{
	public int RoleId { get; set; }

	public int PermissionId { get; set; }
}