using Bookify.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookify.Infrastructure.Configurations;

// Permission-based Authorization
public sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
	public void Configure(EntityTypeBuilder<RolePermission> builder)
	{
		builder.ToTable("role_permissions");

		// Here we are using a composite key. We are using the role permission to
		// instantiate a new object that is going to contain our composite key, and
		// the components will be the role id and the permission id as the unique values.
		builder.HasKey(rolePermission => new { rolePermission.RoleId, rolePermission.PermissionId });

		// We are seeding an initial data by saying HasData, and we can pass in a new role permission
		// instance and assign the primary keys. 
		builder.HasData(
			new RolePermission
			{
				RoleId = Role.Registered.Id,
				PermissionId = Permission.UsersRead.Id
			});
	}
}