namespace Bookify.Domain.Apartments;

// If we do not intend to inherit a class at any point,it makes sense to make it a sealed class.
// Also, it might give a small performance boost in some situations. 
// Apartment entity is an anemic domain model
// because it is basically a bag of data,and it contains only properties and does not contain any behavior logic
public sealed class Apartment
{
	public Guid Id { get; private set; }
	public string Name { get; private set; }
	public string Description { get; private set; }
	public string Country { get; private set; }
	public string State { get; private set; }
	public string ZipCode { get; private set; }
	public string City { get; private set; }
	public string Street { get; private set; }
	public decimal PriceAmount { get; private set; }
	public string PriceCurrency { get; private set; }
	public decimal CleaningFeeAmount { get; private set; }
	public string CleaningFeeCurrency { get; private set; }
	public DateTime? LastBookedOnUtc { get; private set; }
	public List<Amenity> Amenities { get; private set; } = new();
}