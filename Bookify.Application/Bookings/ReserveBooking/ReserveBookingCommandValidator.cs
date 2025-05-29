using FluentValidation;

namespace Bookify.Application.Bookings.ReserveBooking;

internal class ReserveBookingCommandValidator : AbstractValidator<ReserveBookingCommand>
{
	public ReserveBookingCommandValidator()
	{
		RuleFor (
					expression : c => c.UserId
				).
			NotEmpty();

		RuleFor (
					expression : c => c.ApartmentId
				).
			NotEmpty();

		RuleFor (
					expression : c => c.StartDate
				).
			LessThan (
					expression : c => c.EndDate
				);
	}
}
