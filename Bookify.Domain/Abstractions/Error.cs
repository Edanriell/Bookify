namespace Bookify.Domain.Abstractions;

public record Error ( string Code, string Name )
{
	public static readonly Error None = new(
			Code : string.Empty,
			Name : string.Empty
		);

	public static readonly Error NullValue = new(
			Code : "Error.NullValue",
			Name : "Null value was provided"
		);
}
