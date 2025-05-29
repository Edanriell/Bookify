using System.Diagnostics.CodeAnalysis;

namespace Bookify.Domain.Abstractions;

public class Result
{
	public Result ( bool isSuccess, Error error )
	{
		if ( isSuccess && error != Error.None )
			throw new InvalidOperationException();

		if ( !isSuccess && error == Error.None )
			throw new InvalidOperationException();

		IsSuccess = isSuccess;
		Error = error;
	}

	public bool IsSuccess { get; }

	public bool IsFailure => !IsSuccess;

	public Error Error { get; }

	public static Result Success() => new(
			isSuccess : true,
			error : Error.None
		);

	public static Result Failure ( Error error ) => new(
			isSuccess : false,
			error : error
		);

	public static Result<TValue> Success <TValue> ( TValue value ) => new(
			value : value,
			isSuccess : true,
			error : Error.None
		);

	public static Result<TValue> Failure <TValue> ( Error error ) => new(
			value : default(TValue?),
			isSuccess : false,
			error : error
		);

	public static Result<TValue> Create <TValue> ( TValue? value ) => value is not null
																		  ? Success (
																				  value : value
																			  )
																		  : Failure<TValue> (
																				  error : Error.NullValue
																			  );
}

public sealed class Result <TValue> : Result
{
	private readonly TValue? _value;

	public Result ( TValue? value, bool isSuccess, Error error )
		: base (
				isSuccess : isSuccess,
				error : error
			)
	{
		_value = value;
	}

	[ NotNull ]
	public TValue Value => IsSuccess
							   ? _value!
							   : throw new InvalidOperationException (
										 message : "The value of a failure result can not be accessed."
									 );

	public static implicit operator Result<TValue> ( TValue? value ) => Create (
			value : value
		);
}
