namespace Bookify.Infrastructure.Outbox;

// Distributed systems are inherently unrealiable, and if we think about it, even a 
// system consisting of a single API and a database can be considered a distributed system.
// Whenever our database is down so, is ours application because they are tightly coupled, and this becomes even more 
// complicated when we are working in a microservices environment, and we need to communicate between
// multiple services. So how does an outbox pattern help us to improve the reliability of our system?
// Instead of coupling our business transaction, which in case could be registering a user and communicating with
// any external services, we can process the side effects of the business transaction in the background. 

// Transactional Outbox Pattern
// Represents an outbox message
public sealed class OutboxMessage
{
	public OutboxMessage ( Guid id, DateTime occurredOnUtc, string type, string content )
	{
		Id = id;
		OccurredOnUtc = occurredOnUtc;
		Content = content;
		Type = type;
	}

	public Guid Id { get; init; }

	public DateTime OccurredOnUtc { get; init; }

	// Message type represents the fully qualified name of the domain event that is
	// going to be serialized into an outbox message 
	public string Type { get; init; }

	// and the content is going to be a
	// JSON string representing my domain event instance 
	public string Content { get; init; }

	// We are going to use the processed on the UTC column
	// to determine if this is an outbox message that has been proccessed or not.
	// If it is not processed, we are going to handle it using a background worker
	public DateTime? ProcessedOnUtc { get; init; }

	public string? Error { get; init; }
}
