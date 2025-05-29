using FluentValidation;

namespace Bookify.Application.Users.RegisterUser;

internal sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
	public RegisterUserCommandValidator()
	{
		RuleFor (
					expression : c => c.FirstName
				).
			NotEmpty();

		RuleFor (
					expression : c => c.LastName
				).
			NotEmpty();

		RuleFor (
					expression : c => c.Email
				).
			EmailAddress();

		RuleFor (
					expression : c => c.Password
				).
			NotEmpty().
			MinimumLength (
					minimumLength : 5
				);
	}
}
