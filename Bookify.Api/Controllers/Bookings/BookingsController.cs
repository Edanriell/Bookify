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
	// Resource-based Authorization
	// Only user which is created corresponding booking can get access to it
	public async Task<IActionResult> GetBooking ( Guid id, CancellationToken cancellationToken )
	{
		// Creates a new get booking query instance and send it using MediatR
		var query = new GetBookingQuery (
				BookingId : id
			);

		var result = await _sender.Send (
							 request : query,
							 cancellationToken : cancellationToken
						 );

		// Then if the result is successful it means that we found our booking,
		// and we are going to return a 200 okay result containing the booking response, otherwise
		// we are going to return 404 not found 
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
		// We are mapping our reserve booking request to the reserve
		// booking command.
		var command = new ReserveBookingCommand (
				ApartmentId : request.ApartmentId,
				UserId : request.UserId,
				StartDate : request.StartDate,
				EndDate : request.EndDate
			);

		// We are sending this command using MediatR which is going to trigger our command handler
		var result = await _sender.Send (
							 request : command,
							 cancellationToken : cancellationToken
						 );

		// If this is a failure result, we are going to return a bad request response containing 
		// the result error
		if ( result.IsFailure )
		{
			return BadRequest (
					error : result.Error
				);
		}

		// Otherwise, if the result is successful, it means that we were able to reserve the booking, and 
		// we are going to return a 201 created response. This is a RESTful API convention,
		// and the response is going to contain a location header with the route to the get booking endpoint
		// and the id of the newly created booking.  
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
