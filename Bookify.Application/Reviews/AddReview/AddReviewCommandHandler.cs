using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Abstractions.Messaging;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Bookings;
using Bookify.Domain.Reviews;

namespace Bookify.Application.Reviews.AddReview;

internal sealed class AddReviewCommandHandler : ICommandHandler<AddReviewCommand>
{
	private readonly IBookingRepository _bookingRepository;
	private readonly IDateTimeProvider _dateTimeProvider;
	private readonly IReviewRepository _reviewRepository;
	private readonly IUnitOfWork _unitOfWork;

	public AddReviewCommandHandler ( IBookingRepository bookingRepository,
									 IReviewRepository reviewRepository,
									 IUnitOfWork unitOfWork,
									 IDateTimeProvider dateTimeProvider )
	{
		_bookingRepository = bookingRepository;
		_reviewRepository = reviewRepository;
		_unitOfWork = unitOfWork;
		_dateTimeProvider = dateTimeProvider;
	}

	public async Task<Result> Handle ( AddReviewCommand request, CancellationToken cancellationToken )
	{
		var booking = await _bookingRepository.GetByIdAsync (
							  id : request.BookingId,
							  cancellationToken : cancellationToken
						  );

		if ( booking is null )
			return Result.Failure (
					error : BookingErrors.NotFound
				);

		var ratingResult = Rating.Create (
				value : request.Rating
			);

		if ( ratingResult.IsFailure )
			return Result.Failure (
					error : ratingResult.Error
				);

		var reviewResult = Review.Create (
				booking : booking,
				rating : ratingResult.Value,
				comment : new Comment (
						Value : request.Comment
					),
				createdOnUtc : _dateTimeProvider.UtcNow
			);

		if ( reviewResult.IsFailure )
			return Result.Failure (
					error : reviewResult.Error
				);

		_reviewRepository.Add (
				review : reviewResult.Value
			);

		await _unitOfWork.SaveChangesAsync (
				cancellationToken : cancellationToken
			);

		return Result.Success();
	}
}
