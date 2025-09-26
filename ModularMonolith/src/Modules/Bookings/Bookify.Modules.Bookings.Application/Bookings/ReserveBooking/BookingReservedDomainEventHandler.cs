using Bookify.Application.Abstractions.Email;
using Bookify.Modules.Bookings.Domain.Bookings;
using Bookify.Modules.Bookings.Domain.Bookings.Events;
using Bookify.Modules.Bookings.Domain.Customers;
using MediatR;

namespace Bookify.Modules.Bookings.Application.Bookings.ReserveBooking;

internal sealed class BookingReservedDomainEventHandler : INotificationHandler<BookingReservedDomainEvent>
{
	private readonly IBookingRepository _bookingRepository;
	private readonly ICustomerRepository _customerRepository;
	private readonly IEmailService _emailService;

	public BookingReservedDomainEventHandler(
		IBookingRepository bookingRepository,
		ICustomerRepository customerRepository,
		IEmailService emailService)
	{
		_bookingRepository = bookingRepository;
		_customerRepository = customerRepository;
		_emailService = emailService;
	}

	public async Task Handle(BookingReservedDomainEvent notification, CancellationToken cancellationToken)
	{
		Booking? booking = await _bookingRepository.GetByIdAsync(notification.BookingId, cancellationToken);

		if (booking is null)
		{
			return;
		}

		Customer? customer = await _customerRepository.GetByIdAsync(booking.UserId, cancellationToken);

		if (customer is null)
		{
			return;
		}

		await _emailService.SendAsync(
			customer.Email,
			"Booking reserved!",
			"You have 10 minutes to confirm this booking");
	}
}
