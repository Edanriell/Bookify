using Bookify.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookify.Infrastructure.Configurations;

internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
	public void Configure ( EntityTypeBuilder<Role> builder )
	{
		builder.ToTable (
				name : "roles"
			);

		builder.HasKey (
				keyExpression : role => role.Id
			);

		builder.HasMany (
					navigationExpression : role => role.Permissions
				).
			WithMany().
			UsingEntity<RolePermission>();

		builder.HasData (
				Role.Registered
			);
	}
}
