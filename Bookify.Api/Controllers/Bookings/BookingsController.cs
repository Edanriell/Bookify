using Asp.Versioning;
using Bookify.Application.Bookings.GetBooking;
using Bookify.Application.Bookings.ReserveBooking;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Api.Controllers.Bookings;

[ Authorize ]
[ ApiController ]
[ ApiVersion (
		version : ApiVersions.V1
	) ]
[ Route (
		template : "api/v{version:apiVersion}/bookings"
	) ]
public class BookingsController : ControllerBase
{
	private readonly ISender _sender;

	public BookingsController ( ISender sender ) { _sender = sender; }

	[ HttpGet (
			template : "{id}"
		) ]
	public async Task<IActionResult> GetBooking ( Guid id, CancellationToken cancellationToken )
	{
		var query = new GetBookingQuery (
				BookingId : id
			);

		var result = await _sender.Send (
							 request : query,
							 cancellationToken : cancellationToken
						 );

		return result.IsSuccess
				   ? Ok (
						   value : result.Value
					   )
				   : NotFound();
	}

	[ HttpPost ]
	public async Task<IActionResult> ReserveBooking ( ReserveBookingRequest request,
													  CancellationToken cancellationToken )
	{
		var command = new ReserveBookingCommand (
				ApartmentId : request.ApartmentId,
				UserId : request.UserId,
				StartDate : request.StartDate,
				EndDate : request.EndDate
			);

		var result = await _sender.Send (
							 request : command,
							 cancellationToken : cancellationToken
						 );

		if ( result.IsFailure )
			return BadRequest (
					error : result.Error
				);

		return CreatedAtAction (
				actionName : nameof(GetBooking),
				routeValues : new
							  {
								  id = result.Value
							  },
				value : result.Value
			);
	}
}
