using Bookify.Domain.Apartments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookify.Infrastructure.Configurations;

// Fluent Configuration approach using the EntityTypeBuilder class.
internal sealed class ApartmentConfiguration : IEntityTypeConfiguration<Apartment>
{
	public void Configure(EntityTypeBuilder<Apartment> builder)
	{
		// The First call here is going to map the apartment entity in our domain model to the apartments
		// table in the database.
		builder.ToTable("apartments");

		// Defining a primary key.
		builder.HasKey(apartment => apartment.Id);

		// Mapping an address value object, which is a complex object, with a few properties using an owned entity.
		// How this works with EF Core is the value object is going to be mapped into a set of columns in the same 
		// table as the owning entity, so in this case, the address columns are going to be in the apartments table.
		// If this is a collection, we have support for calling ownsMany and mapping a collection of objects,
		// but in that case, the owned collection is going to be mepped into a separate table. 
		builder.OwnsOne(apartment => apartment.Address);

		// For simple value objects like the name and description, we can set the maximum length to 200 and 2000 resoectively,
		// and we're going to define a simple conversion from the value object to the primitive type in the database and from the
		// primitive type back into the value object. 
		builder.Property(apartment => apartment.Name)
		   .HasMaxLength(200)
		   .HasConversion(name => name.Value, value => new Name(value));

		// For mapping the price and the cleaning fee which are money value objects,
		// we are also using the owned entity approach with the added step of defining a conversion for the currency which 
		// contains a code inside, so we are only mapping the currency code to the database. 
		builder.OwnsOne(apartment => apartment.Price, priceBuilder =>
		{
			priceBuilder.Property(money => money.Currency)
			   .HasConversion(currency => currency.Code, code => Currency.FromCode(code));
		});

		builder.OwnsOne(apartment => apartment.CleaningFee, priceBuilder =>
		{
			priceBuilder.Property(money => money.Currency)
			   .HasConversion(currency => currency.Code, code => Currency.FromCode(code));
		});

		// Optimistic concurrency is a mechanism that relies on having some column in the database acting
		// as the version for that row. Usually, this is a database-generated value, and it's used
		// when persisting changes to the database to check if the version that is currently in the database is different
		// from the one that we have in our application at the time we loaded the entity. If those versions are not
		// a match, it means that somebody has changed this row in the database before we could, and we throw
		// a database concurrency exception, which is the case that we are handling.
		// IMPORTANT! We can't use the booking because it doesn't exist in the database until we call save changes, so
		// that leaves us with either using the user or the apartment. If we look at the booking reserve method, we have a nice
		// little call, where we are setting the last booked on value to the current UTC now time (apartment.LastBookedOnUtc = utcNow).
		// This is going to trigger an update on the apartments table for this specific row, which we are making a booking for,
		// so we are going to define a row version column on the apartment entity, which is going to help us implement optimistic concurrency. 
		builder.Property<uint>("Version").IsRowVersion();
		// We are defining a shadow property on our apartments entity, which we are calling version. We are using unsigned integer as the
		// type for the row version, and we are calling IsRowVersion() method, which is going to tell EF Core 
		// that this column should be interpreted as a row version for implementing optimistic concurrency support. 
		// The really ice thing about the Postgre implementation of optimistic concurrency is that it's going to use a system column,
		// which is the xmin column, and it holds the value of the last updating transaction. 
	}
}