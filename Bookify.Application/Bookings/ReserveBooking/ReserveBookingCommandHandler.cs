using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Abstractions.Messaging;
using Bookify.Application.Exceptions;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;
using Bookify.Domain.Users;

namespace Bookify.Application.Bookings.ReserveBooking;

// In ICommandHandler we pass the command we are handling, also this command handler returns Guid response.
internal sealed class ReserveBookingCommandHandler : ICommandHandler<ReserveBookingCommand, Guid>
{
	private readonly IApartmentRepository _apartmentRepository;
	private readonly IBookingRepository _bookingRepository;
	private readonly IDateTimeProvider _dateTimeProvider;
	private readonly PricingService _pricingService;
	private readonly IUnitOfWork _unitOfWork;
	private readonly IUserRepository _userRepository;

	public ReserveBookingCommandHandler(
		IUserRepository userRepository,
		IApartmentRepository apartmentRepository,
		IBookingRepository bookingRepository,
		IUnitOfWork unitOfWork,
		PricingService pricingService,
		IDateTimeProvider dateTimeProvider)
	{
		_userRepository = userRepository;
		_apartmentRepository = apartmentRepository;
		_bookingRepository = bookingRepository;
		_unitOfWork = unitOfWork;
		_pricingService = pricingService;
		_dateTimeProvider = dateTimeProvider;
	}

	public async Task<Result<Guid>> Handle(ReserveBookingCommand request, CancellationToken cancellationToken)
	{
		// Fetching user entity  using the repository
		var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

		if (user is null) return Result.Failure<Guid>(UserErrors.NotFound);

		// Fetching apartment entity 
		var apartment = await _apartmentRepository.GetByIdAsync(request.ApartmentId, cancellationToken);

		if (apartment is null) return Result.Failure<Guid>(ApartmentErrors.NotFound);

		// Creating the date range value object 
		var duration = DateRange.Create(request.StartDate, request.EndDate);

		// IMPORTANT NOTE:
		// RACE CONDITION BELOW!
		// The problem is that we are performing an optimistic check on the database to see if we have an overlap for this
		// apartment and duration. If this check succeeds, and we get back the information that there is no overlap
		// then we go ahead and create our booking and try to persist it in the database. The problem is that we could have
		// two separate transactions, both trying to persist the booking after succeeding on this check. 
		// There is two ways to solve this problem, pessimistic or optimistic locking. Pessimistic locking means creating
		// a transaction with some of the more constrictive isolation levels. Optimistic locking means that we have
		// a concurrency token present on our entitites, and actually this is the approach that we are going to take. 
		// The reason for using optimistic locking is that it's more performant, and it doesn't require locking a certain number
		// of rows in the database for an extended period of time. 

		// Performing optimistic check on the database to see if we are overlapping for this apartments booking.
		if (await _bookingRepository.IsOverlappingAsync(apartment, duration, cancellationToken))
			return Result.Failure<Guid>(BookingErrors.Overlap);
		// Otherwise, we are going to fail here if somebody actually did manage to reserve a booking before us
		// and the user interface should handle this accordingly. 

//		// If checks succeeds we are reserving the booking
//		var booking = Booking.Reserve(
//			apartment,
//			user.Id,
//			duration,
//			// It is always better to use abstractions
//			// The benefit of this approach is that this is completely testable 
//			// If we  can't reliably test our code that is working with a date time object
//			// then we risk introducing many potential bugs and this approach using a date
//			// time provider abstraction solves that because we can easily mock it and provide the date and time
//			// value that we need to satisfy the test.
//			_dateTimeProvider.UtcNow,
////			DateTime.UtcNow,
//			_pricingService);
//
//		// Adding it to the repository 
//		_bookingRepository.Add(booking);
//
//		// And persisting the changes to the database. ! 
//		await _unitOfWork.SaveChangesAsync(cancellationToken);
//
//		return booking.Id;

		try
		{
			var booking = Booking.Reserve(
				apartment, user.Id, duration, _dateTimeProvider.UtcNow, _pricingService);

			_bookingRepository.Add(booking);

			await _unitOfWork.SaveChangesAsync(cancellationToken);

			return booking.Id;
		}
		catch (ConcurrencyException)
		{
			// We are catching the concurrency exception, and we know that this
			// is because we have an overlap at the database level. So somebody else
			// managed to reserve this booking before we could, and they win. We return an overlap error.
			// On the client side, we could retry reserving the booking again in case this was a transient failure.
			return Result.Failure<Guid>(BookingErrors.Overlap);
		}
	}
}