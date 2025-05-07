using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;
using Bookify.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookify.Infrastructure.Configurations;

internal sealed class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
	public void Configure(EntityTypeBuilder<Booking> builder)
	{
		builder.ToTable("bookings");

		builder.HasKey(booking => booking.Id);

		builder.OwnsOne(booking => booking.PriceForPeriod, priceBuilder =>
		{
			priceBuilder.Property(money => money.Currency)
			   .HasConversion(currency => currency.Code, code => Currency.FromCode(code));
		});

		builder.OwnsOne(booking => booking.CleaningFee, priceBuilder =>
		{
			priceBuilder.Property(money => money.Currency)
			   .HasConversion(currency => currency.Code, code => Currency.FromCode(code));
		});

		builder.OwnsOne(booking => booking.AmenitiesUpCharge, priceBuilder =>
		{
			priceBuilder.Property(money => money.Currency)
			   .HasConversion(currency => currency.Code, code => Currency.FromCode(code));
		});

		builder.OwnsOne(booking => booking.TotalPrice, priceBuilder =>
		{
			priceBuilder.Property(money => money.Currency)
			   .HasConversion(currency => currency.Code, code => Currency.FromCode(code));
		});

		builder.OwnsOne(booking => booking.Duration);

		// Configuring relationships from our booking entity to the apartment by saying has one apartment 
		// an apartment could have many bookings, which is why we have a call to with many, and we are 
		// defining which property on the booking entity is our foreign key. The Same approach is for User.
		builder.HasOne<Apartment>().WithMany().HasForeignKey(booking => booking.ApartmentId);

		builder.HasOne<User>().WithMany().HasForeignKey(booking => booking.UserId);
	}
}