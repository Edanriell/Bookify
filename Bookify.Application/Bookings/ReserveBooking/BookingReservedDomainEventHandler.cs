using Bookify.Application.Abstractions.Email;
using Bookify.Domain.Bookings;
using Bookify.Domain.Bookings.Events;
using Bookify.Domain.Users;
using MediatR;

namespace Bookify.Application.Bookings.ReserveBooking;

// The Naming convention we are using is pretty consistent. We always specify what is the command, query,
// or event, and then we append handler. 
// The DI configuration that we added for mediator is going to take care of wiring up this class, and all we
// have to do is implement the INotificationHandler interface and specify our domain event.
internal sealed class BookingReservedDomainEventHandler : INotificationHandler<BookingReservedDomainEvent>
{
	private readonly IBookingRepository _bookingRepository;
	private readonly IEmailService _emailService;
	private readonly IUserRepository _userRepository;

	public BookingReservedDomainEventHandler(IBookingRepository bookingRepository, IEmailService emailService,
											 IUserRepository userRepository)
	{
		_bookingRepository = bookingRepository;
		_emailService = emailService;
		_userRepository = userRepository;
	}

	// We are currently leveraging repositories, and this isn't the most performant approach.
	// This is something that we can live with in our command handlers, but you have to ask yourself if this
	// is the approach that you want to take in the event handlers. In this case, it might be justified,
	// but we could also use the same approach that we are going to use for query handlers, and that is executing 
	// SQL queries on the database directly using dapper. 

	// Sending an email to the user when the booking is reserved.
	public async Task Handle(BookingReservedDomainEvent notification, CancellationToken cancellationToken)
	{
		// Fetching the booking from the repository. 
		var booking = await _bookingRepository.GetByIdAsync(notification.BookingId, cancellationToken);

		if (booking is null) return;

		// Fetching the user from the repository.
		var user = await _userRepository.GetByIdAsync(booking.UserId, cancellationToken);

		if (user is null) return;

		// Sending the email.
		await _emailService.SendAsync(
			user.Email,
			"Booking reserved!",
			"You have 10 minutes to confirm your booking.");
	}
}