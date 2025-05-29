using Bookify.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookify.Infrastructure.Configurations;

internal sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
	public void Configure ( EntityTypeBuilder<RolePermission> builder )
	{
		builder.ToTable (
				name : "role_permissions"
			);

		builder.HasKey (
				keyExpression : rolePermission => new
												  {
													  rolePermission.RoleId,
													  rolePermission.PermissionId
												  }
			);

		builder.HasData (
				new RolePermission
				{
					RoleId = Role.Registered.Id,
					PermissionId = Permission.UsersRead.Id
				}
			);
	}
}
