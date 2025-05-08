using Bookify.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookify.Infrastructure.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> builder)
	{
		builder.ToTable("users");

		builder.HasKey(user => user.Id);

		builder.Property(user => user.FirstName)
		   .HasMaxLength(200)
		   .HasConversion(firstName => firstName.Value, value => new FirstName(value));

		builder.Property(user => user.LastName)
		   .HasMaxLength(200)
		   .HasConversion(firstName => firstName.Value, value => new LastName(value));

		builder.Property(user => user.Email)
		   .HasMaxLength(400)
		   .HasConversion(email => email.Value, value => new Domain.Users.Email(value));

		// Defining an index on the email property or rather the email column in the database, and we are
		// saying that this is a unique index. This is going to give us a database-guaranteed constraint 
		// that the email is going to be unique across the user's table, which is an important quality that we want from the database.
		builder.HasIndex(user => user.Email).IsUnique();
	}
}