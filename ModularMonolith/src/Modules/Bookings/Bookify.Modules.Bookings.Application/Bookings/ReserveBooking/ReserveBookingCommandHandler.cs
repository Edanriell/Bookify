using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Abstractions.Messaging;
using Bookify.Application.Exceptions;
using Bookify.Domain.Abstractions;
using Bookify.Modules.Bookings.Domain.Apartments;
using Bookify.Modules.Bookings.Domain.Bookings;
using Bookify.Modules.Bookings.Domain.Customers;

namespace Bookify.Modules.Bookings.Application.Bookings.ReserveBooking;

internal sealed class ReserveBookingCommandHandler : ICommandHandler<ReserveBookingCommand, Guid>
{
	private readonly IApartmentRepository _apartmentRepository;
	private readonly IBookingRepository _bookingRepository;
	private readonly ICustomerRepository _customerRepository;
	private readonly IDateTimeProvider _dateTimeProvider;
	private readonly PricingService _pricingService;
	private readonly IUnitOfWork _unitOfWork;

	public ReserveBookingCommandHandler(
		ICustomerRepository customerRepository,
		IApartmentRepository apartmentRepository,
		IBookingRepository bookingRepository,
		IUnitOfWork unitOfWork,
		PricingService pricingService,
		IDateTimeProvider dateTimeProvider)
	{
		_customerRepository = customerRepository;
		_apartmentRepository = apartmentRepository;
		_bookingRepository = bookingRepository;
		_unitOfWork = unitOfWork;
		_pricingService = pricingService;
		_dateTimeProvider = dateTimeProvider;
	}

	public async Task<Result<Guid>> Handle(ReserveBookingCommand request, CancellationToken cancellationToken)
	{
		Customer? user = await _customerRepository.GetByIdAsync(request.UserId, cancellationToken);

		if (user is null)
		{
			return Result.Failure<Guid>(CustomerErrors.NotFound);
		}

		Apartment? apartment = await _apartmentRepository.GetByIdAsync(request.ApartmentId, cancellationToken);

		if (apartment is null)
		{
			return Result.Failure<Guid>(ApartmentErrors.NotFound);
		}

		var duration = DateRange.Create(request.StartDate, request.EndDate);

		if (await _bookingRepository.IsOverlappingAsync(apartment, duration, cancellationToken))
		{
			return Result.Failure<Guid>(BookingErrors.Overlap);
		}

		try
		{
			var booking = Booking.Reserve(
				apartment,
				user.Id,
				duration,
				_dateTimeProvider.UtcNow,
				_pricingService);

			_bookingRepository.Add(booking);

			await _unitOfWork.SaveChangesAsync(cancellationToken);

			return booking.Id;
		}
		catch (ConcurrencyException)
		{
			return Result.Failure<Guid>(BookingErrors.Overlap);
		}
	}
}
