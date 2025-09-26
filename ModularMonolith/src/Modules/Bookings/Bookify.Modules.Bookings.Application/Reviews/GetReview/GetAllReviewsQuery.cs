using Bookify.Application.Abstractions.Messaging;

namespace Bookify.Modules.Bookings.Application.Reviews.GetReview;

public sealed record GetAllReviewsQuery : IQuery<IReadOnlyList<ReviewResponse>>;
