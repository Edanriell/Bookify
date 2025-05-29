using Asp.Versioning;
using Bookify.Application.Apartments.SearchApartments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Api.Controllers.Apartments;

[ Authorize ]
[ ApiController ]
[ ApiVersion (
		version : ApiVersions.V1
	) ]
[ Route (
		template : "api/v{version:apiVersion}/apartments"
	) ]
public class ApartmentsController : ControllerBase
{
	private readonly ISender _sender;

	public ApartmentsController ( ISender sender ) { _sender = sender; }

	[ HttpGet ]
	public async Task<IActionResult> SearchApartments ( DateOnly startDate,
														DateOnly endDate,
														CancellationToken cancellationToken )
	{
		var query = new SearchApartmentsQuery (
				StartDate : startDate,
				EndDate : endDate
			);

		var result = await _sender.Send (
							 request : query,
							 cancellationToken : cancellationToken
						 );

		return Ok (
				value : result.Value
			);
	}
}
