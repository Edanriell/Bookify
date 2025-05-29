using Bookify.Application.Abstractions.Messaging;
using Bookify.Application.Exceptions;
using FluentValidation;
using MediatR;
using ValidationException = Bookify.Application.Exceptions.ValidationException;

namespace Bookify.Application.Abstractions.Behaviors;

internal sealed class ValidationBehavior <TRequest, TResponse>
	: IPipelineBehavior<TRequest, TResponse>
	where TRequest : IBaseCommand
{
	private readonly IEnumerable<IValidator<TRequest>> _validators;

	public ValidationBehavior ( IEnumerable<IValidator<TRequest>> validators ) { _validators = validators; }

	public async Task<TResponse> Handle ( TRequest request,
										  RequestHandlerDelegate<TResponse> next,
										  CancellationToken cancellationToken )
	{
		if ( !_validators.Any() )
			return await next();

		var context = new ValidationContext<TRequest> (
				instanceToValidate : request
			);

		var validationErrors = _validators.Select (
					selector : validator => validator.Validate (
							context : context
						)
				).
			Where (
					predicate : validationResult => validationResult.Errors.Any()
				).
			SelectMany (
					selector : validationResult => validationResult.Errors
				).
			Select (
					selector : validationFailure => new ValidationError (
							PropertyName : validationFailure.PropertyName,
							ErrorMessage : validationFailure.ErrorMessage
						)
				).
			ToList();

		if ( validationErrors.Any() )
			throw new ValidationException (
					errors : validationErrors
				);

		return await next();
	}
}
