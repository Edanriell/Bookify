using Bookify.Application.Apartments.SearchApartments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Api.Controllers.Apartments;

// Particular attribute tells .NET runtime that this is supposed to be a controller
// It's more lightweight and requires fewer services, and it's a little bit more performant
// than an MVC style controller. 
[Authorize]
[ApiController]
[Route("api/apartments")]
// We dont need anything that comes with Controller that's why we use ControllerBase
public class ApartmentsController : ControllerBase
{
	private readonly ISender _sender;

	public ApartmentsController(ISender sender)
	{
		_sender = sender;
	}

	[HttpGet]
	public async Task<IActionResult> SearchApartments(
		DateOnly startDate,
		DateOnly endDate,
		CancellationToken cancellationToken)
	{
		// Creating a new instance of the search apartments query, and we are going to pass it the
		// start and end date values that we get from our endpoint, then we need a way to send this
		// query using mediator, and for that we are going to inject a service into our controller, which is called
		// ISender, and it comes from MediatR
		var query = new SearchApartmentsQuery(startDate, endDate);

		// Result object that we get back is a result containg a read-only list of apartment responses
		var result = await _sender.Send(query, cancellationToken);

		// Because this query can never fail, we are going to return Ok, and we are going to return
		// the list of apartment responses that is returned by our query. 
		return Ok(result.Value);
	}
}