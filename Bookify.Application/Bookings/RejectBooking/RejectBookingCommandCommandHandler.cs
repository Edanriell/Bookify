using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Abstractions.Messaging;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Bookings;

namespace Bookify.Application.Bookings.RejectBooking;

internal sealed class RejectBookingCommandCommandHandler : ICommandHandler<RejectBookingCommand>
{
	private readonly IBookingRepository _bookingRepository;
	private readonly IDateTimeProvider _dateTimeProvider;
	private readonly IUnitOfWork _unitOfWork;

	public RejectBookingCommandCommandHandler ( IDateTimeProvider dateTimeProvider,
												IBookingRepository bookingRepository,
												IUnitOfWork unitOfWork )
	{
		_bookingRepository = bookingRepository;
		_unitOfWork = unitOfWork;
		_dateTimeProvider = dateTimeProvider;
	}

	public async Task<Result> Handle ( RejectBookingCommand request,
									   CancellationToken cancellationToken )
	{
		var booking = await _bookingRepository.GetByIdAsync (
							  id : request.BookingId,
							  cancellationToken : cancellationToken
						  );

		if ( booking is null )
			return Result.Failure (
					error : BookingErrors.NotFound
				);

		var result = booking.Reject (
				utcNow : _dateTimeProvider.UtcNow
			);

		if ( result.IsFailure )
			return result;

		await _unitOfWork.SaveChangesAsync (
				cancellationToken : cancellationToken
			);

		return Result.Success();
	}
}
