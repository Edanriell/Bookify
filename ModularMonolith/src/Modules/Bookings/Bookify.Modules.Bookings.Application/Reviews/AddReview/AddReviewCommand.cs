using Bookify.Application.Abstractions.Messaging;

namespace Bookify.Modules.Bookings.Application.Reviews.AddReview;

public sealed record AddReviewCommand(Guid BookingId, int Rating, string Comment) : ICommand;
