using Bookify.Application.Abstractions.Messaging;
using FluentValidation;
using MediatR;
using ValidationException = Bookify.Application.Abstractions.Exceptions.ValidationException;

namespace Bookify.Application.Abstractions.Behaviors;

// Pipeline behavior it has a request and response generic argument, and the request 
// has to be a base command because we only want to be running validation for our commands.
public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : IBaseCommand
{
	// FluentValidation library exposes the IValidator interface we can inject one or more
	// IValidator instances for our TRequest which is going to be our command.
	// We are injecting here any validators that are defined for this TRequest in our case this is
	// going to be the command. 
	private readonly IEnumerable<IValidator<TRequest>> _validators;

	public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
	{
		_validators = validators;
	}

	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
										CancellationToken cancellationToken)
	{
		// We check if we have any validators defined for this type. 
		// If we don't have any validators, let's just invoke the command handler
		// and return from the pipeline. 
		if (!_validators.Any()) return await next();

		// Otherwise, if we do have validators,
		// we need to create a new validation context and pass it to our command instance 
		var context = new ValidationContext<TRequest>(request);

		// Then we are going to execute our validators by iterating over them
		// using the select method from the lin library. 
		var validationErrors = _validators
			// We are going to call the validate method on each of the validators
		   .Select(validator => validator.Validate(context))
			// and if the validation results returned by this method have an error 
		   .Where(validationResult => validationResult.Errors.Any())
			// we are going to select all of them, and we are going to project 
		   .SelectMany(validationResult => validationResult.Errors)
			// these errors into a new validation error instance. 
		   .Select(validationFailure => new ValidationError(
				validationFailure.PropertyName,
				validationFailure.ErrorMessage))
		   .ToList();

		// After getting back a list of validation errors, we check if we 
		// have any errors present, and if that is true, we are going to throw our
		// custom validation exception.
		if (validationErrors.Any()) throw new ValidationException(validationErrors);

		// And if we don't have any validation errors, we can execute our command normally
		return await next();
	}
}