using Bookify.Application.Bookings.GetBooking;
using Bookify.Application.Bookings.ReserveBooking;
using MediatR;

namespace Bookify.Api.Controllers.Bookings;

// Minimal Api example
public static class BookingsEndpoints
{
	public static IEndpointRouteBuilder MapBookingsEndpoints ( this IEndpointRouteBuilder builder )
	{
		// If we implemented a route group we don't need code below
//		var apiVersionSet = builder.NewApiVersionSet().
//			HasApiVersion (
//					apiVersion : new ApiVersion (
//							majorVersion : 1
//						)
//				).
//			ReportApiVersions().
//			Build();

		builder.MapGet (
					// If we implement route group we don't need this
//					pattern : "api/v{version:apiVersion}/bookings/{id}",
					pattern : "bookings{id}",
					handler : GetBooking
				).
			RequireAuthorization().
			WithName (
					endpointName : nameof(GetBooking)
				);
		// If we implement route group we don't need this
//			WithApiVersionSet (
//					apiVersionSet : apiVersionSet
//				);
		// RequireAuthorization("bookings:read");
		// .HasApiVersion(1)

		builder.MapPost (
					// If we implement route group we don't need this
//					pattern : "api/v{version:apiVersion}/bookings",
					pattern : "bookings",
					handler : ReserveBooking
				).
			RequireAuthorization();
		// If we implement route group we don't need this
//			WithApiVersionSet (
//					apiVersionSet : apiVersionSet
//				);
		;

		return builder;
	}

	public static async Task<IResult> GetBooking ( Guid id, ISender sender, CancellationToken cancellationToken )
	{
		var query = new GetBookingQuery (
				BookingId : id
			);

		var result = await sender.Send (
							 request : query,
							 cancellationToken : cancellationToken
						 );

		return result.IsSuccess
				   ? Results.Ok (
						   value : result.Value
					   )
				   : Results.NotFound();
	}

	public static async Task<IResult> ReserveBooking ( ReserveBookingRequest request,
													   ISender sender,
													   CancellationToken cancellationToken )
	{
		var command = new ReserveBookingCommand (
				ApartmentId : request.ApartmentId,
				UserId : request.UserId,
				StartDate : request.StartDate,
				EndDate : request.EndDate
			);

		var result = await sender.Send (
							 request : command,
							 cancellationToken : cancellationToken
						 );

		if ( result.IsFailure )
		{
			return Results.BadRequest (
					error : result.Error
				);
		}

		return Results.CreatedAtRoute (
				routeName : nameof(GetBooking),
				routeValues : new
							  {
								  id = result.Value
							  },
				value : result.Value
			);
	}
}
