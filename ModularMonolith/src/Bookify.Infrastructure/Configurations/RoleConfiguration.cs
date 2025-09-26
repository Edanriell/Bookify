using Bookify.Modules.Users.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookify.Infrastructure.Configurations;

internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
	public void Configure(EntityTypeBuilder<Role> builder)
	{
		builder.ToTable("roles", Schemas.Users);

		builder.HasKey(role => role.Id);

		builder.HasMany(role => role.Permissions)
			.WithMany()
			.UsingEntity<RolePermission>();

		builder.HasData(Role.Registered);
	}
}
