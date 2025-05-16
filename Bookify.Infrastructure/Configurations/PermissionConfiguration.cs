using Bookify.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookify.Infrastructure.Configurations;

// Permission-based Authorization
internal sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
	public void Configure(EntityTypeBuilder<Permission> builder)
	{
		builder.ToTable("permissions");

		builder.HasKey(permission => permission.Id);

		// Seeding initial permissions (data)
		// We can have as many or as few permissions as we want
		// This will all depend on the level of granularity and control that we are enforcing,
		// using permission based authorization
		builder.HasData(Permission.UsersRead);
	}
}