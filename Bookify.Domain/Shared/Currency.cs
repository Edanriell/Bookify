namespace Bookify.Domain.Apartments;

// Amount and currency essentially represent money, and this is a great candidate to introduce a value object
// because money is an important concept of the apartment booking domain.
// We do not use only the default constructor in this case because we want more control over our currency value object
public record Currency
{
	// Supported currencies by our system
	// None currency value does not have currency code, and we aren't going to return it 
	// from the list of all currencies because we don't want to expose it to the outside our domain project
	// The internal keyword is allowing us to achieve that by hiding this field from the outside the domain assembly
	internal static readonly Currency None = new("");
	public static readonly Currency Usd = new("USD");
	public static readonly Currency Eur = new("EUR");

	// Exposing all currencies in our system
	public static readonly IReadOnlyCollection<Currency> All = new[]
															   {
																   Usd,
																   Eur
															   };

	// Private constructor
	private Currency(string code)
	{
		Code = code;
	}

	public string Code { get; init; }

	// This method allows us to pass in currency code and get back the currency instance
	public static Currency FromCode(string code)
	{
		// We are scanning all of our currencies by using FirstOrDefault method, if currency code matches one that is passed in, we return an instance of currency
		// If method FirstOrDefault returns null, we throw an exception "The currency code is invalid"
		return All.FirstOrDefault(c => c.Code == code) ??
			   throw new ApplicationException("The currency code is invalid");
	}
}