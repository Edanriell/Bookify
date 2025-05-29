using Asp.Versioning;
using Bookify.Application.Reviews.AddReview;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Api.Controllers.Reviews;

[ Authorize ]
[ ApiController ]
[ ApiVersion (
		version : ApiVersions.V1
	) ]
[ Route (
		template : "api/v{version:apiVersion}/reviews"
	) ]
public class ReviewsController : ControllerBase
{
	private readonly ISender _sender;

	public ReviewsController ( ISender sender ) { _sender = sender; }

	[ HttpPost ]
	public async Task<IActionResult> AddReview ( AddReviewRequest request, CancellationToken cancellationToken )
	{
		var command = new AddReviewCommand (
				BookingId : request.BookingId,
				Rating : request.Rating,
				Comment : request.Comment
			);

		var result = await _sender.Send (
							 request : command,
							 cancellationToken : cancellationToken
						 );

		if ( result.IsFailure )
			return BadRequest (
					error : result.Error
				);

		return Ok();
	}
}
