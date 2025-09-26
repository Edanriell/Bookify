using Bookify.Application.Abstractions.Messaging;

namespace Bookify.Modules.Bookings.Application.Reviews.DeleteReview;

public sealed record DeleteReviewCommand(Guid ReviewId) : ICommand;
