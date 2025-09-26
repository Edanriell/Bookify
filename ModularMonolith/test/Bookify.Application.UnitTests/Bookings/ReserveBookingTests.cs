using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Exceptions;
using Bookify.Application.UnitTests.Apartments;
using Bookify.Application.UnitTests.Users;
using Bookify.Domain.Abstractions;
using Bookify.Modules.Bookings.Application.Bookings.ReserveBooking;
using Bookify.Modules.Bookings.Domain.Apartments;
using Bookify.Modules.Bookings.Domain.Bookings;
using Bookify.Modules.Bookings.Domain.Customers;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Bookify.Application.UnitTests.Bookings;

public class ReserveBookingTests
{
	private static readonly DateTime UtcNow = DateTime.UtcNow;

	private static readonly ReserveBookingCommand Command = new(
		Guid.NewGuid(),
		Guid.NewGuid(),
		new DateOnly(2024, 1, 1),
		new DateOnly(2024, 1, 10));

	private readonly IApartmentRepository _apartmentRepositoryMock;
	private readonly IBookingRepository _bookingRepositoryMock;

	private readonly ReserveBookingCommandHandler _handler;
	private readonly IUnitOfWork _unitOfWorkMock;
	private readonly ICustomerRepository _userRepositoryMock;

	public ReserveBookingTests()
	{
		_userRepositoryMock = Substitute.For<ICustomerRepository>();
		_apartmentRepositoryMock = Substitute.For<IApartmentRepository>();
		_bookingRepositoryMock = Substitute.For<IBookingRepository>();
		_unitOfWorkMock = Substitute.For<IUnitOfWork>();

		IDateTimeProvider dateTimeProviderMock = Substitute.For<IDateTimeProvider>();
		dateTimeProviderMock.UtcNow.Returns(UtcNow);

		_handler = new ReserveBookingCommandHandler(
			_userRepositoryMock,
			_apartmentRepositoryMock,
			_bookingRepositoryMock,
			_unitOfWorkMock,
			new PricingService(),
			dateTimeProviderMock);
	}

	[Fact]
	public async Task Handle_Should_ReturnFailure_WhenUserIsNull()
	{
		// Arrange
		_userRepositoryMock
			.GetByIdAsync(Command.UserId, Arg.Any<CancellationToken>())
			.Returns((Customer?)null);

		// Act
		Result<Guid> result = await _handler.Handle(Command, default);

		// Assert
		result.Error.Should().Be(CustomerErrors.NotFound);
	}

	[Fact]
	public async Task Handle_Should_ReturnFailure_WhenApartmentIsNull()
	{
		// Arrange
		Customer user = CustomerData.Create();

		_userRepositoryMock
			.GetByIdAsync(Command.UserId, Arg.Any<CancellationToken>())
			.Returns(user);

		_apartmentRepositoryMock
			.GetByIdAsync(Command.ApartmentId, Arg.Any<CancellationToken>())
			.Returns((Apartment?)null);

		// Act
		Result<Guid> result = await _handler.Handle(Command, default);

		// Assert
		result.Error.Should().Be(ApartmentErrors.NotFound);
	}

	[Fact]
	public async Task Handle_Should_ReturnFailure_WhenApartmentIsBooked()
	{
		// Arrange
		Customer user = CustomerData.Create();
		Apartment apartment = ApartmentData.Create();
		var duration = DateRange.Create(Command.StartDate, Command.EndDate);

		_userRepositoryMock
			.GetByIdAsync(Command.UserId, Arg.Any<CancellationToken>())
			.Returns(user);

		_apartmentRepositoryMock
			.GetByIdAsync(Command.ApartmentId, Arg.Any<CancellationToken>())
			.Returns(apartment);

		_bookingRepositoryMock
			.IsOverlappingAsync(apartment, duration, Arg.Any<CancellationToken>())
			.Returns(true);

		// Act
		Result<Guid> result = await _handler.Handle(Command, default);

		// Assert
		result.Error.Should().Be(BookingErrors.Overlap);
	}

	[Fact]
	public async Task Handle_Should_ReturnFailure_WhenUnitOfWorkThrows()
	{
		// Arrange
		Customer user = CustomerData.Create();
		Apartment apartment = ApartmentData.Create();
		var duration = DateRange.Create(Command.StartDate, Command.EndDate);

		_userRepositoryMock
			.GetByIdAsync(Command.UserId, Arg.Any<CancellationToken>())
			.Returns(user);

		_apartmentRepositoryMock
			.GetByIdAsync(Command.ApartmentId, Arg.Any<CancellationToken>())
			.Returns(apartment);

		_bookingRepositoryMock
			.IsOverlappingAsync(apartment, duration, Arg.Any<CancellationToken>())
			.Returns(false);

		_unitOfWorkMock
			.SaveChangesAsync()
			.ThrowsAsync(new ConcurrencyException("Concurrency", new Exception()));

		// Act
		Result<Guid> result = await _handler.Handle(Command, default);

		// Assert
		result.Error.Should().Be(BookingErrors.Overlap);
	}

	[Fact]
	public async Task Handle_Should_ReturnSuccess_WhenBookingIsReserved()
	{
		// Arrange
		Customer user = CustomerData.Create();
		Apartment apartment = ApartmentData.Create();
		var duration = DateRange.Create(Command.StartDate, Command.EndDate);

		_userRepositoryMock
			.GetByIdAsync(Command.UserId, Arg.Any<CancellationToken>())
			.Returns(user);

		_apartmentRepositoryMock
			.GetByIdAsync(Command.ApartmentId, Arg.Any<CancellationToken>())
			.Returns(apartment);

		_bookingRepositoryMock
			.IsOverlappingAsync(apartment, duration, Arg.Any<CancellationToken>())
			.Returns(false);

		// Act
		Result<Guid> result = await _handler.Handle(Command, default);

		// Assert
		result.IsSuccess.Should().BeTrue();
	}

	[Fact]
	public async Task Handle_Should_CallRepository_WhenBookingIsReserved()
	{
		// Arrange
		Customer user = CustomerData.Create();
		Apartment apartment = ApartmentData.Create();
		var duration = DateRange.Create(Command.StartDate, Command.EndDate);

		_userRepositoryMock
			.GetByIdAsync(Command.UserId, Arg.Any<CancellationToken>())
			.Returns(user);

		_apartmentRepositoryMock
			.GetByIdAsync(Command.ApartmentId, Arg.Any<CancellationToken>())
			.Returns(apartment);
		_bookingRepositoryMock
			.IsOverlappingAsync(apartment, duration, Arg.Any<CancellationToken>())
			.Returns(false);

		// Act
		Result<Guid> result = await _handler.Handle(Command, default);

		// Assert
		_bookingRepositoryMock.Received(1).Add(Arg.Is<Booking>(b => b.Id == result.Value));
	}
}
