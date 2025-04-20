using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Apartments;

// If we do not intend to inherit a class at any point,it makes sense to make it a sealed class.
// Also, it might give a small performance boost in some situations. 

// Apartment entity is an anemic domain model
// because it is basically a bag of data,and it contains only properties and does not contain any behavior logic

// The Problem with primitive data types,particularly with strings, which are a part of our domain model, is that they convey no meaning
// To solve primitive obsession and also improve the design of our entity we must use a value object 
// After all our refactoring we have a Rich domain model instead of an anemic one

// Important!
// We have private setters on all of the properties, because we don't want to allow any values inside of our entity to be 
// changeable outside the scope of the entity. If we had public setters, someone could take properties and change the value
// therefore breaking the invariants that are already enforced by our defensive design
public sealed class Apartment : Entity
{
	// Constructor accepts as an argument an identifier and
	// passes it down to the base constructor of the entity abstract class
	// We must implement this ctor! 
	public Apartment(Guid id, Name name, Description description, Address address, Money price, Money cleaningFee,
					 List<Amenity> amenities) : base(id)
	{
		Name = name;
		Description = description;
		Address = address;
		Price = price;
		CleaningFee = cleaningFee;
		Amenities = amenities;
	}

	// Id property comes from Entity abstract class, if it stays we will hide an Id property which comes from Entity abstract class
//	public Guid Id { get; private set; }

	public Name Name { get; private set; }
//	public string Name { get; private set; }

	public Description Description { get; private set; }
//	public string Description { get; private set; }

	public Address Address { get; private set; }
//	public string Country { get; private set; }
//	public string State { get; private set; }
//	public string ZipCode { get; private set; }
//	public string City { get; private set; }
//	public string Street { get; private set; }

	public Money Price { get; private set; }
//	public decimal PriceAmount { get; private set; }
//	public string PriceCurrency { get; private set; }

	public Money CleaningFee { get; private set; }
//	public decimal CleaningFeeAmount { get; private set; }
//	public string CleaningFeeCurrency { get; private set; }

	public DateTime? LastBookedOnUtc { get; private set; }
	public List<Amenity> Amenities { get; private set; } = new();
}