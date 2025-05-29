using Bookify.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookify.Infrastructure.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
	public void Configure ( EntityTypeBuilder<User> builder )
	{
		builder.ToTable (
				name : "users"
			);

		builder.HasKey (
				keyExpression : user => user.Id
			);

		builder.Property (
					propertyExpression : user => user.FirstName
				).
			HasMaxLength (
					maxLength : 200
				).
			HasConversion (
					convertToProviderExpression : firstName => firstName.Value,
					convertFromProviderExpression : value => new FirstName (
							value
						)
				);

		builder.Property (
					propertyExpression : user => user.LastName
				).
			HasMaxLength (
					maxLength : 200
				).
			HasConversion (
					convertToProviderExpression : firstName => firstName.Value,
					convertFromProviderExpression : value => new LastName (
							value
						)
				);

		builder.Property (
					propertyExpression : user => user.Email
				).
			HasMaxLength (
					maxLength : 400
				).
			HasConversion (
					convertToProviderExpression : email => email.Value,
					convertFromProviderExpression : value => new Domain.Users.Email (
							value
						)
				);

		builder.HasIndex (
					indexExpression : user => user.Email
				).
			IsUnique();

		builder.HasIndex (
					indexExpression : user => user.IdentityId
				).
			IsUnique();
	}
}
