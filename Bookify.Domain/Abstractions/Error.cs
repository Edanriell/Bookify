namespace Bookify.Domain.Abstractions;

// Error represents that something went wrong and we want to assign it a unique code and a name
public record Error(string Code, string Name)
{
	public static Error None = new(string.Empty, string.Empty);

	public static Error NullValue = new("Error.NullValue", "Null value was provided");
}