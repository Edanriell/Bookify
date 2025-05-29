using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Abstractions.Messaging;
using Bookify.Application.Exceptions;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;
using Bookify.Domain.Users;

namespace Bookify.Application.Bookings.ReserveBooking;

internal sealed class ReserveBookingCommandHandler : ICommandHandler<ReserveBookingCommand, Guid>
{
	private readonly IApartmentRepository _apartmentRepository;
	private readonly IBookingRepository _bookingRepository;
	private readonly IDateTimeProvider _dateTimeProvider;
	private readonly PricingService _pricingService;
	private readonly IUnitOfWork _unitOfWork;
	private readonly IUserRepository _userRepository;

	public ReserveBookingCommandHandler ( IUserRepository userRepository,
										  IApartmentRepository apartmentRepository,
										  IBookingRepository bookingRepository,
										  IUnitOfWork unitOfWork,
										  PricingService pricingService,
										  IDateTimeProvider dateTimeProvider )
	{
		_userRepository = userRepository;
		_apartmentRepository = apartmentRepository;
		_bookingRepository = bookingRepository;
		_unitOfWork = unitOfWork;
		_pricingService = pricingService;
		_dateTimeProvider = dateTimeProvider;
	}

	public async Task<Result<Guid>> Handle ( ReserveBookingCommand request, CancellationToken cancellationToken )
	{
		var user = await _userRepository.GetByIdAsync (
						   id : request.UserId,
						   cancellationToken : cancellationToken
					   );

		if ( user is null )
			return Result.Failure<Guid> (
					error : UserErrors.NotFound
				);

		var apartment = await _apartmentRepository.GetByIdAsync (
								id : request.ApartmentId,
								cancellationToken : cancellationToken
							);

		if ( apartment is null )
			return Result.Failure<Guid> (
					error : ApartmentErrors.NotFound
				);

		var duration = DateRange.Create (
				start : request.StartDate,
				end : request.EndDate
			);

		if ( await _bookingRepository.IsOverlappingAsync (
					 apartment : apartment,
					 duration : duration,
					 cancellationToken : cancellationToken
				 ) )
			return Result.Failure<Guid> (
					error : BookingErrors.Overlap
				);

		try
		{
			var booking = Booking.Reserve (
					apartment : apartment,
					userId : user.Id,
					duration : duration,
					utcNow : _dateTimeProvider.UtcNow,
					pricingService : _pricingService
				);

			_bookingRepository.Add (
					booking : booking
				);

			await _unitOfWork.SaveChangesAsync (
					cancellationToken : cancellationToken
				);

			return booking.Id;
		}
		catch ( ConcurrencyException )
		{
			return Result.Failure<Guid> (
					error : BookingErrors.Overlap
				);
		}
	}
}
