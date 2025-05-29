namespace Bookify.Domain.Shared;

public sealed record Currency
{
	internal static readonly Currency None = new(
			code : ""
		);

	public static readonly Currency Usd = new(
			code : "USD"
		);

	public static readonly Currency Eur = new(
			code : "EUR"
		);

	public static readonly IReadOnlyCollection<Currency> All = new[]
															   {
																   Usd,
																   Eur
															   };

	private Currency ( string code ) { Code = code; }

	public string Code { get; init; }

	public static Currency FromCode ( string code )
	{
		return All.FirstOrDefault (
					   predicate : c => c.Code == code
				   )
			?? throw new ApplicationException (
					   message : "The currency code is invalid"
				   );
	}
}
