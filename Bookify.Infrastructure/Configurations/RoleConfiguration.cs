using Bookify.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookify.Infrastructure.Configurations;

// Role-based Authorization
internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
	public void Configure(EntityTypeBuilder<Role> builder)
	{
		// Creating roles table
		builder.ToTable("roles");

		// Primary key
		builder.HasKey(role => role.Id);

		// Role has many users, and users can have or has many roles
		builder.HasMany(role => role.Users)
		   .WithMany(user => user.Roles);

		// Seeding initial roles (data)
		builder.HasData(Role.Registered);
	}
}