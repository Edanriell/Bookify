using Bookify.Application.Bookings.GetBooking;
using Bookify.Application.Bookings.ReserveBooking;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Api.Controllers.Bookings;

[Authorize]
[ApiController]
[Route("api/bookings")]
public class BookingsController : ControllerBase
{
	private readonly ISender _sender;

	public BookingsController(ISender sender)
	{
		_sender = sender;
	}

	[HttpGet("{id}")]
	// Resource-based Authorization
	// Only user which is created corresponding booking can get access to it
	public async Task<IActionResult> GetBooking(Guid id, CancellationToken cancellationToken)
	{
		// Creates a new get booking query instance and send it using MediatR
		var query = new GetBookingQuery(id);

		var result = await _sender.Send(query, cancellationToken);

		// Then if the result is successful it means that we found our booking,
		// and we are going to return a 200 okay result containing the booking response, otherwise
		// we are going to return 404 not found 
		return result.IsSuccess ? Ok(result.Value) : NotFound();
	}

	[HttpPost]
	public async Task<IActionResult> ReserveBooking(
		ReserveBookingRequest request,
		CancellationToken cancellationToken)
	{
		// We are mapping our reserve booking request to the reserve
		// booking command.
		var command = new ReserveBookingCommand(
			request.ApartmentId,
			request.UserId,
			request.StartDate,
			request.EndDate);

		// We are sending this command using MediatR which is going to trigger our command handler
		var result = await _sender.Send(command, cancellationToken);

		// If this is a failure result, we are going to return a bad request response containing 
		// the result error
		if (result.IsFailure) return BadRequest(result.Error);

		// Otherwise, if the result is successful, it means that we were able to reserve the booking, and 
		// we are going to return a 201 created response. This is a RESTful API convention,
		// and the response is going to contain a location header with the route to the get booking endpoint
		// and the id of the newly created booking.  
		return CreatedAtAction(nameof(GetBooking), new { id = result.Value }, result.Value);
	}
}