namespace Bookify.Api.Controllers.Bookings;

// ReserveBookingCommand is part of our internal API and it
// is only valid inside of the scope of our application. 
// We don't want to expose this on our API endpoint because then 
// we are coupling our endpoints and the definition of our command
// This is leaking information about our internal API which we never want to do and
// secondly it is preventing our command from envolving separately from our endpoint. 
// We might want to introduce some additional information on the command which we can populate inside of
// our API request, and we wouldn't necessarily want to expose all of these values as part
// of our API contracts.  
public sealed record ReserveBookingRequest(
	Guid ApartmentId,
	Guid UserId,
	DateOnly StartDate,
	DateOnly EndDate);