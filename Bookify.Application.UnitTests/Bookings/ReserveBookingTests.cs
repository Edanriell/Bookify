using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Bookings.ReserveBooking;
using Bookify.Application.Exceptions;
using Bookify.Application.UnitTests.Apartments;
using Bookify.Application.UnitTests.Users;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;
using Bookify.Domain.Users;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Bookify.Application.UnitTests.Bookings;

public class ReserveBookingTests
{
	private static readonly DateTime UtcNow = DateTime.UtcNow;

	private static readonly ReserveBookingCommand Command = new(
			ApartmentId : Guid.NewGuid(),
			UserId : Guid.NewGuid(),
			StartDate : new DateOnly (
					year : 2024,
					month : 1,
					day : 1
				),
			EndDate : new DateOnly (
					year : 2024,
					month : 1,
					day : 10
				)
		);

	private readonly IApartmentRepository _apartmentRepositoryMock;
	private readonly IBookingRepository _bookingRepositoryMock;

	private readonly ReserveBookingCommandHandler _handler;
	private readonly IUnitOfWork _unitOfWorkMock;
	private readonly IUserRepository _userRepositoryMock;

	public ReserveBookingTests()
	{
		_userRepositoryMock = Substitute.For<IUserRepository>();
		_apartmentRepositoryMock = Substitute.For<IApartmentRepository>();
		_bookingRepositoryMock = Substitute.For<IBookingRepository>();
		_unitOfWorkMock = Substitute.For<IUnitOfWork>();

		var dateTimeProviderMock = Substitute.For<IDateTimeProvider>();
		dateTimeProviderMock.UtcNow.Returns (
				returnThis : UtcNow
			);

		_handler = new ReserveBookingCommandHandler (
				userRepository : _userRepositoryMock,
				apartmentRepository : _apartmentRepositoryMock,
				bookingRepository : _bookingRepositoryMock,
				unitOfWork : _unitOfWorkMock,
				pricingService : new PricingService(),
				dateTimeProvider : dateTimeProviderMock
			);
	}

	[ Fact ]
	public async Task Handle_Should_ReturnFailure_WhenUserIsNull()
	{
		// Arrange
		_userRepositoryMock.GetByIdAsync (
					id : Command.UserId,
					cancellationToken : Arg.Any<CancellationToken>()
				).
			Returns (
					returnThis : (User?)null
				);

		// Act
		var result = await _handler.Handle (
							 request : Command,
							 cancellationToken : default(CancellationToken)
						 );

		// Assert
		result.Error.Should().
			Be (
					expected : UserErrors.NotFound
				);
	}

	[ Fact ]
	public async Task Handle_Should_ReturnFailure_WhenApartmentIsNull()
	{
		// Arrange
		var user = UserData.Create();

		_userRepositoryMock.GetByIdAsync (
					id : Command.UserId,
					cancellationToken : Arg.Any<CancellationToken>()
				).
			Returns (
					returnThis : user
				);

		_apartmentRepositoryMock.GetByIdAsync (
					id : Command.ApartmentId,
					cancellationToken : Arg.Any<CancellationToken>()
				).
			Returns (
					returnThis : (Apartment?)null
				);

		// Act
		var result = await _handler.Handle (
							 request : Command,
							 cancellationToken : default(CancellationToken)
						 );

		// Assert
		result.Error.Should().
			Be (
					expected : ApartmentErrors.NotFound
				);
	}

	[ Fact ]
	public async Task Handle_Should_ReturnFailure_WhenApartmentIsBooked()
	{
		// Arrange
		var user = UserData.Create();
		var apartment = ApartmentData.Create();
		var duration = DateRange.Create (
				start : Command.StartDate,
				end : Command.EndDate
			);

		_userRepositoryMock.GetByIdAsync (
					id : Command.UserId,
					cancellationToken : Arg.Any<CancellationToken>()
				).
			Returns (
					returnThis : user
				);

		_apartmentRepositoryMock.GetByIdAsync (
					id : Command.ApartmentId,
					cancellationToken : Arg.Any<CancellationToken>()
				).
			Returns (
					returnThis : apartment
				);

		_bookingRepositoryMock.IsOverlappingAsync (
					apartment : apartment,
					duration : duration,
					cancellationToken : Arg.Any<CancellationToken>()
				).
			Returns (
					returnThis : true
				);

		// Act
		var result = await _handler.Handle (
							 request : Command,
							 cancellationToken : default(CancellationToken)
						 );

		// Assert
		result.Error.Should().
			Be (
					expected : BookingErrors.Overlap
				);
	}

