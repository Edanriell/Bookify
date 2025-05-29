using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;
using Bookify.Domain.Shared;
using Bookify.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookify.Infrastructure.Configurations;

internal sealed class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
	public void Configure ( EntityTypeBuilder<Booking> builder )
	{
		builder.ToTable (
				name : "bookings"
			);

		builder.HasKey (
				keyExpression : booking => booking.Id
			);

		builder.OwnsOne (
				navigationExpression : booking => booking.PriceForPeriod,
				buildAction : priceBuilder =>
				{
					priceBuilder.Property (
								propertyExpression : money => money.Currency
							).
						HasConversion (
								convertToProviderExpression : currency => currency.Code,
								convertFromProviderExpression : code => Currency.FromCode (
										code
									)
							);
				}
			);

		builder.OwnsOne (
				navigationExpression : booking => booking.CleaningFee,
				buildAction : priceBuilder =>
				{
					priceBuilder.Property (
								propertyExpression : money => money.Currency
							).
						HasConversion (
								convertToProviderExpression : currency => currency.Code,
								convertFromProviderExpression : code => Currency.FromCode (
										code
									)
							);
				}
			);

		builder.OwnsOne (
				navigationExpression : booking => booking.AmenitiesUpCharge,
				buildAction : priceBuilder =>
				{
					priceBuilder.Property (
								propertyExpression : money => money.Currency
							).
						HasConversion (
								convertToProviderExpression : currency => currency.Code,
								convertFromProviderExpression : code => Currency.FromCode (
										code
									)
							);
				}
			);

		builder.OwnsOne (
				navigationExpression : booking => booking.TotalPrice,
				buildAction : priceBuilder =>
				{
					priceBuilder.Property (
								propertyExpression : money => money.Currency
							).
						HasConversion (
								convertToProviderExpression : currency => currency.Code,
								convertFromProviderExpression : code => Currency.FromCode (
										code
									)
							);
				}
			);

		builder.OwnsOne (
				navigationExpression : booking => booking.Duration
			);

		builder.HasOne<Apartment>().
			WithMany().
			HasForeignKey (
					foreignKeyExpression : booking => booking.ApartmentId
				);

		builder.HasOne<User>().
			WithMany().
			HasForeignKey (
					foreignKeyExpression : booking => booking.UserId
				);
	}
}
