namespace Bookify.Infrastructure.Outbox;

// Transactional Outbox Pattern
public sealed class OutboxOptions
{
	// Represents how often we want to run our background job
	public int IntervalInSeconds { get; init; }

	// This field represents how many outbox messages 
	// we are going to read in one single run of the background job 
	// and then publish them all one by one. 
	public int BatchSize { get; init; }
}
