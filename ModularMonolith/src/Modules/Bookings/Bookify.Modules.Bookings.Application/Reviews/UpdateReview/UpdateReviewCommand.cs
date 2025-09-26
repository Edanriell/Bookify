using Bookify.Application.Abstractions.Messaging;

namespace Bookify.Modules.Bookings.Application.Reviews.UpdateReview;

public sealed record UpdateReviewCommand(Guid ReviewId, int Rating, string Comment) : ICommand;
