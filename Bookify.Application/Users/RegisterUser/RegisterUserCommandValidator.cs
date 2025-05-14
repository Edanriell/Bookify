using FluentValidation;

namespace Bookify.Application.Users.RegisterUser;

internal sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
	public RegisterUserCommandValidator()
	{
		RuleFor(c => c.FirstName).NotEmpty();

		RuleFor(c => c.LastName).NotEmpty();

		RuleFor(c => c.Email).NotEmpty();

		RuleFor(c => c.Password).NotEmpty().MinimumLength(5);
	}
}