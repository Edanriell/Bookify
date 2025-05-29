using Bookify.Domain.Abstractions;
using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings.Events;
using Bookify.Domain.Shared;

namespace Bookify.Domain.Bookings;

public sealed class Booking : Entity
{
	private Booking ( Guid id,
					  Guid apartmentId,
					  Guid userId,
					  DateRange duration,
					  Money priceForPeriod,
					  Money cleaningFee,
					  Money amenitiesUpCharge,
					  Money totalPrice,
					  BookingStatus status,
					  DateTime createdOnUtc )
		: base (
				id : id
			)
	{
		ApartmentId = apartmentId;
		UserId = userId;
		Duration = duration;
		PriceForPeriod = priceForPeriod;
		CleaningFee = cleaningFee;
		AmenitiesUpCharge = amenitiesUpCharge;
		TotalPrice = totalPrice;
		Status = status;
		CreatedOnUtc = createdOnUtc;
	}

	private Booking() { }

	public Guid ApartmentId { get; private set; }

	public Guid UserId { get; private set; }

	public DateRange Duration { get; }

	public Money PriceForPeriod { get; private set; }

	public Money CleaningFee { get; private set; }

	public Money AmenitiesUpCharge { get; private set; }

	public Money TotalPrice { get; private set; }

	public BookingStatus Status { get; private set; }

	public DateTime CreatedOnUtc { get; private set; }

	public DateTime? ConfirmedOnUtc { get; private set; }

	public DateTime? RejectedOnUtc { get; private set; }

	public DateTime? CompletedOnUtc { get; private set; }

	public DateTime? CancelledOnUtc { get; private set; }

	public static Booking Reserve ( Apartment apartment,
									Guid userId,
									DateRange duration,
									DateTime utcNow,
									PricingService pricingService )
	{
		var pricingDetails = pricingService.CalculatePrice (
				apartment : apartment,
				period : duration
			);

		var booking = new Booking (
				id : Guid.NewGuid(),
				apartmentId : apartment.Id,
				userId : userId,
				duration : duration,
				priceForPeriod : pricingDetails.PriceForPeriod,
				cleaningFee : pricingDetails.CleaningFee,
				amenitiesUpCharge : pricingDetails.AmenitiesUpCharge,
				totalPrice : pricingDetails.TotalPrice,
				status : BookingStatus.Reserved,
				createdOnUtc : utcNow
			);

		booking.RaiseDomainEvent (
				domainEvent : new BookingReservedDomainEvent (
						BookingId : booking.Id
					)
			);

		apartment.LastBookedOnUtc = utcNow;

		return booking;
	}

	public Result Confirm ( DateTime utcNow )
	{
		if ( Status != BookingStatus.Reserved )
			return Result.Failure (
					error : BookingErrors.NotReserved
				);

		Status = BookingStatus.Confirmed;
		ConfirmedOnUtc = utcNow;

		RaiseDomainEvent (
				domainEvent : new BookingConfirmedDomainEvent (
						BookingId : Id
					)
			);

		return Result.Success();
	}

	public Result Reject ( DateTime utcNow )
	{
		if ( Status != BookingStatus.Reserved )
			return Result.Failure (
					error : BookingErrors.NotReserved
				);

		Status = BookingStatus.Rejected;
		RejectedOnUtc = utcNow;

		RaiseDomainEvent (
				domainEvent : new BookingRejectedDomainEvent (
						BookingId : Id
					)
			);

		return Result.Success();
	}

	public Result Complete ( DateTime utcNow )
	{
		if ( Status != BookingStatus.Confirmed )
			return Result.Failure (
					error : BookingErrors.NotConfirmed
				);

		Status = BookingStatus.Completed;
		CompletedOnUtc = utcNow;

		RaiseDomainEvent (
				domainEvent : new BookingCompletedDomainEvent (
						BookingId : Id
					)
			);

		return Result.Success();
	}

	public Result Cancel ( DateTime utcNow )
	{
		if ( Status != BookingStatus.Confirmed )
			return Result.Failure (
					error : BookingErrors.NotConfirmed
				);

		var currentDate = DateOnly.FromDateTime (
				dateTime : utcNow
			);

		if ( currentDate > Duration.Start )
			return Result.Failure (
					error : BookingErrors.AlreadyStarted
				);

		Status = BookingStatus.Cancelled;
		CancelledOnUtc = utcNow;

		RaiseDomainEvent (
				domainEvent : new BookingCancelledDomainEvent (
						BookingId : Id
					)
			);

		return Result.Success();
	}
}
