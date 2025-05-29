using Bookify.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookify.Infrastructure.Configurations;

internal sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
	public void Configure ( EntityTypeBuilder<Permission> builder )
	{
		builder.ToTable (
				name : "permissions"
			);

		builder.HasKey (
				keyExpression : permission => permission.Id
			);

		builder.HasData (
				Permission.UsersRead
			);
	}
}
