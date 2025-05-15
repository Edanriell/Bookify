namespace Bookify.Domain.Shared;

public record Money(decimal Amount, Currency Currency)
{
	// Operator overloading, if we want to add to instances of Money class, we can use + operator
	public static Money operator +(Money first, Money second)
	{
		// The Currency has to be equal, otherwise we can't add two money instances
		if (first.Currency != second.Currency) throw new InvalidOperationException("Currencies have to be equal");

		return new Money(first.Amount + second.Amount, first.Currency);
	}

	// Static method which allows us to create a money instance with no value
	public static Money Zero()
	{
		return new Money(0, Currency.None);
	}

	public static Money Zero(Currency currency)
	{
		return new Money(0, currency);
	}

	public bool IsZero()
	{
		return this == Zero(Currency);
	}
}