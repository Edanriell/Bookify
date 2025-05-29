using Bookify.Domain.Apartments;
using Bookify.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookify.Infrastructure.Configurations;

internal sealed class ApartmentConfiguration : IEntityTypeConfiguration<Apartment>
{
	public void Configure ( EntityTypeBuilder<Apartment> builder )
	{
		builder.ToTable (
				name : "apartments"
			);

		builder.HasKey (
				keyExpression : apartment => apartment.Id
			);

		builder.OwnsOne (
				navigationExpression : apartment => apartment.Address
			);

		builder.Property (
					propertyExpression : apartment => apartment.Name
				).
			HasMaxLength (
					maxLength : 200
				).
			HasConversion (
					convertToProviderExpression : name => name.Value,
					convertFromProviderExpression : value => new Name (
							value
						)
				);

		builder.Property (
					propertyExpression : apartment => apartment.Description
				).
			HasMaxLength (
					maxLength : 2000
				).
			HasConversion (
					convertToProviderExpression : description => description.Value,
					convertFromProviderExpression : value => new Description (
							value
						)
				);

		builder.OwnsOne (
				navigationExpression : apartment => apartment.Price,
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
				navigationExpression : apartment => apartment.CleaningFee,
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

		builder.Property<uint> (
					propertyName : "Version"
				).
			IsRowVersion();
	}
}
