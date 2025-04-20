namespace Bookify.Domain.Apartments;

// Entities are uniquely identified by their identity (id). On the other hand, value objects are uniquely identified by their values, which essentially means that 
// value objects have a structural equality. Records fully support this, so it is one of the indicators that tell us that record is a solid choice for the value object.
// Also, value objects must be immutable, and records also support this! So overall, records are The best way to define a value object.
// However, sometimes in real world scenarios we will see ValueObject base class, where we implement what are the equality members basically specifying all the properties 
// and then using the values of those properties to determine the value object equality! 
public record Address(string Country, string State, string ZipCode, string City, string Street);