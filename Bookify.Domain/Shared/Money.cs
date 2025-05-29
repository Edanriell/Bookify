namespace Bookify.Domain.Shared;

public sealed record Money ( decimal Amount, Currency Currency )
{
	public static Money operator + ( Money first, Money second )
	{
		if ( first.Currency != second.Currency )
			throw new InvalidOperationException (
					message : "Currencies have to be equal"
				);

		return new Money (
				Amount : first.Amount + second.Amount,
				Currency : first.Currency
			);
	}

	public static Money Zero() => new(
			Amount : 0,
			Currency : Currency.None
		);

	public static Money Zero ( Currency currency ) => new(
			Amount : 0,
			Currency : currency
		);

	public bool IsZero() => this
						 == Zero (
									currency : Currency
								);
}
