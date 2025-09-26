using Bookify.Modules.Bookings.Domain.Apartments;
using Bookify.Modules.Bookings.Domain.Bookings;
using Bookify.Modules.Bookings.Domain.Reviews;
using Bookify.Modules.Users.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookify.Infrastructure.Configurations;

internal sealed class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
	public void Configure(EntityTypeBuilder<Review> builder)
	{
		builder.ToTable("reviews", Schemas.Bookings);

		builder.HasKey(review => review.Id);

		builder.Property(review => review.Rating)
			.HasConversion(rating => rating.Value, value => Rating.Create(value).Value);

		builder.Property(review => review.Comment)
			.HasMaxLength(200)
			.HasConversion(comment => comment.Value, value => new Comment(value));

		builder.HasOne<Apartment>()
			.WithMany()
			.HasForeignKey(review => review.ApartmentId);

		builder.HasOne<Booking>()
			.WithMany()
			.HasForeignKey(review => review.BookingId);

		builder.HasOne<User>()
			.WithMany()
			.HasForeignKey(review => review.UserId);
	}
}
