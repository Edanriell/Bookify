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
	}
}