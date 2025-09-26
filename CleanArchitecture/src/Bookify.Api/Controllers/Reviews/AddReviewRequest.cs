namespace Bookify.Api.Controllers.Reviews;

internal sealed record AddReviewRequest(Guid BookingId, int Rating, string Comment);