	[ Fact ]
	public async Task Handle_Should_ReturnFailure_WhenUnitOfWorkThrows()
	{
		// Arrange
		var user = UserData.Create();
		var apartment = ApartmentData.Create();
		var duration = DateRange.Create (
				start : Command.StartDate,
				end : Command.EndDate
			);

		_userRepositoryMock.GetByIdAsync (
					id : Command.UserId,
					cancellationToken : Arg.Any<CancellationToken>()
				).
			Returns (
					returnThis : user
				);

		_apartmentRepositoryMock.GetByIdAsync (
					id : Command.ApartmentId,
					cancellationToken : Arg.Any<CancellationToken>()
				).
			Returns (
					returnThis : apartment
				);

		_bookingRepositoryMock.IsOverlappingAsync (
					apartment : apartment,
					duration : duration,
					cancellationToken : Arg.Any<CancellationToken>()
				).
			Returns (
					returnThis : false
				);

		_unitOfWorkMock.SaveChangesAsync().
			ThrowsAsync (
					ex : new ConcurrencyException (
							message : "Concurrency",
							innerException : new Exception()
						)
				);

		// Act
		var result = await _handler.Handle (
							 request : Command,
							 cancellationToken : default(CancellationToken)
						 );

		// Assert
		result.Error.Should().
			Be (
					expected : BookingErrors.Overlap
				);
	}

	[ Fact ]
	public async Task Handle_Should_ReturnSuccess_WhenBookingIsReserved()
	{
		// Arrange
		var user = UserData.Create();
		var apartment = ApartmentData.Create();
		var duration = DateRange.Create (
				start : Command.StartDate,
				end : Command.EndDate
			);

		_userRepositoryMock.GetByIdAsync (
					id : Command.UserId,
					cancellationToken : Arg.Any<CancellationToken>()
				).
			Returns (
					returnThis : user
				);

		_apartmentRepositoryMock.GetByIdAsync (
					id : Command.ApartmentId,
					cancellationToken : Arg.Any<CancellationToken>()
				).
			Returns (
					returnThis : apartment
				);

		_bookingRepositoryMock.IsOverlappingAsync (
					apartment : apartment,
					duration : duration,
					cancellationToken : Arg.Any<CancellationToken>()
				).
			Returns (
					returnThis : false
				);

		// Act
		var result = await _handler.Handle (
							 request : Command,
							 cancellationToken : default(CancellationToken)
						 );

		// Assert
		result.IsSuccess.Should().
			BeTrue();
	}

	[ Fact ]
	public async Task Handle_Should_CallRepository_WhenBookingIsReserved()
	{
		// Arrange
		var user = UserData.Create();
		var apartment = ApartmentData.Create();
		var duration = DateRange.Create (
				start : Command.StartDate,
				end : Command.EndDate
			);

		_userRepositoryMock.GetByIdAsync (
					id : Command.UserId,
					cancellationToken : Arg.Any<CancellationToken>()
				).
			Returns (
					returnThis : user
				);

		_apartmentRepositoryMock.GetByIdAsync (
					id : Command.ApartmentId,
					cancellationToken : Arg.Any<CancellationToken>()
				).
			Returns (
					returnThis : apartment
				);
		_bookingRepositoryMock.IsOverlappingAsync (
					apartment : apartment,
					duration : duration,
					cancellationToken : Arg.Any<CancellationToken>()
				).
			Returns (
					returnThis : false
				);

		// Act
		var result = await _handler.Handle (
							 request : Command,
							 cancellationToken : default(CancellationToken)
						 );

		// Assert
		_bookingRepositoryMock.Received (
					requiredNumberOfCalls : 1
				).
			Add (
					booking : Arg.Is<Booking> (
							predicate : b => b.Id == result.Value
						)
				);
	}
}
