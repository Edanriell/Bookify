namespace Bookify.Domain.Apartments;

// When we have strings in our anemic domain model, we can easily almost always replace those values with a value object
// Ask yourself; what is an empty string? What are strings with one character mean? We can't know that for sure
public record Name(string Value); 