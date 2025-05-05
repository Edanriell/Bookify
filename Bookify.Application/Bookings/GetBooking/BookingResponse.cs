namespace Bookify.Application.Bookings.GetBooking;

// For the query responses, we want to be using primitive types as
// much as possible, and we want a very flat structure which we can easily return from the database. 
// All of these properties are already present on the booking, however, they are hidden behind
// the rich domain model and value objects. 
public sealed class BookingResponse
{
	public Guid Id { get; init; }
	public Guid UserId { get; init; }
	public Guid ApartmentId { get; init; }
	public int Status { get; init; }
	public decimal PriceAmount { get; init; }
	public string PriceCurrency { get; init; }
	public decimal CleaningFeeAmount { get; init; }
	public string CleaningFeeCurrency { get; init; }
	public decimal AmenitiesUpChargeAmount { get; init; }
	public string AmenitiesUpChargeCurrency { get; init; }
	public decimal TotalPriceAmount { get; init; }
	public string TotalPriceCurrency { get; init; }
	public DateOnly DurationStart { get; init; }
	public DateOnly DurationEnd { get; init; }
	public DateTime CreatedOnUtc { get; init; }
}