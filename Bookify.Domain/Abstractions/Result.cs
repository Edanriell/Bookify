using System.Diagnostics.CodeAnalysis;

namespace Bookify.Domain.Abstractions;

public class Result
{
	// Can only be accessed from this assembly inside of this type
	protected internal Result(bool isSuccess, Error error)
	{
		if (isSuccess && error != Error.None) throw new InvalidOperationException();

		if (!isSuccess && error == Error.None) throw new InvalidOperationException();

		isSuccess = isSuccess;
		Error = error;
	}

	public bool IsSuccess { get; }

	public bool IsFailure => !IsSuccess;

	public Error Error { get; }

	public static Result Success()
	{
		return new Result(true, Error.None);
	}

	public static Result Failure(Error error)
	{
		return new Result(false, error);
	}

	public static Result<TValue> Success<TValue>(TValue value)
	{
		return new Result<TValue>(value, true, Error.None);
	}

	public static Result<TValue> Failure<TValue>(Error error)
	{
		return new Result<TValue>(default, false, error);
	}

	public static Result<TValue> Create<TValue>(TValue? value)
	{
		return value is not null ? Success(value) : Failure<TValue>(Error.NullValue);
	}
}

public class Result<TValue> : Result
{
	private readonly TValue? _value;

	protected internal Result(TValue? value, bool isSuccess, Error error) : base(isSuccess, error)
	{
		_value = value;
	}

	// We can access the value only if this is a success result
	[NotNull]
	public TValue Value => IsSuccess
							   ? _value!
							   : throw new InvalidOperationException(
									 "The value of a failure result can not be accessed.");

	public static implicit operator Result<TValue>(TValue? value)
	{
		return Create(value);
	}
}