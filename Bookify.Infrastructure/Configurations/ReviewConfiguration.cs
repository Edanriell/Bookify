using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;
using Bookify.Domain.Reviews;
using Bookify.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookify.Infrastructure.Configurations;

internal sealed class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
	public void Configure ( EntityTypeBuilder<Review> builder )
	{
		builder.ToTable (
				name : "reviews"
			);

		builder.HasKey (
				keyExpression : review => review.Id
			);

		builder.Property (
					propertyExpression : review => review.Rating
				).
			HasConversion (
					convertToProviderExpression : rating => rating.Value,
					convertFromProviderExpression : value => Rating.Create (
								value
							).
						Value
				);

		builder.Property (
					propertyExpression : review => review.Comment
				).
			HasMaxLength (
					maxLength : 200
				).
			HasConversion (
					convertToProviderExpression : comment => comment.Value,
					convertFromProviderExpression : value => new Comment (
							value
						)
				);

		builder.HasOne<Apartment>().
			WithMany().
			HasForeignKey (
					foreignKeyExpression : review => review.ApartmentId
				);

		builder.HasOne<Booking>().
			WithMany().
			HasForeignKey (
					foreignKeyExpression : review => review.BookingId
				);

		builder.HasOne<User>().
			WithMany().
			HasForeignKey (
					foreignKeyExpression : review => review.UserId
				);
	}
}
