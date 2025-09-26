using Bookify.Application.Abstractions.Messaging;
using Bookify.Modules.Bookings.Domain.Apartments;

namespace Bookify.Modules.Bookings.Application.Apartments.UpdateApartments;

public sealed record UpdateApartmentCommand(
	Guid ApartmentId,
	decimal PriceAmount,
	string PriceAmountCurrency,
	decimal CleaningFeeAmount,
	string CleaningFeeCurrency,
	Amenity[] Amenities) : ICommand;
